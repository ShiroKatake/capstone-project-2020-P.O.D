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
    [SerializeField] private int maxClusterRadius;
    [SerializeField] private float respawnDelay;

    [Header("Penalty Stats")]
    [SerializeField] private float defencePenaltyThreshold;
    [SerializeField] private float nonDefencePenaltyThreshold;
    [SerializeField] private int penaltyIncrement;
    [SerializeField] private float penaltyCooldown;

    [Header("For Testing")]
    [SerializeField] private bool spawnAliens;
    //[SerializeField] private bool spawnAlienNow;
    //[SerializeField] private Vector3 testSpawnPos;
    [SerializeField] private bool ignoreDayNightCycle;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Alien Spawning
    private List<Alien> aliens;
    private float timeOfLastDeath;
    private Dictionary<int, List<Vector3>> clusterOffsets;

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

        //Setting up position offsets that can be randomly selected from for cluster spawning 
        clusterOffsets = new Dictionary<int, List<Vector3>>();

        for (int i = 0; i <= maxClusterRadius; i++)
        {
            clusterOffsets[i] = new List<Vector3>();
        }

        for (int i = maxClusterRadius * -1; i <= maxClusterRadius; i++)
        {
            for (int j = maxClusterRadius * -1; j <= maxClusterRadius; j++)
            {
                int iMag = MathUtility.Instance.IntMagnitude(i);
                int jMag = MathUtility.Instance.IntMagnitude(j);
                Vector3 pos = new Vector3(i, 0, j);

                foreach (KeyValuePair<int, List<Vector3>> p in clusterOffsets)
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
    /// Spawns more Enemies if there are less than 4 in the scene.
    /// </summary>
    private void SpawnAliens()
    {
        //if (spawnAlienNow)
        //{
        //    aliens.Add(AlienFactory.Instance.GetAlien(testSpawnPos));
        //    spawnAlienNow = false;
        //}

        if (spawnAliens)
        {
            if (ignoreDayNightCycle)
            {
                while (aliens.Count < 4)
                {
                    aliens.Add(AlienFactory.Instance.GetAlien());
                }
            }
            else
            {
                if (!ClockController.Instance.Daytime && aliens.Count == 0 && Time.time - timeOfLastDeath > respawnDelay)
                {
                    //Debug.Log("Nighttime? No enemies? Spawning time!");

                    //Check and increment penalty
                    if (Time.time - timeOfLastPenalty > penaltyCooldown && (Time.time - BuildingController.Instance.TimeLastDefenceWasBuilt > defencePenaltyThreshold || Time.time - BuildingController.Instance.TimeLastNonDefenceWasBuilt > nonDefencePenaltyThreshold))
                    {
                        spawnCountPenalty += penaltyIncrement;
                        timeOfLastPenalty = Time.time;
                        Debug.Log($"AlienController.spawnCountPenalty incremented to {spawnCountPenalty}");
                    }

                    //Spawn enemies
                    int spawnCount = BuildingController.Instance.BuildingCount * 3 + spawnCountPenalty;
                    Vector3 clusterCentre = MapController.Instance.RandomAlienSpawnablePos();                    
                    int clusterRadius = 0;
                    List<Vector3> availableOffsets = new List<Vector3>();

                    for (int i = 0; i < spawnCount; i++)
                    {
                        if (availableOffsets.Count == 0)
                        {
                            if (clusterRadius >= clusterOffsets.Count)
                            {
                                return;
                            }

                            availableOffsets.AddRange(clusterOffsets[clusterRadius]);
                            clusterRadius++;
                        }

                        int j = Random.Range(0, availableOffsets.Count);
                        Vector3 spawnPos = clusterCentre + availableOffsets[j] * 2;
                        availableOffsets.RemoveAt(j);

                        if (MapController.Instance.PositionAvailableForSpawning(spawnPos, true))
                        {
                            aliens.Add(AlienFactory.Instance.GetAlien(spawnPos));
                        }
                        else
                        {
                            i--;
                        }                        
                    }
                }
            }
        }
    }

    /// <summary>
    /// Removes the alien from AlienController's list of enemies.
    /// </summary>
    /// <param name="alien">The alien to be removed from AlienController's list of enemies.</param>
    public void DeRegisterAlien(Alien alien)
    {
        if (aliens.Contains(alien))
        {
            aliens.Remove(alien);
            timeOfLastDeath = Time.time;
        }
    }
}
