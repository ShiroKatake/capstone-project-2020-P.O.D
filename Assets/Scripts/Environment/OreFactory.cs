using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for ores.
/// </summary>
public class OreFactory : Factory<OreFactory, Ore, ENone>
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	//Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Ore Stats")]
	[SerializeField] private int oreValue;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Basic Public Properties----------------------------------------------------------------------                                                    

    /// <summary>
    /// How much ore should ore nodes yield.
    /// </summary>
	public int OreValue { get => oreValue; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    protected override void Start()
    {
        base.Start();

        foreach (Ore o in pool[ENone.None])
        {
            o.gameObject.SetActive(false);
        }
    }

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get a new ore.
    /// </summary>
    /// <param name="type">The type of ore to instantiate. Should be left as default value of ENone.None.</param>
    /// <returns>An ore.</returns>
    public override Ore Get(ENone type = ENone.None)
    {
        return base.Get(type);
    }

    /// <summary>
    /// Destroy an ore.
    /// </summary>
    /// <param name="ore">The ore to destroy.</param>
    /// <param name="type">The type of ore to destroy. Should be left as default value of ENone.None.</param>
	public override void Destroy(Ore ore, ENone type = ENone.None)
	{
        base.Destroy(ore, type);
	}
}
