using System.Collections;
using System.Collections.Generic;
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

    [Header("Swarm Stats")]
	[Tooltip("How much space a swarm takes.")]
	[SerializeField] private int maxSwarmRadius;
	[Tooltip("How many swarms should be in a night.")]
	[SerializeField] private int maxSwarmCount;
	[Tooltip("How many aliens can be in a swarm.")]
	[SerializeField] private int maxSwarmSize;
	[Tooltip("How many extra aliens will spawn per existing building.")]
	[SerializeField] private int alienMultiplier;

	[Header("Penalty Stats")]
	[Tooltip("If the player takes this long to build a defence building, they'll get penalised.")]
	[SerializeField] private float defencePenaltyThreshold;
	[Tooltip("If the player takes this long to build a non defence building, they'll get penalised.")]
	[SerializeField] private float nonDefencePenaltyThreshold;
	[Tooltip("How long a penalty interval is before the game punishes the player again.")]
	[SerializeField] private float penaltyCooldown;
	[Tooltip("Number of aliens to punish the player every penalty interval.")]
	[SerializeField] private int penaltyIncrement;
	[Tooltip("How long the player needs to be AFK before the game starts punishing.")]
	[SerializeField] private float timeOfLastPenalty;

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
    System.Diagnostics.Stopwatch loopStopwatch;

    //Penalty Incrementation
    private int spawnCountPenalty;

    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    ///// <summary>
    ///// AlienController's singleton public property.
    ///// </summary>
    //public static AlienController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// A list of all aliens
    /// </summary>
    public List<Alien> Aliens { get => aliens; }

    //Complex Public Properties--------------------------------------------------------------------

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        //if (Instance != null)
        //{
        //    Debug.LogError("There should never be 2 or more AlienControllers.");
        //}

        //Instance = this;
        base.Awake();
        aliens = new List<Alien>();
        timeOfLastDeath = waveDelay * -1;
        timeOfLastPenalty = penaltyCooldown * -1;
        spawnCountPenalty = 0;
        groundLayerMask = LayerMask.GetMask("Ground");

        spawnableStages = new List<EStage>() { EStage.Combat, EStage.MainGame };
        gameOverStages = new List<EStage>() { EStage.Win, EStage.Lose };

        //Setting up spawning optimisation variables
        loopStopwatch = new System.Diagnostics.Stopwatch();

        //Setting up position offsets that can be randomly selected from for cluster spawning 
        swarmOffsets = new Dictionary<int, List<Vector3>>();

        for (int i = 0; i <= maxSwarmRadius; i++)
        {
            swarmOffsets[i] = new List<Vector3>();
        }

        for (int i = maxSwarmRadius * -1; i <= maxSwarmRadius; i++)
        {
            for (int j = maxSwarmRadius * -1; j <= maxSwarmRadius; j++)
            {
                int iMag = MathUtility.Instance.IntMagnitude(i);
                int jMag = MathUtility.Instance.IntMagnitude(j);
                Vector3 pos = new Vector3(i, 0, j);

                foreach (KeyValuePair<int, List<Vector3>> p in swarmOffsets)
                {
                    if ((iMag == p.Key || jMag == p.Key) && iMag <= p.Key && jMag <= p.Key)
                    {
                        p.Value.Add(pos);
                    }
                }
            }
        }
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

        while (spawnAliens && !gameOverStages.Contains(StageManager.Instance.CurrentStage.ID))
        {
            if (spawnableStages.Contains(StageManager.Instance.CurrentStage.ID) 
                && !ClockController.Instance.Daytime 
                && aliens.Count == 0 
                && Time.time - timeOfLastDeath > waveDelay)
            {
                //Reset start time
                loopStopwatch.Restart();

                //Check and increment penalty
                if (Time.time - timeOfLastPenalty > penaltyCooldown && (Time.time - BuildingController.Instance.TimeLastDefenceWasBuilt > defencePenaltyThreshold || Time.time - BuildingController.Instance.TimeLastNonDefenceWasBuilt > nonDefencePenaltyThreshold))
                {
                    spawnCountPenalty += penaltyIncrement;
                    timeOfLastPenalty = Time.time;
                }

                //Spawn aliens
                EStage currentStage = StageManager.Instance.CurrentStage.ID;
                int spawnCount = (currentStage == EStage.Combat ? 3 : BuildingController.Instance.BuildingCount * alienMultiplier + spawnCountPenalty);
                Vector3 swarmCentre = Vector3.zero;
                int swarmSize = 0;
                int swarmRadius = 0;
                int swarmCount = 0;
                float offsetMultiplier = 2;
                List<Vector3> availableOffsets = new List<Vector3>();
                List<Vector3> availablePositions = MapController.Instance.GetAlienSpawnablePositions();

                for (int i = 0; i < spawnCount; i++)
                //for (int i = 0; i < 100; i++)
                {
                    if (loopStopwatch.ElapsedMilliseconds >= spawningFrameTimeLimit)
                    {
                        yield return null;
                        loopStopwatch.Restart();
                    }

                    if (availableOffsets.Count == 0)
                    {
                        if (swarmSize >= maxSwarmSize || swarmRadius >= swarmOffsets.Count)
                        {
                            swarmCount++;

                            if (swarmCount >= maxSwarmCount || availablePositions.Count == 0)
                            {
                                break;
                            }

                            swarmCentre = GetRandomPosition(availablePositions);
                            swarmRadius = 0;
                            swarmSize = 0;
                            yield return new WaitForSeconds(swarmDelay);
                        }

                        availableOffsets.AddRange(swarmOffsets[swarmRadius]);
                        swarmRadius++;
                    }

                    int j = Random.Range(0, availableOffsets.Count);
                    Vector3 spawnPos = swarmCentre + availableOffsets[j] * offsetMultiplier;
                    availableOffsets.RemoveAt(j);

                    if (MapController.Instance.PositionAvailableForSpawning(spawnPos, true) || currentStage == EStage.Combat)
                    {
                        RaycastHit rayHit;
                        NavMeshHit navHit;
                        Physics.Raycast(spawnPos, Vector3.down, out rayHit, 25, groundLayerMask);
                        Alien alien = AlienFactory.Instance.Get(new Vector3(spawnPos.x, rayHit.point.y, spawnPos.z));
                        alien.Setup(IdGenerator.Instance.GetNextId());

                        if (NavMesh.SamplePosition(alien.transform.position, out navHit, 1, NavMesh.AllAreas))
                        {
                            aliens.Add(alien);
                            swarmSize++;
                        }
                        else
                        {
                            MapController.Instance.RegisterOffMeshPosition(spawnPos);
                            AlienFactory.Instance.Destroy(alien);
                            i--;
                        }
                        
                        for (int m = -2; m <= 2; m++)
                        {
                            for (int n = -2; n <= 2; n++)
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
    /// Returns a random position from a list of Vector3 positions.
    /// </summary>
    /// <param name="positions">The list of Vector3 positions to choose from.</param>
    /// <returns>The chosen Vector3 position.</returns>
    private Vector3 GetRandomPosition(List<Vector3> positions)
    {
        switch (positions.Count)
        {
            case 0:
                return new Vector3(-1, AlienFactory.Instance.AlienSpawnHeight, -1);
            case 1:
                return positions[0];
            default:
                return positions[Random.Range(0, positions.Count)];
        }
    }

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
