using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A behaviour class for short range turrets.
/// </summary>
public class ShortRangeTurretBehaviour : BuildingBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private static ShortRangeTurretBehaviour instance = null;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------       

    /// <summary>
    /// ShortRangeTurretBehaviour's singleton public property.
    /// </summary>
    public static ShortRangeTurretBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ShortRangeTurretBehaviour();
            }

            return instance;
        }
    }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// ShortRangeTurretBehaviour's constructor method.
    /// </summary>
    private ShortRangeTurretBehaviour()
    {
        buildingType = EBuilding.ShortRangeTurret;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Executes the behaviour of short range turrets.
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
