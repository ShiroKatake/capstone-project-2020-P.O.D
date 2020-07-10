using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is introduced to the buildings and how terraforming works.
/// </summary>
public class StageTerraforming : Stage
{
    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// StageTerraforming's singleton public property.
    /// </summary>
    public StageTerraforming Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one StageTerraforming.");
        }

        Instance = this;
        id = EStage.Terraforming;
        base.Awake();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of StageTerraforming.
    /// </summary>
    public override void Execute()
    {
        switch (step)
        {
            default:

                break;
        }
    }
}
