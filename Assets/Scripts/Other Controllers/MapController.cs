using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A controller class for tracking which parts of the map have buildings, can be spawned to by aliens, etc.
/// </summary>
public class MapController : SerializableSingleton<MapController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Map Size")]
    [SerializeField] private int xMax;
    [SerializeField] private int zMax;

    [Header("No Alien Spawning Area")]
    [SerializeField] private Vector3 innerBottomLeft;
    [SerializeField] private Vector3 innerTopRight;

    [Header("Tutorial Alien Spawning Area")]
    [SerializeField] private Vector3 tutorialBottomLeft;
    [SerializeField] private Vector3 tutorialTopRight;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private bool[,] availableBuildingPositions;
    private bool[,] alienExclusionArea;
    private bool[,] availableAlienPositions;
    private LayerMask groundLayerMask;

    private List<Vector3> alienSpawnablePositions;
    private List<Vector3> tutorialAlienSpawnablePositions;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    ///// <summary>
    ///// MapController's singleton public property.
    ///// </summary>
    //public static MapController Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        //if (Instance != null)
        //{
        //    Debug.LogError("There should never be more than one MapController.");
        //}

        //Instance = this;

        base.Awake();
        availableBuildingPositions = new bool[xMax + 1 , zMax + 1];
        availableAlienPositions = new bool[xMax + 1, zMax + 1];
        alienExclusionArea = new bool[xMax + 1, zMax + 1];
        alienSpawnablePositions = new List<Vector3>();
        tutorialAlienSpawnablePositions = new List<Vector3>();
        groundLayerMask = LayerMask.GetMask("Ground");
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        float alienSpawnHeight = AlienFactory.Instance.AlienSpawnHeight;
        int noAlienXMin = (int)Mathf.Round(innerBottomLeft.x);
        int noAlienXMax = (int)Mathf.Round(innerTopRight.x);
        int noAlienZMin = (int)Mathf.Round(innerBottomLeft.z);
        int noAlienZMax = (int)Mathf.Round(innerTopRight.z);

        //Debug.Log($"Enemies cannot spawn within ({noAlienXMin}, {noAlienXMax}) to ({noAlienZMin}, {noAlienZMax})");

        for (int i = 0; i <= xMax; i++)
        {
            for (int j = 0; j <= zMax; j++)
            {
                //Debug.Log($"Assessing position ({i},{j})");
                availableBuildingPositions[i, j] = true;
                availableAlienPositions[i, j] = (i < noAlienXMin || i > noAlienXMax || j < noAlienZMin || j > noAlienZMax);//((i < noAlienXMin || i > noAlienXMax) && (j < noAlienZMin || j > noAlienZMax));
                alienExclusionArea[i, j] = !availableAlienPositions[i, j];

                //Debug.Log($"available for building: {availableBuildingPositions[i, j]}, available for enemies: {availableAlienPositions[i, j]}, alien exclusion area: {alienExclusionArea[i, j]}");

                if (availableAlienPositions[i, j])
                {
                    alienSpawnablePositions.Add(new Vector3(i, alienSpawnHeight, j));
                }
            }
        }

        int tuteAlienXMin = (int)Mathf.Round(tutorialBottomLeft.x);
        int tuteAlienXMax = (int)Mathf.Round(tutorialTopRight.x);
        int tuteAlienZMin = (int)Mathf.Round(tutorialBottomLeft.z);
        int tuteAlienZMax = (int)Mathf.Round(tutorialTopRight.z);

        for (int i = tuteAlienXMin; i <= tuteAlienXMax; i++)
        {
            for (int j = tuteAlienZMin; j <= tuteAlienZMax; j++)
            {
                if (availableBuildingPositions[i, j])
                { 
                    tutorialAlienSpawnablePositions.Add(new Vector3(i, alienSpawnHeight, j));
                }
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //Availabile Position Methods------------------------------------------------------------------

    /// <summary>
    /// Checks if a given building can legally be placed given its size and position and the spaces available.
    /// </summary>
    /// <param name="building">The building whose placement is being checked.</param>
    /// <returns>Whether the building can legally be placed.</returns>
    public bool PositionAvailableForBuilding(Building building)
    {
        Vector3 buildingPos = building.transform.position;
        //Debug.Log($"Verifying for building at {buildingPos}");

        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            if (!PositionAvailableForSpawning(buildingPos + offset, false))
            {
                //Debug.Log("MapController.PositionAvailableForBuilding returned false");
                return false;
            }
        }

        //Debug.Log("MapController.PositionAvailableForBuilding returned false");
        return true;
    }

    /// <summary>
    /// Checks if something can legally be spawned at a given position given the spaces available.
    /// </summary>
    /// <param name="position">The position the something would be spawned at.</param>
    /// <param name="alien">Whether or not the something is an alien.</param>
    /// <returns>Whether something can legally be spawned.</returns>
    public bool PositionAvailableForSpawning(Vector3 position, bool alien)
    {
        //Debug.Log($"Verifying for spawnable at {position}");
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);

        //Check if out of bounds
        if (position.x < 0 || position.x > xMax || position.z < 0 || position.z > zMax)
        {
            //Debug.Log($"Can't spawn at {position}, which is outside the bounds of (0,0) to ({xMax},{zMax})");
            return false;
        }

        //Check if already building occupiued
        if (!availableBuildingPositions[(int)position.x, (int)position.z])
        {
            //Debug.Log($"Can't spawn at {position}, which is already occupied by a building.");
            return false;
        }

        if (alien)
        {
            //Check if in alien exclusion area
            if (StageManager.Instance.CurrentStage.ID != EStage.Combat && alienExclusionArea[(int)position.x, (int)position.z]) //Need to be able to spawn within the exclusion area during the tutorial
            {
                //Debug.Log($"Can't spawn an alien at {position}, which is within the alien exclusion area.");
                return false;       
            }

            RaycastHit hit;

            //Check if a cliff or pit or too close to either
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector3 testPos = new Vector3(position.x + i, position.y, position.z + j);
                    //Debug.Log($"TestPos {testPos}");  

                    if (testPos.x < 0 || testPos.x > xMax || testPos.z < 0 || testPos.z > zMax || !Physics.Raycast(testPos, Vector3.down, out hit, 25, groundLayerMask))
                    {
                        //Debug.Log($"Out of bounds or failed to hit on raycast");
                        return false;
                    }
                    else
                    {
                        float hitHeight = hit.point.y;
                        float errorMargin = 0.01f;
                        //Debug.Log($"Test modifier ({i}, {j}), adjusted position {position}, raycast down hit at height {hitHeight}, error margin {errorMargin}");

                        if ((hitHeight < 0f - errorMargin || hitHeight > 0f + errorMargin) && (hitHeight < 2.5f - errorMargin || hitHeight > 2.5f + errorMargin))
                        {
                            //Debug.Log($"Point.y != 0 or 2.5, therefore pit or cliff, therefore not alien spawnable. Adding to alienExclusionArea.");

                            for (int k = -1; k <= 1; k++)
                            {
                                for (int l = -1; l <= 1; l++)
                                {
                                    int m = (int)(position.x + i + k);
                                    int n = (int)(position.z + j + l);

                                    if (m >= 0 && m <= xMax && n >= 0 && n <= zMax)
                                    {
                                        alienExclusionArea[m, n] = true;
                                    }
                                }
                            }
                            
                            return false;
                        }
                    }
                }
            }            
        }

        return true;
    }

    ///// <summary>
    ///// Gets a random position that an alien could legally be spawned at.
    ///// </summary>
    ///// <returns>A position for an alien to spawn at.</returns>
    //public Vector3 RandomAlienSpawnablePos(List<Vector3> temporarilyUnavailablePositions)
    //{
    //    List<Vector3> availablePositions = new List<Vector3>((StageManager.Instance.CurrentStage.ID == EStage.Combat ? tutorialAlienSpawnablePositions : alienSpawnablePositions));

    //    foreach (Vector3 p in temporarilyUnavailablePositions)
    //    {
    //        if (availablePositions.Contains(p))
    //        {
    //            availablePositions.Remove(p);
    //        }
    //    }

    //    //Debug.Log($"Getting alien spawnable position, available positions: {availablePositions.Count}");

    //    switch (availablePositions.Count)
    //    {
    //        case 0:
    //            return new Vector3 (-1, AlienFactory.Instance.AlienSpawnHeight, -1);
    //        case 1:
    //            return availablePositions[0];
    //        default:
    //            return availablePositions[Random.Range(0, availablePositions.Count)];
    //    }
    //}


    /// <summary>
    /// Gets the current list of alien spawnable positions.
    /// </summary>
    /// <returns>The current list of alien spawnable positions.</returns>
    public List<Vector3> GetAlienSpawnablePositions()
    {
        return StageManager.Instance.CurrentStage.ID == EStage.Combat ? tutorialAlienSpawnablePositions : alienSpawnablePositions;
    }

    //Entity Registration Methods------------------------------------------------------------------

    /// <summary>
    /// Registers a building with MapController so that it knows that the spaces it occupies are occupied.
    /// </summary>
    /// <param name="building">The building to be registered.</param>
    public void RegisterBuilding(Building building)
    {
        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            UpdateAvailablePosition(building.gameObject, building.transform.position + offset, false);
        }
    }
    
    /// <summary>
    /// Registers a mineral with MapController so that it knows that the space it occupies is occupied.
    /// </summary>
    /// <param name="mineral">The mineral to be registered.</param>
    public void RegisterMineral(Mineral mineral)
    {
        UpdateAvailablePosition(mineral.gameObject, mineral.transform.position, false);
    }
    
    /// <summary>
    /// Deregisters a building with MapController so that it knows that the spaces it occupied are unoccupied.
    /// </summary>
    /// <param name="building">The building to be deregistered.</param>
    public void DeRegisterBuilding(Building building)
    {
        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            UpdateAvailablePosition(building.gameObject, building.transform.position + offset, true);
        }
    }
    
    /// <summary>
    /// Deregisters a mineral with MapController so that it knows that the space it occupied is unoccupied.
    /// </summary>
    /// <param name="mineral">The mineral to be deregistered.</param>
    public void DeRegisterMineral(Mineral mineral)
    {
        UpdateAvailablePosition(mineral.gameObject, mineral.transform.position, true);
    }

    /// <summary>
    /// Informs MapController that the passed position is not on the NavMesh and shouldn't allow alien spawning.
    /// </summary>
    /// <param name="position">The position that is not on the NavMesh</param>
    public void RegisterOffMeshPosition(Vector3 position)
    {
        int x = (int)Mathf.Round(position.x);
        int z = (int)Mathf.Round(position.z);

        if (x >= 0 && x <= xMax && z >= 0 && z <= zMax)
        {
            if (StageManager.Instance.CurrentStage.ID == EStage.Combat)
            {
                Vector3 pos = new Vector3(x, AlienFactory.Instance.AlienSpawnHeight, z);
                tutorialAlienSpawnablePositions.Remove(pos);
                return;
            }
            else if (availableAlienPositions[x, z])
            {
                availableAlienPositions[x, z] = false;
                Vector3 pos = new Vector3(x, AlienFactory.Instance.AlienSpawnHeight, z);
                alienSpawnablePositions.Remove(pos);
                return;
            }
        }

        //Debug.LogError($"{gameObject.name} can't update the availability of position {position}, which is outside the bounds of (0,0) to ({xMax},{zMax})");
    }

    /// <summary>
    /// Updates the availability of the space(s) occupied / to be occupied by a building or mineral.
    /// </summary>
    /// <param name="gameObject">The game object whose space(s) are having their availability updated.</param>
    /// <param name="position">The position having its availability updated.</param>
    /// <param name="available">Is the space now available, or is it now unavailable?</param>
    private void UpdateAvailablePosition(GameObject gameObject, Vector3 position, bool available)
    {
        int x = (int)Mathf.Round(position.x);
        int z = (int)Mathf.Round(position.z);

        if (x >= 0 && x <= xMax && z >= 0 && z <= zMax)
        {
            //Debug.Log($"MapController.UpdateAvailablePositions() offset loop for {gameObject} at position {position}, x is {x}, z is {z}, xMax is {xMax}, zMax is {zMax}");
            bool startingAlienAvailability = availableAlienPositions[x, z];
            availableBuildingPositions[x, z] = available;
            availableAlienPositions[x, z] = (availableBuildingPositions[x, z] && !alienExclusionArea[x, z]);

            if (availableAlienPositions[x, z] != startingAlienAvailability)
            {
                Vector3 pos = new Vector3(x, AlienFactory.Instance.AlienSpawnHeight, z);

                if (availableAlienPositions[x, z])
                {
                    alienSpawnablePositions.Add(pos);
                }
                else
                {
                    alienSpawnablePositions.Remove(pos);
                }
            }
        }
        else
        {
            //Debug.LogError($"{gameObject.name} can't update the availability of position {position}, which is outside the bounds of (0,0) to ({xMax},{zMax})");
        }
    }
}
