using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component class for players for holding their ID so that you just need to retrieve this component, not enter the player's ID into every player input script.
/// </summary>
public class PlayerID : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private int playerID;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The value of the player's player ID.
    /// </summary>
    public int Value { get => playerID; }
}
