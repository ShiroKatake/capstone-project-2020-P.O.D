using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A dispatcher for messages.
/// </summary>
public class MessageDispatcher : SerializableSingleton<MessageDispatcher>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private Dictionary<string, List<IMessenger>> subscribers;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        subscribers = new Dictionary<string, List<IMessenger>>();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Subscribes a messenger to the message dispatcher to receive messages according to its tag.
    /// </summary>
    /// <param name="tag">The tag of the game object of the subscriber.</param>
    /// <param name="subscriber">The messenger subscribing to the message dispatcher.</param>
    public void Subscribe(string tag, IMessenger subscriber)
    {
        if (!subscribers.ContainsKey(tag))
        {
            subscribers[tag] = new List<IMessenger>();
            subscribers[tag].Add(subscriber);
        }
        else if (!subscribers[tag].Contains(subscriber))
        {
            subscribers[tag].Add(subscriber);
        }
    }

    /// <summary>
    /// Gives a message to MessageDispatcher to deliver.
    /// </summary>
    /// <param name="recipientTag">The tag of the game object of the recipient.</param>
    /// <param name="message">The message to deliver.</param>
    public void SendMessage(string recipientTag, Message message)
    {
        if (subscribers.ContainsKey(recipientTag))
        {
            foreach (IMessenger subscriber in subscribers[recipientTag])
            {
                subscriber.Receive(message);
            }
        }
    }

    /// <summary>
    /// Unsubscribes a game object from the message dispatcher so that it no longer receives messages.
    /// </summary>
    /// <param name="tag">The tag of the game object of the messenger.</param>
    /// <param name="subscriber">The messenger unsubscribing from the message dispatcher.</param>
    public void Unsubscribe(string tag, IMessenger subscriber)
    {
        if (subscribers.ContainsKey(tag) && subscribers[tag].Contains(subscriber))
        {
            subscribers[tag].Remove(subscriber);
        }
    }
}
