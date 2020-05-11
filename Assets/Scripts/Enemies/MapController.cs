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
    [SerializeField] private Vector3 innerTopLeft;
    [SerializeField] private Vector3 innerBottomRight;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private bool[,] availablePositions;
    private bool[,] enemySpawnablePositions;

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
        availablePositions = new bool[xMax + 1 , zMax + 1];
        enemySpawnablePositions = new bool[xMax + 1, zMax + 1];

        int noEnemyXMin = (int)Mathf.Round(innerTopLeft.x);
        int noEnemyXMax = (int)Mathf.Round(innerBottomRight.x);
        int noEnemyZMin = (int)Mathf.Round(innerBottomRight.z);
        int noEnemyZMax = (int)Mathf.Round(innerTopLeft.z);

        Debug.Log($"Enemies cannot spawn within ({noEnemyXMin}, {noEnemyZMin}) to ({noEnemyXMax}, {noEnemyZMax})");

        for (int i = 0; i < xMax; i++)
        {
            for (int j = 0; j < zMax; j++)
            {
                availablePositions[i, j] = true;
                enemySpawnablePositions[i, j] = (i < noEnemyXMin && i > noEnemyXMax && j < noEnemyZMin && j > noEnemyZMax);
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //TODO: have buildings make use of PositionAvailableforBuilding() to make sure they're spawning within the bounds of the map
    //TODO: set xMax and zMax in the inspector both to 201, so that map spans (0,0) to (200, 200); 201 accounts for starting at 0, not 1.
    //TODO: make sure other values are set properly in the inspector for this.
    public bool PositionAvailableForBuilding(Building building)
    {
        Vector3 buildingPos = building.transform.position;
        Debug.Log($"Verifying for building at {buildingPos}");

        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            if (!PositionAvailableForSpawning(buildingPos + offset))
            {
                return false;
            }
        }

        return true;
    }

    //TODO: triple-slash comments
    //TODO: have enemies make use of PositionAvailableForSpawning()
    public bool PositionAvailableForSpawning(Vector3 position)
    {
        Debug.Log($"Verifying for spawnable at {position}");
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);

        if (position.x < 0 || position.x > xMax || position.z < 0 || position.z > zMax)
        {
            Debug.Log($"Can't spawn at {position}, which is outside the bounds of (0,0) to ({xMax},{zMax})");
            return false;
        }

        if (!availablePositions[(int)position.x, (int)position.z])
        {
            Debug.Log($"Can't spawn at {position}, which is already occupied.");
            return false;
        }

        return true;
    }

    public void RegisterBuilding(Building building)
    {
        UpdateAvailablePositions(building, true);
    }

    public void DeRegisterBuilding(Building building)
    {
        UpdateAvailablePositions(building, false);
    }

    private void UpdateAvailablePositions(Building building, bool available)
    {
        Vector3 buildingPos = building.transform.position;
        Debug.Log($"Updating availability of positions for building at {buildingPos}");

        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            Vector3 foundationPos = buildingPos + offset;
            foundationPos.x = Mathf.Round(foundationPos.x);
            foundationPos.z = Mathf.Round(foundationPos.z);

            if (foundationPos.x >= 0 || foundationPos.x <= xMax || foundationPos.z >= 0 || foundationPos.z <= zMax)
            {
                availablePositions[(int)foundationPos.x, (int)foundationPos.z] = available;
            }
            else
            {
                Debug.Log($"{building.gameObject.name} can't update the availability of position {foundationPos}, which is outside the bounds of (0,0) to ({xMax},{zMax})");
            }            
        }
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
