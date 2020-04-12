using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A behaviour class for water drills.
/// </summary>
public class WaterDrillBehaviour : BuildingBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private static WaterDrillBehaviour instance = null;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------       

    /// <summary>
    /// WaterDrillBehaviour's singleton public property.
    /// </summary>
    public static WaterDrillBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WaterDrillBehaviour();
            }

            return instance;
        }
    }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// WindTurbineBehaviour's constructor method.
    /// </summary>
    private WaterDrillBehaviour()
    {
        buildingType = EBuilding.WaterDrill;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Executes the behaviour of water drills.
    /// </summary>
    /// <param name="building">The building executing its behaviour.</param>
    public override void Execute(Building building)
    {
        if (building.BuildingType == buildingType)
        {
            return;
        }
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
