using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [SerializeField] private Enemy enemyPrefab;

    //Non-Serialized Fields

    private List<Enemy> enemies;

    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property

    public static EnemyController Instance { get; protected set; }

    //Basic Public Properties

    public List<Enemy> Enemies { get => enemies; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more EnemyControllers.");
        }

        Instance = this;

        enemies = new List<Enemy>();
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

    void Update()
    {
        while (enemies.Count < 4)
        {
            Vector3 spawnPoint;

            switch(Random.Range(0, 4))
            {
                case 0:
                    spawnPoint = new Vector3(Random.Range(-24, 24), 0.5f, Random.Range(-24, -18));
                    break;
                case 1:
                    spawnPoint = new Vector3(Random.Range(-24, 24), 0.5f, Random.Range(18, 24));
                    break;
                case 2:
                    spawnPoint = new Vector3(Random.Range(-24, -18), 0.5f, Random.Range(-24, 24));
                    break;
                case 3:
                    spawnPoint = new Vector3(Random.Range(18, 24), 0.5f, Random.Range(-24, 24));
                    break;
                default:
                    spawnPoint = Vector3.zero;
                    break;
            }

            enemies.Add(Instantiate<Enemy>(enemyPrefab, spawnPoint, new Quaternion()));
        }
    }
}
