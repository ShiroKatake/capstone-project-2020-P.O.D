using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A template for organising comments that can be copied and pasted into a new class.
/// </summary>
public class CommentsTemplate : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    



    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// [CLASSNAME]'s singleton public property.
    /// </summary>
    public static CommentsTemplate Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one [CLASSNAME].");
        }

        Instance = this;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        
    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        
    }

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    

    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------

    

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------



    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  

    
}
