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



    //Non-Serialized Fields------------------------------------------------------------------------

    private List<Enemy> enemies;

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
        while (enemies.Count < 4)
        {
            enemies.Add(EnemyFactory.Instance.GetEnemy());
        }
    }

    public void DeRegisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
}
