using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controller class for aliens.
/// </summary>
public class AlienController : SerializableSingleton<AlienController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Spawning Stats")]
    [Tooltip("How long the game should wait between the death of all of each wave of aliens before spawning more.")]
    [SerializeField] private float waveDelay;
    [Tooltip("How long the game should wait between spawning each swarm of aliens in a wave.")]
    [SerializeField] private float swarmDelay;
    [Tooltip("How many waves of aliens should the player have to defeat in a night?")]
    [SerializeField] private float wavesPerNight;
	[Tooltip("What is the maximum number of aliens that will spawn per building in a wave? (Number in inspector is the initial value for the first night.)")]
	[SerializeField] private float maxAliensPerBuilding;
    [Tooltip("How much the max. number of aliens per wave increase each night? (Increments maxAliensPerBuilding each night.)")]
    [SerializeField] private float aliensPerBuildingIncrementPerNight;
    [Tooltip("What proportion of the aliens spawned should be crawlers?")]
    [SerializeField][Range(0, 1)] private float crawlerFrequency;


    //[Header("Swarm Stats")]
    //[Tooltip("How much space a swarm takes.")]
    //[SerializeField] private int maxSwarmRadius;
    //[Tooltip("How many swarms should be in a night.")]
    //[SerializeField] private int maxSwarmCount;
    //[Tooltip("How many aliens can be in a swarm.")]
    //[SerializeField] private int maxSwarmSize;


    //[Header("Penalty Stats")]
    //[Tooltip("If the player takes this long to build a defence building, they'll get penalised.")]
    //[SerializeField] private float defencePenaltyThreshold;
    //[Tooltip("If the player takes this long to build a non defence building, they'll get penalised.")]
    //[SerializeField] private float nonDefencePenaltyThreshold;
    //[Tooltip("How long a penalty interval is before the game punishes the player again.")]
    //[SerializeField] private float penaltyCooldown;
    //[Tooltip("Number of aliens to punish the player every penalty interval.")]
    //[SerializeField] private int penaltyIncrement;
    //[Tooltip("How long the player needs to be AFK before the game starts punishing.")]
    //[SerializeField] private float timeOfLastPenalty;

    [Header("Spawning Time Limit Per Frame (Milliseconds)")]
    [SerializeField] private float spawningFrameTimeLimit;

    [Header("For Testing")]
    [SerializeField] private bool spawnAliens;
    [SerializeField] private bool spawnAlienNow;
    [SerializeField] private bool ignoreDayNightCycle;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Alien Spawning
    private List<Alien> aliens;
    private float timeOfLastDeath;
    private Dictionary<int, List<Vector3>> swarmOffsets;
    private LayerMask groundLayerMask;

    private List<EStage> spawnableStages;
    private List<EStage> gameOverStages;

    //Alien Spawning Per-Frame Optimisation
    private Stopwatch loopStopwatch;

    private int currentWave;
    private int aliensInCurrentWave;

    //Penalty Incrementation
    //private int spawnCountPenalty;

    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// A list of all aliens
    /// </summary>
    public List<Alien> Aliens { get => aliens; }

    //Complex Public Properties

    /// <summary>
    /// Percentage of waves completed in the current night, plus a portion of the current wave equal to the percentage of aliens killed in the current wave.
    /// E.g. if there's three waves, one wave has been completed and the second wave is halfway done, should return 0.5f.
    /// E.g. if there's three waves, two are completed and the third is halfway through, should return 0.83f.
    /// </summary>
    public float AlienKillProgress
    {
        get
        {
            if (currentWave == 0)
            {
                return 0;
            }
            else
            {
                return (
                            currentWave - 1                                               //Start of current wave, i.e 0 for start of first wave, 1 for start of second wave, etc.
                          + ((aliensInCurrentWave - aliens.Count) / aliensInCurrentWave)  //Plus progress % in current wave, i.e. aliens killed this wave / total aliens in wave
                       )
                       / wavesPerNight;                                                   //All over the number of waves
            }
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        aliens = new List<Alien>();
        timeOfLastDeath = waveDelay * -1;
        //timeOfLastPenalty = penaltyCooldown * -1;
        //spawnCountPenalty = 0;
        groundLayerMask = LayerMask.GetMask("Ground");
        currentWave = 0;

        spawnableStages = new List<EStage>() { EStage.Combat, EStage.MainGame };
        gameOverStages = new List<EStage>() { EStage.Win, EStage.Lose };

        //Setting up spawning optimisation variables
        loopStopwatch = new System.Diagnostics.Stopwatch();

        //Setting up position offsets that can be randomly selected from for cluster spawning 
        swarmOffsets = new Dictionary<int, List<Vector3>>();

        //for (int i = 0; i <= maxSwarmRadius; i++)
        //{
        //    swarmOffsets[i] = new List<Vector3>();
        //}

        //for (int i = maxSwarmRadius * -1; i <= maxSwarmRadius; i++)
        //{
        //    for (int j = maxSwarmRadius * -1; j <= maxSwarmRadius; j++)
        //    {
        //        int iMag = MathUtility.Instance.IntMagnitude(i);
        //        int jMag = MathUtility.Instance.IntMagnitude(j);
        //        Vector3 pos = new Vector3(i, 0, j);

        //        foreach (KeyValuePair<int, List<Vector3>> p in swarmOffsets)
        //        {
        //            if ((iMag == p.Key || jMag == p.Key) && iMag <= p.Key && jMag <= p.Key)
        //            {
        //                p.Value.Add(pos);
        //            }
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Start()
    {
        StartCoroutine(SpawnAliens());
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Spawns more aliens on a regular basis.
    /// </summary>
    private IEnumerator SpawnAliens()
    {
        while (StageManager.Instance == null || StageManager.Instance.CurrentStage == null)
        {
            yield return null;
        }

        EStage currentStage = StageManager.Instance.CurrentStage.GetID();

        while (spawnAliens && !gameOverStages.Contains(currentStage))
        {
            currentStage = StageManager.Instance.CurrentStage.GetID();

            if (ClockController.Instance.Daytime)
            {
                if (currentWave > 0)
                {
                    currentWave = 0;
                    maxAliensPerBuilding += aliensPerBuildingIncrementPerNight;
                }
            }
            else if (
                spawnableStages.Contains(currentStage)
                //&& !ClockController.Instance.Daytime //Nighttime already established by first if case
                && aliens.Count == 0
                && Time.time - timeOfLastDeath > waveDelay
                && currentWave < wavesPerNight)
            {
                //Reset start time
                loopStopwatch.Restart();

                //Start new wave
                if (currentStage != EStage.Combat)
                {
                    currentWave += 1;//TODO: double check that currentWave number get updated appropriately
                }

                ////Check and increment penalty
                //if (Time.time - timeOfLastPenalty > penaltyCooldown && (Time.time - BuildingController.Instance.TimeLastDefenceWasBuilt > defencePenaltyThreshold || Time.time - BuildingController.Instance.TimeLastNonDefenceWasBuilt > nonDefencePenaltyThreshold))
                //{
                //    spawnCountPenalty += penaltyIncrement;
                //    timeOfLastPenalty = Time.time;
                //}

                //Spawn aliens
                aliensInCurrentWave = CalculateAliensInWave(currentStage);//(currentStage == EStage.Combat ? 3 : BuildingController.Instance.BuildingCount * aliensPerBuilding + spawnCountPenalty);
                int majorityCount = 0;
                int minorityCount = 0;
                int majorityMaxCount = Mathf.RoundToInt(aliensInCurrentWave * 0.7f);//TODO: double check how majority/minority affects tutorial alien spawning.
                int minorityMaxCount = aliensInCurrentWave - majorityMaxCount;
                float minAngle = Random.Range(0, 360);
                float maxAngle = minAngle + 120;
                maxAngle = (maxAngle > 360 ? maxAngle - 360 : maxAngle);
                float cumulativeCrawlerFrequency = 0;
                Vector3 centrePos = new Vector3(100, 1, 100);
                MapController.Instance.ResetCurrentAlienSpawnablePositions();
                List<Vector3> availablePositions = MapController.Instance.CurrentAlienSpawnablePositions;

                //Vector3 swarmCentre = Vector3.zero;
                //int swarmSize = 0;
                //int swarmRadius = 0;
                //int swarmCount = 0;
                //float offsetMultiplier = 2;
                //List<Vector3> availableOffsets = new List<Vector3>();
                //List<Vector3> availablePositions = MapController.Instance.GetAlienSpawnablePositions();

                for (int i = 0; i < aliensInCurrentWave; i++)
                //for (int i = 0; i < 100; i++)
                {
                    //if (loopStopwatch.ElapsedMilliseconds >= spawningFrameTimeLimit)
                    //{
                    //    yield return null;
                    //    loopStopwatch.Restart();
                    //}

                    //if (availableOffsets.Count == 0)
                    //{
                    //    if (swarmSize >= maxSwarmSize || swarmRadius >= swarmOffsets.Count)
                    //    {
                    //        swarmCount++;

                    //        if (swarmCount >= maxSwarmCount || availablePositions.Count == 0)
                    //        {
                    //            break;
                    //        }

                    //        swarmCentre = GetRandomPosition(availablePositions);
                    //        swarmRadius = 0;
                    //        swarmSize = 0;
                    //        yield return new WaitForSeconds(swarmDelay);
                    //    }

                    //    availableOffsets.AddRange(swarmOffsets[swarmRadius]);
                    //    swarmRadius++;
                    //}

                    //int j = Random.Range(0, availableOffsets.Count);
                    //Vector3 spawnPos = swarmCentre + availableOffsets[j] * offsetMultiplier;
                    //availableOffsets.RemoveAt(j);

                    
                    Vector3 spawnPos = GetRandomAvailablePosition(availablePositions);
                    centrePos.y = spawnPos.y;
                    bool inMajority = MathUtility.Instance.AngleIsBetween(Vector3.Angle(spawnPos, centrePos), minAngle, maxAngle);

                    if ((
                               currentStage == EStage.Combat //Majority/minority spawn areas irrelevant
                            || (inMajority && majorityCount < majorityMaxCount) //If in majority spawn area and majority spawn area not maxed out
                            || (!inMajority && minorityCount < minorityMaxCount) //If in minority spawn area and minority spawn area not maxed out
                        ) 
                        && MapController.Instance.PositionAvailableForSpawning(spawnPos, true)
                    )
                    {
                        RaycastHit rayHit;
                        NavMeshHit navHit;
                        Physics.Raycast(spawnPos, Vector3.down, out rayHit, 25, groundLayerMask);
                        Alien alien = AlienFactory.Instance.Get(new Vector3(spawnPos.x, rayHit.point.y, spawnPos.z), (cumulativeCrawlerFrequency >= 1 ? EAlien.Crawler : EAlien.Scuttler));
                        alien.Setup(IdGenerator.Instance.GetNextId());

                        if (NavMesh.SamplePosition(alien.transform.position, out navHit, 1, NavMesh.AllAreas))
                        {
                            aliens.Add(alien);

                            if (cumulativeCrawlerFrequency >= 1)
                            {
                                cumulativeCrawlerFrequency -= 1;
                            }

                            cumulativeCrawlerFrequency += crawlerFrequency;
                        }
                        else
                        {
                            MapController.Instance.RegisterOffMeshPosition(spawnPos);
                            AlienFactory.Instance.Destroy(alien, alien.Type);
                            i--;
                        }

                        int margin = (alien.Type == EAlien.Scuttler ? 1 : 2);

                        for (int m = -margin; m <= margin; m++)
                        {
                            for (int n = -margin; n <= margin; n++)
                            {
                                availablePositions.Remove(new Vector3(spawnPos.x + m, spawnPos.y, spawnPos.z + n));

                                if (loopStopwatch.ElapsedMilliseconds >= spawningFrameTimeLimit)
                                {
                                    yield return null;
                                    loopStopwatch.Restart();
                                }
                            }
                        }
                    }
                    else
                    {
                        i--;
                    }
                }
            }            

            yield return null;
        }
    }

    /// <summary>
    /// Calculates the number of aliens to be spawned in the current wave.
    /// </summary>
    /// <param name="currentStage">The current stage of the game.</param>
    /// <returns>The number of aliens to be spawned in the current wave.</returns>
    private int CalculateAliensInWave(EStage currentStage)
    {
        if (currentStage == EStage.Combat)
        {
            return 3;
        }

        //int numAliens = (currentStage == EStage.Combat ? 3 : BuildingController.Instance.BuildingCount * aliensPerBuilding + spawnCountPenalty);
        float waveMultiplier = 0.5f * (1 + ((currentWave - 1) / (wavesPerNight - 1))); //First wave of the night has a multiplier of 0.5 + 0.5 * 0/(N-1), Last wave of the night has a multiplier of 0.5 + 0.5 * (N-1)/(N-1), i.e. 1.
        return Mathf.RoundToInt(BuildingController.Instance.BuildingCount * maxAliensPerBuilding * waveMultiplier);
    }

    /// <summary>
    /// Picks an available position to spawn the alien at.
    /// </summary>
    /// <param name="availablePositions">A list of positions available for alien spawning.</param>
    /// <returns>The position to spawn the alien at.</returns>
    private Vector3 GetRandomAvailablePosition(List<Vector3> availablePositions)
    {
        switch (availablePositions.Count)
        {
            case 0:
                return new Vector3(-1, -1, -1);
            case 1:
                return availablePositions[0];
            default:
                return availablePositions[Random.Range(0, availablePositions.Count - 1)];
        }
    }

    ///// <summary>
    ///// Returns a random position from a list of Vector3 positions.
    ///// </summary>
    ///// <param name="positions">The list of Vector3 positions to choose from.</param>
    ///// <returns>The chosen Vector3 position.</returns>
    //private Vector3 GetRandomPosition(List<Vector3> positions)
    //{
    //    switch (positions.Count)
    //    {
    //        case 0:
    //            return new Vector3(-1, AlienFactory.Instance.AlienSpawnHeight, -1);
    //        case 1:
    //            return positions[0];
    //        default:
    //            return positions[Random.Range(0, positions.Count)];
    //    }
    //}

    /// <summary>
    /// Removes the alien from AlienController's list of aliens.
    /// </summary>
    /// <param name="alien">The alien to be removed from AlienController's list of aliens.</param>
    public void DeRegisterAlien(Alien alien)
    {
        if (aliens.Contains(alien))
        {
            aliens.Remove(alien);
            timeOfLastDeath = Time.time;
        }
    }
}
