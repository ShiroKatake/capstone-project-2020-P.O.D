using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private int maxSwarmRadius;
    [SerializeField] private int maxSwarmSize;
    [SerializeField] private int maxSwarmCount;

    [Header("Penalty Stats")]
    [SerializeField] private float defencePenaltyThreshold;
    [SerializeField] private float nonDefencePenaltyThreshold;
    [SerializeField] private int penaltyIncrement;
    [SerializeField] private float penaltyCooldown;

    [Header("For Testing")]
    [SerializeField] private bool spawnAliens;
    [SerializeField] private bool spawnAlienNow;
    //[SerializeField] private Vector3 testSpawnPos;
    [SerializeField] private bool ignoreDayNightCycle;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Alien Spawning
    private List<Alien> aliens;
    private float timeOfLastDeath;
    private Dictionary<int, List<Vector3>> swarmOffsets;
    private LayerMask groundLayerMask;

    //Penalty Incrementation
    private int spawnCountPenalty;
    private float timeOfLastPenalty;

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
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more AlienControllers.");
        }

        Instance = this;
        aliens = new List<Alien>();
        timeOfLastDeath = respawnDelay * -1;
        timeOfLastPenalty = penaltyCooldown * -1;
        spawnCountPenalty = 0;
        groundLayerMask = LayerMask.GetMask("Ground");

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

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    void Update()
    {
        SpawnAliens();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Spawns more aliens on a regular basis.
    /// </summary>
    private void SpawnAliens()
    {
        if (spawnAliens && (spawnAlienNow || (!ClockController.Instance.Daytime && aliens.Count == 0 && Time.time - timeOfLastDeath > respawnDelay)))
        {
            //Debug.Log("Nighttime? No aliens? Spawning time!");
            if (spawnAlienNow)
            {
                //Debug.Log("Test position");
                //MapController.Instance.PositionAvailableForSpawning(new Vector3(10, 5, 10), true);
                spawnAlienNow = false;
            }

            //Check and increment penalty
            if (Time.time - timeOfLastPenalty > penaltyCooldown && (Time.time - BuildingController.Instance.TimeLastDefenceWasBuilt > defencePenaltyThreshold || Time.time - BuildingController.Instance.TimeLastNonDefenceWasBuilt > nonDefencePenaltyThreshold))
            {
                spawnCountPenalty += penaltyIncrement;
                timeOfLastPenalty = Time.time;
                Debug.Log($"AlienController.spawnCountPenalty incremented to {spawnCountPenalty}");
            }

            //Spawn aliens
            int spawnCount = BuildingController.Instance.BuildingCount * 3 + spawnCountPenalty;
            Vector3 swarmCentre = Vector3.zero; 
            int swarmSize = 0;                   
            int swarmRadius = 0;
            int swarmCount = 0;
            float offsetMultiplier = 2;
            List<Vector3> availableOffsets = new List<Vector3>();
            Dictionary<Vector3, bool> unavailablePositions = new Dictionary<Vector3, bool>();

            for (int i = 0; i < spawnCount; i++)
            //for (int i = 0; i < 100; i++)
            {
                if (availableOffsets.Count == 0)
                {
                    if (swarmRadius >= swarmOffsets.Count || swarmSize >= maxSwarmSize)
                    {
                        swarmCount++;

                        if (swarmCount >= maxSwarmCount)
                        {
                            return;
                        }

                        swarmCentre = MapController.Instance.RandomAlienSpawnablePos(new List<Vector3>(unavailablePositions.Keys));
                        swarmRadius = 0;
                        swarmSize = 0;
                    }

                    availableOffsets.AddRange(swarmOffsets[swarmRadius]);
                    swarmRadius++;
                }

                int j = Random.Range(0, availableOffsets.Count);
                Vector3 spawnPos = swarmCentre + availableOffsets[j] * offsetMultiplier;
                availableOffsets.RemoveAt(j);

                if (MapController.Instance.PositionAvailableForSpawning(spawnPos, true))
                {
                    RaycastHit hit;
                    Physics.Raycast(spawnPos, Vector3.down, out hit, 25, groundLayerMask);
                    Alien alien = AlienFactory.Instance.GetAlien(new Vector3(spawnPos.x, hit.point.y/* + 0.1f*/, spawnPos.z));
                    alien.Setup(IdGenerator.Instance.GetNextId());
                    aliens.Add(alien);
                    swarmSize++;

                    //Debug.Log($"Spawning at ({spawnPos.x}, {hit.point.y/* + 0.1f*/}, {spawnPos.z})");

                    int maxLeft = (int)(maxSwarmRadius * offsetMultiplier * -1);
                    int maxRight = Mathf.CeilToInt(maxSwarmRadius * offsetMultiplier);

                    for (int m = maxLeft; m <= maxRight; m++)
                    {
                        for (int n = maxLeft; n <= maxRight; n++)
                        {
                            Vector3 q = new Vector3(spawnPos.x + m, spawnPos.y, spawnPos.z + n);
                            unavailablePositions[q] = true;
                        }
                    }                          
                }
                else
                {
                    i--;
                }                        
            }
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
