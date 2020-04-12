using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A behaviour class for solar panels.
/// </summary>
public class SolarPanelBehaviour : BuildingBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private static SolarPanelBehaviour instance = null;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------       

    /// <summary>
    /// SolarPanelBehaviour's singleton public property.
    /// </summary>
    public static SolarPanelBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SolarPanelBehaviour();
            }

            return instance;
        }
    }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// SolarPanelBehaviour's constructor method.
    /// </summary>
    private SolarPanelBehaviour()
    {
        buildingType = EBuilding.SolarPanel;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Executes the behaviour of solar panels.
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
