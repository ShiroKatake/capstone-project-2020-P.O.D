using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates unique ID numbers.
/// </summary>
public class IdGenerator
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private static IdGenerator instance = null;
    private int nextId;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// IdGenerator's singleton public property.
    /// </summary>
    public static IdGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new IdGenerator();
            }

            return instance;
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// IdGenerator's constructor method.
    /// </summary>
    private IdGenerator()
    {
        nextId = -1;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Increments and returns the next available unique ID number.
    /// </summary>
    public int GetNextId()
    {
        //Debug.Log("ID Generator is generating a new ID.");
        nextId++;
        return nextId;
    }
}
