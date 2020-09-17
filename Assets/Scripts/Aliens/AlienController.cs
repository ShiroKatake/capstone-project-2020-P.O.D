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
    [SerializeField] private float waveDelay; //Not currently updated.
    [Tooltip("How long the game should wait between spawning each swarm of aliens in a wave.")]
    [SerializeField] private float swarmDelay; //Not currently used.
    [Tooltip("How many waves of aliens should the player have to defeat in a night?")]
    [SerializeField] private float wavesPerNight;
	[Tooltip("What is the maximum number of aliens that will spawn per building in a wave? (Number in inspector is the initial value for the first night.)")]
	[SerializeField] private float maxAliensPerBuilding;
    [Tooltip("How much the max. number of aliens per wave increase each night? (Increments maxAliensPerBuilding each night.)")]
    [SerializeField] private float aliensPerBuildingIncrementPerNight;
    [Tooltip("What range of angles clockwise from a calculated angle and relative to the centre of the map should the majority of aliens spawn within during each wave?")]
    [SerializeField][Range(0, 360)] private float angleRange;
    [Tooltip("What proportion of the aliens spawned should be crawlers?")]
    [SerializeField][Range(0, 1)] private float crawlerFrequency;

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
    private int currentWave;
    private int aliensInCurrentWave;

    private List<EStage> spawnableStages;
    private List<EStage> gameOverStages;

    //Alien Spawning Per-Frame Optimisation
    private System.Diagnostics.Stopwatch loopStopwatch;

    private int majorityCount;
    private int minorityCount;
    private int majorityMaxCount;
    private int minorityMaxCount;
    private float minAngle;
    private float maxAngle;

    private bool spawningAliens;

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
                float wavesCompleted = currentWave - 1;
                float aliensLeftInCurrentWave = spawningAliens ? aliensInCurrentWave : aliens.Count;
                float progressInCurrentWave = (aliensInCurrentWave - aliensLeftInCurrentWave) / aliensInCurrentWave;
                float result = (wavesCompleted + progressInCurrentWave) / wavesPerNight;
                //Debug.Log($"CurrentWave: {currentWave}, aliensInCurrentWave: {aliensInCurrentWave}, spawningAliens: {spawningAliens}, aliens.Count: {aliens.Count}, aliensLeftInCurrentWave: {aliensLeftInCurrentWave} progressInCurrentWave: {progressInCurrentWave}");
                return result;
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
        groundLayerMask = LayerMask.GetMask("Ground");
        currentWave = 0;
        spawningAliens = false;

        spawnableStages = new List<EStage>() { EStage.Combat, EStage.MainGame };
        gameOverStages = new List<EStage>() { EStage.Win, EStage.Lose };

        //Setting up spawning optimisation variables
        loopStopwatch = new System.Diagnostics.Stopwatch();

        //Setting up position offsets that can be randomly selected from for cluster spawning 
        swarmOffsets = new Dictionary<int, List<Vector3>>();
    }

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Start()
    {
        StartCoroutine(ControllerUpdate());
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Manages AlienController's recurring behaviours.
    /// </summary>
    private IEnumerator ControllerUpdate()
    {
        while (StageManager.Instance == null || StageManager.Instance.CurrentStage == null)
        {
            yield return null;
        }

        EStage currentStage = StageManager.Instance.CurrentStage.GetID();

        while (spawnAliens && !gameOverStages.Contains(currentStage))
        {
            currentStage = StageManager.Instance.CurrentStage.GetID();

            if (ClockController.Instance.Daytime && currentWave != 0)
            {
                currentWave = 0;
            }

            if (MapController.Instance.MajorityAlienSpawnPoints.Count == 0 && MapController.Instance.MinorityAlienSpawnPoints.Count == 0)
            {
                loopStopwatch.Restart();
                MapController.Instance.ResetCurrentAlienSpawnPoints();
                yield return StartCoroutine(SortSpawnPointsByAngle());
            }
            else if (
                spawnableStages.Contains(currentStage)
                && aliens.Count == 0
                && !ClockController.Instance.Daytime
                && Time.time - timeOfLastDeath > waveDelay
                && currentWave < wavesPerNight
            )
            {
                loopStopwatch.Restart();

                if (currentStage != EStage.Combat)
                {
                    currentWave++;
                }

                yield return StartCoroutine(SpawnAliens(currentStage));
                MapController.Instance.MajorityAlienSpawnPoints.Clear();
                MapController.Instance.MinorityAlienSpawnPoints.Clear();
            }            

            yield return null;
        }
    }

    /// <summary>
    /// Sorts spawn points by whether they are within the 120 degrees allocated to spawning 70% of aliens, or the 240 degrees for the remaining 30%. Called after spawning all aliens in a wave while waiting for the next wave.
    /// </summary>
    private IEnumerator SortSpawnPointsByAngle()
    {
        minAngle = Random.Range(0, 360);
        maxAngle = MathUtility.Instance.NormaliseAngle(minAngle + angleRange);

        for (int i = 0; i < MapController.Instance.CurrentAlienSpawnPoints.Count; i++)
        {
            Vector3 pos = MapController.Instance.CurrentAlienSpawnPoints[i];
            PositionData posData = MapController.Instance.GetPositionData(pos);

            if (posData != null)
            {
                if (MathUtility.Instance.AngleIsBetween(posData.Angle, minAngle, maxAngle))
                {
                    MapController.Instance.MajorityAlienSpawnPoints.Add(pos);
                }
                else
                {
                    MapController.Instance.MinorityAlienSpawnPoints.Add(pos);
                }
            }

            if (loopStopwatch.ElapsedMilliseconds >= spawningFrameTimeLimit)
            {
                yield return null;
                loopStopwatch.Restart();
            }
        }
    }

    /// <summary>
    /// Spawns new aliens at the start of each wave in a night.
    /// </summary>
    private IEnumerator SpawnAliens(EStage currentStage)
    {
        spawningAliens = true;
        aliensInCurrentWave = CalculateAliensInWave(currentStage);
        majorityCount = 0;
        minorityCount = 0;
        majorityMaxCount = Mathf.RoundToInt(aliensInCurrentWave * 0.7f);
        minorityMaxCount = aliensInCurrentWave - majorityMaxCount;
        float cumulativeCrawlerFrequency = 0;
        List<Vector3> availableSpawnPoints = (currentStage == EStage.Combat ? MapController.Instance.CurrentAlienSpawnPoints : MapController.Instance.MajorityAlienSpawnPoints);
        List<Vector3> minoritySpawnPoints = MapController.Instance.MinorityAlienSpawnPoints;

        for (int i = 0; i < aliensInCurrentWave; i++)
        //for (int i = 0; i < 100; i++)
        {
            if (loopStopwatch.ElapsedMilliseconds >= spawningFrameTimeLimit)
            {
                yield return null;
                loopStopwatch.Restart();
            }

            if (currentStage != EStage.Combat && majorityCount == majorityMaxCount && availableSpawnPoints != minoritySpawnPoints)
            {
                availableSpawnPoints = minoritySpawnPoints;
            }

            Vector3 spawnPos = GetRandomAvailablePosition(availableSpawnPoints);
            float angle = MapController.Instance.GetPositionData(spawnPos).Angle;
            bool positionAvailable;
            Alien alien = SpawnAlien(spawnPos, (cumulativeCrawlerFrequency >= 1 ? EAlien.Crawler : EAlien.Scuttler), out positionAvailable);
            Debug.Log($"spawnPos: {spawnPos}, angle from centre: {angle}, minAngle: {minAngle}, maxAngle: {maxAngle}");

            if (alien != null)
            {
                aliens.Add(alien);

                if (cumulativeCrawlerFrequency >= 1)
                {
                    cumulativeCrawlerFrequency--;
                }

                cumulativeCrawlerFrequency += crawlerFrequency;

                if (currentStage != EStage.Combat)
                {
                    if (majorityCount < majorityMaxCount)
                    {
                        majorityCount++;
                    }
                    else
                    {
                        minorityCount++;
                    }
                }
            }
            else
            {
                i--;

                if (positionAvailable)
                {
                    int margin = (alien.Type == EAlien.Scuttler ? 1 : 2);

                    for (int m = -margin; m <= margin; m++)
                    {
                        for (int n = -margin; n <= margin; n++)
                        {
                            availableSpawnPoints.Remove(new Vector3(spawnPos.x + m, spawnPos.y, spawnPos.z + n));

                            if (loopStopwatch.ElapsedMilliseconds >= spawningFrameTimeLimit)
                            {
                                yield return null;
                                loopStopwatch.Restart();
                            }
                        }
                    }
                }
            }
        }

        spawningAliens = false;
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

    /// <summary>
    /// Spawn a single alien.
    /// </summary>
    /// <param name="spawnPos">The position to spawn the alien at.</param>
    /// <param name="type">The type of alien to spawn.</param>
    /// <param name="positionAvailable">Did MapController.PositionAvailableForSpawning() return true?</param>
    /// <returns></returns>
    public Alien SpawnAlien(Vector3 spawnPos, EAlien type, out bool positionAvailable)
    {
        Alien alien = null;

        if (MapController.Instance.PositionAvailableForSpawning(spawnPos, true))
        {
            positionAvailable = true;
            RaycastHit rayHit;
            NavMeshHit navHit;
            Physics.Raycast(spawnPos, Vector3.down, out rayHit, 25, groundLayerMask);
            alien = AlienFactory.Instance.Get(new Vector3(spawnPos.x, rayHit.point.y, spawnPos.z), type);
            alien.Setup(IdGenerator.Instance.GetNextId());

            if (!NavMesh.SamplePosition(alien.transform.position, out navHit, 1, NavMesh.AllAreas))
            {
                MapController.Instance.RegisterOffMeshPosition(spawnPos);
                AlienFactory.Instance.Destroy(alien, alien.Type);
                alien = null;
            }            
        }
        else
        {
            positionAvailable = false;
        }

        return alien;
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
