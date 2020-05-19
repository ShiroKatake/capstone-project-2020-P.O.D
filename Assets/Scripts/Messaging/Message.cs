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
    private string messageContents;
    private int messageLifespan;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The contents of the message.
    /// </summary>
    public string MessageContents { get => messageContents; }

    /// <summary>
    /// The lifespan of the message in frames.
    /// </summary>
    public int MessageLifespan { get => messageLifespan; }
    public string SenderName { get => senderName; }
    public string SenderTag { get => senderTag; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Message's constructor
    /// </summary>
    /// <param name="name">The name of the sender of the message.</param>
    /// <param name="tag">The tag of the sender of the message.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="lifespan">The lifespan of the message in frames.</param>
    public Message(string name, string tag, string contents, int lifespan)
    {
        senderName = name;
        senderTag = tag;
        messageContents = contents;
        messageLifespan = lifespan;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Decrements the lifespan of the message, and calls for MessageBoard to remove it if it's lifespan has reached 0.
    /// </summary>
    public void DecrementLifespan()
    {
        messageLifespan -= 1;

        if (messageLifespan <= 0)
        {
            MessageBoard.Instance.Remove(this);
        }
    }
}
