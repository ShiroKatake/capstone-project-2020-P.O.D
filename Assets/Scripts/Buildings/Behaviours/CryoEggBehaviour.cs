using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A behaviour class for the cryo egg.
/// </summary>
public class CryoEggBehaviour : BuildingBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private static CryoEggBehaviour instance = null;
    private bool setup;
    private Material material;
    private float colourLerpProgress;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------       

    /// <summary>
    /// CryoEggBehaviour's singleton public property.
    /// </summary>
    public static CryoEggBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CryoEggBehaviour();
            }

            return instance;
        }
    }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// CryoEggBehaviour's constructor method.
    /// </summary>
    private CryoEggBehaviour()
    {
        buildingType = EBuilding.CryoEgg;
        setup = false;
        colourLerpProgress = 0;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Executes the behaviour of the cryo egg.
    /// </summary>
    /// <param name="building">The building executing its behaviour.</param>
    public override void Execute(Building building)
    {
        if (building.BuildingType == buildingType)
        {
            if (!setup)
            {
                MeshRenderer meshRenderer = building.gameObject.GetComponent<MeshRenderer>();
                meshRenderer.material = new Material(meshRenderer.material);
                material = meshRenderer.material;
                setup = true;
                Debug.Log("CryoEggBehaviour.Setup, !setup()");
            }

            if (colourLerpProgress != building.Health.Value * 0.01)
            {
                colourLerpProgress = building.Health.Value * 0.01f;
                //Debug.Log($"colourLerpProgress updated to {colourLerpProgress}");
                material.color = Color.Lerp(Color.red, Color.white, colourLerpProgress);
                //Debug.Log($"colour updated to {material.color.ToString()} ({material.color.r}, {material.color.g}, {material.color.b}, {material.color.a})");
            }
        }
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
