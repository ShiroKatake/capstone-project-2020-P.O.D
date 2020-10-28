using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates unique ID numbers.
/// </summary>
public class IdGenerator : Singleton<IdGenerator>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private int nextId;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// IdGenerator's constructor method. 
    /// WARNING: DO NOT CALL IN-CODE. IT IS ONLY PUBLIC FOR SINGLETON.INSTANCE TO USE.
    /// </summary>
    public IdGenerator()
    {
        nextId = -1;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Increments and returns the next available unique ID number.
    /// </summary>
    public int GetNextId()
    {
        nextId++;
        return nextId;
    }
}
