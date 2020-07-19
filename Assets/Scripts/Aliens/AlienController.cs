using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controller class for aliens.
/// </summary>
public class AlienController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Spawning Stats")]
    [SerializeField] private float respawnDelay;

    [Header("Swarm Stats")]
    [SerializeField] private Vector3 tutorialSwarmCentre;
    [SerializeField] private int maxSwarmRadius;
    [SerializeField] private int maxSwarmSize;
    [SerializeField] private int maxSwarmCount;

    [Header("Penalty Stats")]
    [SerializeField] private float defencePenaltyThreshold;
    [SerializeField] private float nonDefencePenaltyThreshold;
    [SerializeField] private int penaltyIncrement;
    [SerializeField] private float penaltyCooldown;

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
    System.Diagnostics.Stopwatch methodStopwatch;
    long spawnAliensTickBudget;

    //Penalty Incrementation
    private int spawnCountPenalty;
    private float timeOfLastPenalty;

    private int yieldPerFrame;

    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// AlienController's singleton public property.
    /// </summary>
    public static AlienController Instance { get; protected set; }

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
        if (Instance != null)
        {
            UnityEngine.Debug.LogError("There should never be 2 or more AlienControllers.");
        }

        Instance = this;
        aliens = new List<Alien>();
        timeOfLastDeath = respawnDelay * -1;
        timeOfLastPenalty = penaltyCooldown * -1;
        spawnCountPenalty = 0;
        groundLayerMask = LayerMask.GetMask("Ground");
        spawnableStages = new List<EStage>() { EStage.Combat, EStage.MainGame };
        gameOverStages = new List<EStage>() { EStage.Win, EStage.Lose };

        //Setting up spawning optimisation variables
        loopStopwatch = new System.Diagnostics.Stopwatch();
        methodStopwatch = new System.Diagnostics.Stopwatch();
        //spawnAliensTickBudget = (long)(System.Diagnostics.Stopwatch.Frequency * spawningFrameTimeLimit / 1000f);

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

        yieldPerFrame = 0;
    }


    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Start()
    {
        StartCoroutine(SpawnAliens());
    }

    private void Update()
    {
        Debug.Log($"Yields last frame: {yieldPerFrame}");
        yieldPerFrame = 0;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Spawns more aliens on a regular basis.
    /// </summary>
    private IEnumerator SpawnAliens()
    {
        Debug.Log("Starting coroutine SpawnAliens");

        while (StageManager.Instance == null || StageManager.Instance.CurrentStage == null)
        {
            yieldPerFrame++;
            yield return null;
        }

        while (spawnAliens && !gameOverStages.Contains(StageManager.Instance.CurrentStage.ID))
        {
            if (spawnableStages.Contains(StageManager.Instance.CurrentStage.ID) 
                && !ClockController.Instance.Daytime 
                && aliens.Count == 0 
                && Time.time - timeOfLastDeath > respawnDelay)
            {
                Debug.Log("Starting Spawning Aliens");
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
                int spawnCount = (currentStage == EStage.Combat ? 3 : BuildingController.Instance.BuildingCount * 3 + spawnCountPenalty);
                Vector3 swarmCentre = Vector3.zero;
                int swarmSize = 0;
                int swarmRadius = 0;
                int swarmCount = 0;
                float offsetMultiplier = 2;
                List<Vector3> availableOffsets = new List<Vector3>();
                List<Vector3> availablePositions = MapController.Instance.GetAlienSpawnablePositions();
                //Dictionary<Vector3, bool> unavailablePositions = new Dictionary<Vector3, bool>();

                //for (int i = 0; i < spawnCount; i++)
                for (int i = 0; i < 100; i++)
                {
                    if (loopStopwatch.ElapsedMilliseconds >= spawningFrameTimeLimit)
                    {
                        Debug.Log($"Spawning time limit per frame: {spawnAliensTickBudget}, elapsed milliseconds: {loopStopwatch.ElapsedMilliseconds}, yielding for this frame from start of spawning main loop");
                        yieldPerFrame++;
                        yield return null;
                        loopStopwatch.Restart();
                    }

                    if (availableOffsets.Count == 0)
                    {
                        if (swarmSize >= maxSwarmSize || swarmRadius >= swarmOffsets.Count)
                        {
                            swarmCount++;

                            if (swarmCount >= maxSwarmCount)
                            {
                                break;
                            }

                            methodStopwatch.Restart();
                            //swarmCentre = MapController.Instance.RandomAlienSpawnablePos(new List<Vector3>(unavailablePositions.Keys));     //RandomAlienSpawnablePos() checks the stage before selecting its list of normally available positions
                            swarmCentre = GetRandomPosition(availablePositions);
                            Debug.Log($"Method stop watch timing MapController.Instance.RandomAlienSpawnablePos(), elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");
                            swarmRadius = 0;
                            swarmSize = 0;
                        }

                        availableOffsets.AddRange(swarmOffsets[swarmRadius]);
                        swarmRadius++;
                    }

                    int j = Random.Range(0, availableOffsets.Count);
                    Vector3 spawnPos = swarmCentre + availableOffsets[j] * offsetMultiplier;
                    availableOffsets.RemoveAt(j);

                    methodStopwatch.Restart();
                    if (MapController.Instance.PositionAvailableForSpawning(spawnPos, true) || currentStage == EStage.Combat)
                    {
                        Debug.Log($"Method stop watch timing MapController.Instance.PositionAvailableForSpawning() on returns true or bypassed, elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");
                        RaycastHit rayHit;
                        NavMeshHit navHit;

                        methodStopwatch.Restart();
                        Physics.Raycast(spawnPos, Vector3.down, out rayHit, 25, groundLayerMask);
                        Debug.Log($"Method stop watch timing Physics.Raycast(), elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");

                        methodStopwatch.Restart();
                        Alien alien = AlienFactory.Instance.GetAlien(new Vector3(spawnPos.x, rayHit.point.y, spawnPos.z));
                        Debug.Log($"Method stop watch timing AlienFactory.Instance.GetAlien(), elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");

                        methodStopwatch.Restart();
                        alien.Setup(IdGenerator.Instance.GetNextId());
                        Debug.Log($"Method stop watch timing Alien.Setup(), elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");

                        methodStopwatch.Restart();
                        if (NavMesh.SamplePosition(alien.transform.position, out navHit, 1, NavMesh.AllAreas))
                        {
                            Debug.Log($"Method stop watch timing NavMesh.SamplePosition() on returned true, elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");
                            aliens.Add(alien);
                            swarmSize++;
                        }
                        else
                        {
                            Debug.Log($"Method stop watch timing NavMesh.SamplePosition() on returned false, elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");

                            methodStopwatch.Restart();
                            MapController.Instance.RegisterOffMeshPosition(spawnPos);
                            Debug.Log($"Method stop watch timing MapController.Instance.RegisterOffMeshPosition(), elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");

                            AlienFactory.Instance.DestroyAlien(alien);
                            i--;
                        }

                        //int maxLeft = (int)(maxSwarmRadius * offsetMultiplier * -1);
                        //int maxRight = Mathf.CeilToInt(maxSwarmRadius * offsetMultiplier);
                        int count = 0;

                        methodStopwatch.Restart();
                        for (int m = -1; m <= 1; m++)
                        {
                            for (int n = -1; n <= 1; n++)
                            {
                                count++;
                                availablePositions.Remove(new Vector3(spawnPos.x + m, spawnPos.y, spawnPos.z + n));

                                if (loopStopwatch.ElapsedMilliseconds >= spawningFrameTimeLimit)
                                {
                                    Debug.Log($"Spawning time limit per frame: {spawnAliensTickBudget}, elapsed milliseconds: {loopStopwatch.ElapsedMilliseconds}, yielding for this frame from removing unavailable positions loop");
                                    yieldPerFrame++;
                                    yield return null;
                                    loopStopwatch.Restart();
                                }
                            }
                        }

                        //int maxLeft = (int)(maxSwarmRadius * offsetMultiplier * -1);
                        //int maxRight = Mathf.CeilToInt(maxSwarmRadius * offsetMultiplier);
                        //int count = 0;

                        //methodStopwatch.Restart();
                        //for (int m = maxLeft; m <= maxRight; m++)
                        //{
                        //    for (int n = maxLeft; n <= maxRight; n++)
                        //    {
                        //        count++;
                        //        availablePositions.Remove(new Vector3(spawnPos.x + m, spawnPos.y, spawnPos.z + n));
                        //    }
                        //}

                        Debug.Log($"Method stop watch timing availablePositions.Remove() x{count}, elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");
                    }
                    else
                    {
                        Debug.Log($"Method stop watch timing MapController.Instance.RandomAlienSpawnablePos() on returns false and not bypassed, elapsed milliseconds: {methodStopwatch.ElapsedMilliseconds}");
                        i--;
                    }
                }

                Debug.LogError("Finished Spawning Aliens");
            }

            yieldPerFrame++;
            yield return null;
        }

        Debug.Log("Finished coroutine SpawnAliens");
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
