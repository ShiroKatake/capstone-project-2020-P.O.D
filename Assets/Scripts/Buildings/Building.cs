using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A building placed by the player.
/// </summary>
public class Building : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Other Components")]
    [SerializeField] private Health health;

    [Header("Building Category")]
    [SerializeField] private EBuildingCategory buildingCategory;
    [SerializeField] private EBuilding buildingType;

    [Header("Resource Requirements")]
    [SerializeField] private int oreCost;
    [SerializeField] private int powerUsage;
    [SerializeField] private int waterUsage;

    [Header("Other Stats")]
    [SerializeField] private int gridSize;
    [SerializeField] private float buildSpeed;
    [SerializeField] private float barSpeed;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    



    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// How quickly this building fills up terraforming bars.
    /// </summary>
    public float BarSpeed { get => barSpeed; }

    /// <summary>
    /// The broad category of building (cryo egg, resources, terraforming, defence) that this building falls under.
    /// </summary>
    public EBuildingCategory BuildingCategory { get => buildingCategory; }

    /// <summary>
    /// The type of building this building is.
    /// </summary>
    public EBuilding BuildingType { get => buildingType; }     
    
    /// <summary>
    /// How quickly this building builds itself when the player places it in the scene.
    /// </summary>
    public float BuildSpeed { get => buildSpeed; }
    
    /// <summary>
    /// How many squares this building occupies in the scene.
    /// </summary>
    public int GridSize { get => gridSize; }

    /// <summary>
    /// The Building's Health component.
    /// </summary>
    public Health Health { get => health; }

    /// <summary>
    /// How much ore it costs to build this building.
    /// </summary>
    public int OreCost { get => oreCost; }

    /// <summary>
    /// How much power this building requires per second to function.
    /// </summary>
    public int PowerUsage { get => powerUsage; }

    /// <summary>
    /// How much water this building requires per second to function.
    /// </summary>
    public int WaterUsage { get => waterUsage; }

    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    //private void Awake()
    //{

    //}

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    //private void Start()
    //{

    //}

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    //private void Update()
    //{

    //}

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    //private void FixedUpdate()
    //{

    //}

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  



    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------



    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------



    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------



    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
