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
    [SerializeField] private Vector3 centre;
    [SerializeField] private Vector3 outerTopLeft;
    [SerializeField] private Vector3 outerBottomRight;

    [Header("No Alien Spawning Area")]
    [SerializeField] private Vector3 innerTopLeft;
    [SerializeField] private Vector3 innerBottomRight;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private bool[,] positions;
    private bool[,] enemySpawnablePositions;

    [SerializeField] private float xOffset;
    [SerializeField] private float zOffset;

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

        xOffset = outerTopLeft.x;
        zOffset = outerBottomRight.z;

        int xMax = (int)Mathf.Round(outerBottomRight.x - xOffset);
        int zMax = (int)Mathf.Round(outerTopLeft.z - zOffset);

        positions = new bool[xMax , zMax];
        enemySpawnablePositions = new bool[xMax , zMax];

        Debug.Log($"Map ranges from {outerTopLeft} to {outerBottomRight}. Offset is ({xOffset}, {zOffset}. If start at 0, max is at ({xMax}, {zMax})");
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    public bool PositionAvailableForBuilding(Building building)
    {
        Vector3 buildingPos = building.transform.position;
        Debug.Log($"Verifying for building at {buildingPos}");

        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            Vector2 position = new Vector2(buildingPos.x + offset.x - xOffset, buildingPos.z + offset.z - zOffset);
            bool result;

            try
            {
                Debug.Log($"Foundation offset of {offset}; occupuies world space pos of {buildingPos + offset}; offset by ({xOffset},{zOffset}) to occupy positions[{position.x},{position.y}]");
                result = positions[(int)Mathf.Round(position.x), (int)Mathf.Round(position.y)];
            }
            catch
            {
                result = false;
            }
            
            if (result == false)
            {
                return false;
            }
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
        Debug.Log($"Building building at {buildingPos}");

        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            Vector2 position = new Vector2(buildingPos.x + offset.x - xOffset, buildingPos.z + offset.z - zOffset);
            Debug.Log($"Foundation offset of {offset}; occupuies world space pos of {buildingPos + offset}; offset by ({xOffset},{zOffset}) to occupy positions[{position.x},{position.y}]");

            try
            {
                positions[(int)Mathf.Round(position.x), (int)Mathf.Round(position.y)] = available;
            }
            catch
            {
                Debug.LogError($"UpdateAvailablePositions tried to update the position availability regarding {building.gameObject.name}, but encountered an error.");
            }
        }
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
