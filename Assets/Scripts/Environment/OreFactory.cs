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
    /// <returns>An ore.</returns>
    public Ore Get()
    {
        return Get(ENone.None);
    }

    /// <summary>
    /// Destroy an ore.
    /// </summary>
    /// <param name="ore">The ore to destroy.</param>
    public void Destroy(Ore ore)
    {
        Destroy(ENone.None, ore);
    }

    /// <summary>
    /// Destroy an ore.
    /// </summary>
    /// <param name="type">The type of ore to destroy.</param>
    /// <param name="ore">The ore to destroy.</param>
	public override void Destroy(ENone type, Ore ore)
	{
		ore.gameObject.SetActive(false);
        base.Destroy(type, ore);
	}
}
