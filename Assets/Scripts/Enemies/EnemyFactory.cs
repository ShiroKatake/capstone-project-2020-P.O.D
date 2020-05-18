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

    [Header("Game Objects")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform enemyPoolParent;

    //[Header("Spawning Bounds")]
    //[SerializeField] private Transform outerTopLeft;
    //[SerializeField] private Transform outerBottomRight;
    //[SerializeField] private Transform innerTopLeft;
    //[SerializeField] private Transform innerBottomRight;
    ////TODO: remove transforms, and get map size and enemy spawn bounds from MapController, then use that information to calculate other values.

    //Non-Serialized Fields------------------------------------------------------------------------

    private List<Enemy> enemyPool;

    //float outerXMin;
    //float outerXMax;
    //float outerZMin;
    //float outerZMax;
    //float innerXMin;
    //float innerXMax;
    //float innerZMin;
    //float innerZMax;

    //float accumulativeAreaTopLeft;
    //float accumulativeAreaTopCentre;
    //float accumulativeAreaTopRight;
    //float accumulativeAreaCentreLeft;
    //float accumulativeAreaCentreRight;
    //float accumulativeAreaBottomLeft;
    //float accumulativeAreaBottomCentre;
    //float accumulativeAreaBottomRight;

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

        //outerXMin = outerTopLeft.position.x;
        //outerXMax = outerBottomRight.position.x;
        //outerZMin = outerBottomRight.position.z;
        //outerZMax = outerTopLeft.position.z;
        //innerXMin = innerTopLeft.position.x;
        //innerXMax = innerBottomRight.position.x;
        //innerZMin = innerBottomRight.position.z;
        //innerZMax = innerTopLeft.position.z;
        ////Debug.Log($"outerXMin: {outerXMin}, outerXMax: {outerXMax}");
        ////Debug.Log($"outerZMin: {outerZMin}, outerZMax: {outerZMax}");
        ////Debug.Log($"innerXMin: {innerXMin}, innerXMax: {innerXMax}");
        ////Debug.Log($"innerZMin: {innerZMin}, innerZMax: {innerZMax}");

        //float leftXLength = innerXMin - outerXMin;
        //float centreXLength = innerXMax - innerXMin;
        //float rightXLength = outerXMax - innerXMax;
        //float bottomZLength = innerZMin - outerZMin;
        //float centreZLength = innerZMax - innerZMin;
        //float topZLength = outerZMax - innerZMax;

        //accumulativeAreaTopLeft = leftXLength * topZLength;
        //accumulativeAreaTopCentre = centreXLength * topZLength + accumulativeAreaTopLeft;
        //accumulativeAreaTopRight = rightXLength * topZLength + accumulativeAreaTopCentre;
        //accumulativeAreaCentreLeft = leftXLength * centreZLength + accumulativeAreaTopRight;
        //accumulativeAreaCentreRight = rightXLength * centreZLength + accumulativeAreaCentreLeft;
        //accumulativeAreaBottomLeft = leftXLength * bottomZLength + accumulativeAreaCentreRight;
        //accumulativeAreaBottomCentre = centreXLength * bottomZLength + accumulativeAreaBottomLeft;
        //accumulativeAreaBottomRight = rightXLength * bottomZLength + accumulativeAreaBottomCentre;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves Enemies from a pool if there's any available, and instantiates a new Enemy if there isn't one. Provides a random position within the accepted bounds.
    /// </summary>
    /// <returns>A new enemy.</returns>
    public Enemy GetEnemy()
    {
        //float selection = Random.Range(0, accumulativeAreaBottomRight);

        //if (selection <= accumulativeAreaTopLeft)
        //{
        //    return GetEnemy(new Vector3(Mathf.Round(Random.Range(outerXMin, innerXMin)), 0.25f, Mathf.Round(Random.Range(innerZMax, outerZMax))));
        //}
        //else if (selection <= accumulativeAreaTopCentre)
        //{
        //    return GetEnemy(new Vector3(Mathf.Round(Random.Range(innerXMin, innerXMax)), 0.25f, Mathf.Round(Random.Range(innerZMax, outerZMax))));
        //}
        //else if (selection <= accumulativeAreaTopRight)
        //{
        //    return GetEnemy(new Vector3(Mathf.Round(Random.Range(innerXMax, outerXMax)), 0.25f, Mathf.Round(Random.Range(innerZMax, outerZMax))));
        //}
        //else if (selection <= accumulativeAreaCentreLeft)
        //{
        //    return GetEnemy(new Vector3(Mathf.Round(Random.Range(outerXMin, innerXMin)), 0.25f, Mathf.Round(Random.Range(innerZMin, innerZMax))));
        //}
        //else if (selection <= accumulativeAreaCentreRight)
        //{
        //    return GetEnemy(new Vector3(Mathf.Round(Random.Range(innerXMax, outerXMax)), 0.25f, Mathf.Round(Random.Range(innerZMin, innerZMax))));
        //}
        //else if (selection <= accumulativeAreaBottomLeft)
        //{
        //    return GetEnemy(new Vector3(Mathf.Round(Random.Range(outerXMin, innerXMin)), 0.25f, Mathf.Round(Random.Range(outerZMin, innerZMin))));
        //}
        //else if (selection <= accumulativeAreaBottomCentre)
        //{
        //    return GetEnemy(new Vector3(Mathf.Round(Random.Range(innerXMin, innerXMax)), 0.25f, Mathf.Round(Random.Range(outerZMin, innerZMin))));
        //}
        //else
        //{
        //    return GetEnemy(new Vector3(Mathf.Round(Random.Range(innerXMax, outerXMax)), 0.25f, Mathf.Round(Random.Range(outerZMin, innerZMin))));
        //}
        Vector2 pos2 = MapController.Instance.RandomEnemySpawnablePos();
        Vector3 pos3 = new Vector3(pos2.x, 0.25f, pos2.y);
        return GetEnemy(pos3);
    }

    /// <summary>
    /// Retrieves Enemies from a pool if there's any available, and instantiates a new Enemy if there isn't one.
    /// </summary>
    /// <param name="position">The position the Enemy should be instantiated at.</param>
    /// <returns>A new enemy.</returns>
    public Enemy GetEnemy(Vector3 position)
    {
        Enemy enemy;        

        if (enemyPool.Count > 0)
        {
            enemy = enemyPool[0];
            enemyPool.Remove(enemy);
            enemy.transform.parent = null;
            enemy.transform.position = position;
        }
        else
        {
            enemy = Instantiate(enemyPrefab, position, new Quaternion());
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
