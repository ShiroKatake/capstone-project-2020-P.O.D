using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A behaviour class for greenhouses.
/// </summary>
public class GreenhouseBehaviour : BuildingBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private static GreenhouseBehaviour instance = null;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------       

    /// <summary>
    /// GreenhouseBehaviour's singleton public property.
    /// </summary>
    public static GreenhouseBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GreenhouseBehaviour();
            }

            return instance;
        }
    }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// GreenhouseBehaviour's constructor method.
    /// </summary>
    private GreenhouseBehaviour()
    {
        buildingType = EBuilding.Greenhouse;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Executes the behaviour of greenhouses.
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
