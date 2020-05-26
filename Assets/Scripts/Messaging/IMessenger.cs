using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface for classes that interact with MessageDispatcher.
/// </summary>
public interface IMessenger
{
    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// A template method for IMesseger children to receive messages.
    /// </summary>
    /// <param name="message">The message to be delivered to the IMessenger.</param>
    void Receive(Message message);
}
