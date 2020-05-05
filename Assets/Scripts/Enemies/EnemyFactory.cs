using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class for Enemy.
/// </summary>
public class EnemyFactory : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform enemyPoolParent;

    //Non-Serialized Fields------------------------------------------------------------------------

    private List<Enemy> enemyPool;

    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// EnemyController's singleton public property.
    /// </summary>
    public static EnemyFactory Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more EnemyFactories.");
        }

        Instance = this;
        enemyPool = new List<Enemy>();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves Enemies from a pool if there's any available, and instantiates a new Enemy if there isn't one.
    /// </summary>
    /// <param name="position">The position the Enemy should be instantiated at.</param>
    /// <returns></returns>
    public Enemy GetEnemy()
    {
        Enemy enemy;
        Vector3 spawnPoint;

        switch (Random.Range(0, 4))
        {
            case 0:
                spawnPoint = new Vector3(Random.Range(-24, 24), 0.25f, Random.Range(-24, -18));
                break;
            case 1:
                spawnPoint = new Vector3(Random.Range(-24, 24), 0.25f, Random.Range(18, 24));
                break;
            case 2:
                spawnPoint = new Vector3(Random.Range(-24, -18), 0.25f, Random.Range(-24, 24));
                break;
            case 3:
                spawnPoint = new Vector3(Random.Range(18, 24), 0.25f, Random.Range(-24, 24));
                break;
            default:
                spawnPoint = Vector3.zero;
                break;
        }

        if (enemyPool.Count > 0)
        {
            enemy = enemyPool[0];
            enemyPool.Remove(enemy);
            enemy.transform.parent = null;
            enemy.transform.position = spawnPoint;
        }
        else
        {
            enemy = Instantiate<Enemy>(enemyPrefab, spawnPoint, new Quaternion());
        }

        enemy.Setup(IdGenerator.Instance.GetNextId());
        enemy.Moving = true;
        return enemy;
    }

    /// <summary>
    /// Handles the destruction of Enemies.
    /// </summary>
    /// <param name="enemy">The enemy to be destroyed.</param>
    public void DestroyEnemy(Enemy enemy)
    {
        EnemyController.Instance.DeRegisterEnemy(enemy);
        enemyPool.Add(enemy);
        enemy.Moving = false;
        enemy.transform.position = enemyPoolParent.position;
        enemy.transform.parent = enemyPoolParent;
    }
}
