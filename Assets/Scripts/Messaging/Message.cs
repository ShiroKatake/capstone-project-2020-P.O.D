using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A message class.
/// </summary>
public class Message
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private string senderName;
    private string senderTag;
    private GameObject senderObject;
    private string messageContents;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The contents of the message.
    /// </summary>
    public string MessageContents { get => messageContents; }

    /// <summary>
    /// The name of the game object that sent this message.
    /// </summary>
    public string SenderName { get => senderName; }

    /// <summary>
    /// The game object of whatever class sent this message.
    /// </summary>
    public GameObject SenderObject { get => senderObject; }

    /// <summary>
    /// The tag of the game object that sent this message.
    /// </summary>
    public string SenderTag { get => senderTag; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Message's constructor
    /// </summary>
    /// <param name="name">The name of the sender of the message.</param>
    /// <param name="tag">The tag of the sender of the message.</param>
    /// <param name="contents">The contents of the message.</param>
    public Message(string name, string tag, GameObject gameObject, string contents)
    {
        senderName = name;
        senderTag = tag;
        senderObject = gameObject;
        messageContents = contents;
    }
}
