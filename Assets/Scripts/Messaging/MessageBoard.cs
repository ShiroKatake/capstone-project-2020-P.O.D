using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A message board for posting messages.
/// </summary>
public class MessageBoard : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<Message> messages;
    private List<Message> expiredMessages;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// MessageBoard's singleton public property.
    /// </summary>
    public static MessageBoard Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The messages posted on the message board. Once the list has been retrieved, it and its contents should not be modified. Any modification to the list or its contents should be done via MessageBoard.Add() or MessageBoard.Remove().
    /// </summary>
    public List<Message> Messages { get => messages; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one MessageBoard.");
        }

        Instance = this;
        messages = new List<Message>();
        expiredMessages = new List<Message>();
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        //Debug.Log($"MessageBoard: {messages.Count} messages");
        foreach (Message m in messages)
        {
            m.DecrementLifespan();
        }

        while (expiredMessages.Count > 0)
        {
            messages.Remove(expiredMessages[0]);
            expiredMessages.RemoveAt(0);
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Adds a message to the message board.
    /// </summary>
    /// <param name="message">The message to be added.</param>
    public void Add(Message message)
    {
        messages.Add(message);
    }

    /// <summary>
    /// Removes a message from the message board.
    /// </summary>
    /// <param name="message">The message to be removed.</param>
    public void Remove(Message message)
    {
        expiredMessages.Add(message);
    }
}
