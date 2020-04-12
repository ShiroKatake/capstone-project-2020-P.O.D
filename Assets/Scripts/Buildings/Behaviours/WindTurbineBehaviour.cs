using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A behaviour class for wind turbines.
/// </summary>
public class WindTurbineBehaviour : BuildingBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private static WindTurbineBehaviour instance = null;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------       
    
    /// <summary>
    /// WindTurbineBehaviour's singleton public property.
    /// </summary>
    public static WindTurbineBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WindTurbineBehaviour();
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
    private WindTurbineBehaviour()
    {
        buildingType = EBuilding.WindTurbine;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Executes the behaviour of wind turbines.
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
