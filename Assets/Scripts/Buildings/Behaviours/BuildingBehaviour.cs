using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract class for building behaviours.
/// </summary>
public abstract class BuildingBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    protected EBuilding buildingType;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Abstract method for executing the behaviours of a particular type of building.
    /// </summary>
    /// <param name="building">The building that's executing "its" behaviour.</param>
    public abstract void Execute(Building building);

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
