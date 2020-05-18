using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A controller class for tracking which parts of the map have buildings, can be spawned to by aliens, etc.
/// </summary>
public class MapController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Map Size")]
    [SerializeField] private int xMax;
    [SerializeField] private int zMax;

    [Header("No Alien Spawning Area")]
    [SerializeField] private Vector3 innerBottomLeft;
    [SerializeField] private Vector3 innerTopRight;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private bool[,] availableBuildingPositions;
    private bool[,] enemyExclusionArea;
    private bool[,] availableEnemyPositions;

    [SerializeField] private List<Vector3> enemySpawnablePositions;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// MapController's singleton public property.
    /// </summary>
    public static MapController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



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
            Debug.LogError("There should never be more than one MapController.");
        }

        Instance = this;
        availableBuildingPositions = new bool[xMax + 1 , zMax + 1];
        availableEnemyPositions = new bool[xMax + 1, zMax + 1];
        enemyExclusionArea = new bool[xMax + 1, zMax + 1];
        enemySpawnablePositions = new List<Vector3>();

        int noEnemyXMin = (int)Mathf.Round(innerBottomLeft.x);
        int noEnemyXMax = (int)Mathf.Round(innerTopRight.x);
        int noEnemyZMin = (int)Mathf.Round(innerBottomLeft.z);
        int noEnemyZMax = (int)Mathf.Round(innerTopRight.z);

        //Debug.Log($"Enemies cannot spawn within ({noEnemyXMin}, {noEnemyZMin}) to ({noEnemyXMax}, {noEnemyZMax})");

        for (int i = 0; i <= xMax; i++)
        {
            for (int j = 0; j <= zMax; j++)
            {
                //Debug.Log($"Assessing position ({i},{j})");
                availableBuildingPositions[i, j] = true;
                availableEnemyPositions[i, j] = ((i < noEnemyXMin || i > noEnemyXMax) && (j < noEnemyZMin || j > noEnemyZMax));
                enemyExclusionArea[i, j] = !availableEnemyPositions[i, j];

                //Debug.Log($"available for building: {availableBuildingPositions[i, j]}, available for enemies: {availableEnemyPositions[i, j]}, enemy exclusion area: {enemyExclusionArea[i, j]}");

                if (availableEnemyPositions[i, j])
                {
                    enemySpawnablePositions.Add(new Vector3(i, 0.25f, j));
                }
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    public bool PositionAvailableForBuilding(Building building)
    {
        Vector3 buildingPos = building.transform.position;
        //Debug.Log($"Verifying for building at {buildingPos}");

        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            if (!PositionAvailableForSpawning(buildingPos + offset, false))
            {
                return false;
            }
        }

        return true;
    }

    //TODO: triple-slash comments
    public bool PositionAvailableForSpawning(Vector3 position, bool enemy)
    {
        //Debug.Log($"Verifying for spawnable at {position}");
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);

        if (position.x < 0 || position.x > xMax || position.z < 0 || position.z > zMax)
        {
            Debug.Log($"Can't spawn at {position}, which is outside the bounds of (0,0) to ({xMax},{zMax})");
            return false;
        }

        if (enemy && enemyExclusionArea[(int)position.x, (int)position.z])
        {
            Debug.Log($"Can't spawn an enemy at {position}, which is within the enemy exclusion area.");
        }

        if (!availableBuildingPositions[(int)position.x, (int)position.z])
        {
            Debug.Log($"Can't spawn at {position}, which is already occupied by a building.");
            return false;
        }

        return true;
    }

    public Vector2 RandomEnemySpawnablePos()
    {
        switch (enemySpawnablePositions.Count)
        {
            case 0:
                return new Vector2 (-1, -1);
            case 1:
                return enemySpawnablePositions[0];
            default:
                return enemySpawnablePositions[Random.Range(0, enemySpawnablePositions.Count)];
        }
    }

    public void RegisterBuilding(Building building)
    {
        UpdateAvailablePositions(building, false);
    }

    public void DeRegisterBuilding(Building building)
    {
        UpdateAvailablePositions(building, true);
    }

    private void UpdateAvailablePositions(Building building, bool available)
    {
        Vector3 buildingPos = building.transform.position;
        //Debug.Log($"Updating availability of positions for building at {buildingPos}");

        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            Vector3 foundationPos = buildingPos + offset;
            int x = (int)Mathf.Round(foundationPos.x);
            int z = (int)Mathf.Round(foundationPos.z);

            if (x >= 0 || x <= xMax || z >= 0 || z <= zMax)
            {                
                bool startingEnemyAvailability = availableEnemyPositions[x, z];
                availableBuildingPositions[x, z] = available;
                availableEnemyPositions[x, z] = (availableBuildingPositions[x, z] && !enemyExclusionArea[x, z]);

                if (availableEnemyPositions[x, z] != startingEnemyAvailability)
                {
                    Vector3 pos = new Vector3(x, 0.25f, z);

                    if (availableEnemyPositions[x, z])
                    {
                        enemySpawnablePositions.Add(pos);
                    }
                    else
                    {
                        enemySpawnablePositions.Remove(pos);
                    }
                }
            }
            else
            {
                Debug.Log($"{building.gameObject.name} can't update the availability of position {foundationPos}, which is outside the bounds of (0,0) to ({xMax},{zMax})");
            }            
        }
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
