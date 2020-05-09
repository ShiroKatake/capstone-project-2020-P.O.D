using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller class for Enemy.
/// </summary>
public class EnemyController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Enemy Stats")]
    [SerializeField] private float respawnDelay;

    [Header("For Testing")]
    [SerializeField] private bool spawnEnemies;
    [SerializeField] private bool ignoreDayNightCycle;

    //Non-Serialized Fields------------------------------------------------------------------------

    private List<Enemy> enemies;
    private float timeOfLastDeath;

    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// EnemyController's singleton public property.
    /// </summary>
    public static EnemyController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// A list of all Enemies
    /// </summary>
    public List<Enemy> Enemies { get => enemies; }

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
            Debug.LogError("There should never be 2 or more EnemyControllers.");
        }

        Instance = this;
        enemies = new List<Enemy>();
        timeOfLastDeath = respawnDelay * -1;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    void Update()
    {
        SpawnEnemies();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Spawns more Enemies if there are less than 4 in the scene.
    /// </summary>
    private void SpawnEnemies()
    {
        if (spawnEnemies)
        {
            if (ignoreDayNightCycle)
            {
                while (enemies.Count < 4)
                {
                    enemies.Add(EnemyFactory.Instance.GetEnemy());
                }
            }
            else
            {
                if (!DayNightCycleController.Instance.Daytime && enemies.Count == 0 && Time.time - timeOfLastDeath > respawnDelay)
                {
                    Debug.Log("Nighttime? No enemies? Spawning time!");
                    int spawnCount = 4; //TODO: derive spawn count from number of buildings and delay since last defence/non-defence built.

                    //TODO: spawning in a cluster without overlapping

                    for (int i = 0; i < spawnCount; i++)
                    {
                        enemies.Add(EnemyFactory.Instance.GetEnemy());
                    }
                }
            }
        }
    }

    /// <summary>
    /// Removes the enemy from EnemyController's list of enemies.
    /// </summary>
    /// <param name="enemy">The enemy to be removed from EnemyController's list of enemies.</param>
    public void DeRegisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            timeOfLastDeath = Time.time;
        }
    }
}
