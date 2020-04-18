using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A template for organising comments that can be copied and pasted into a new class.
/// </summary>
public class CollisionReporter : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private Collider collider;
    private List<ICollisionListener> collisionListeners;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        collider = GetComponent<Collider>();
        collisionListeners = new List<ICollisionListener>(GetComponentsInParent<ICollisionListener>());
        Debug.Log($"CollisionReporter found {collisionListeners.Count} ICollisionListeners.");
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CollisionReporter.OnCollisionEnter()");
        foreach (ICollisionListener l in collisionListeners)
        {
            l.OnCollisionEnter(collision);
        }
    }

    /// <summary>
    /// OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    void OnCollisionExit(Collision collision)
    {
        Debug.Log("CollisionReporter.OnCollisionExit()");
        foreach (ICollisionListener l in collisionListeners)
        {
            l.OnCollisionExit(collision);
        }
    }

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    void OnCollisionStay(Collision collision)
    {
        Debug.Log("CollisionReporter.OnCollisionStay()");
        foreach (ICollisionListener l in collisionListeners)
        {
            l.OnCollisionStay(collision);
        }
    }

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("CollisionReporter.OnTriggerEnter()");
        foreach (ICollisionListener l in collisionListeners)
        {
            l.OnTriggerEnter(other);
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        Debug.Log("CollisionReporter.OnTriggerExit()");
        foreach (ICollisionListener l in collisionListeners)
        {
            l.OnTriggerExit(other);
        }
    }

    /// <summary>
    /// OnTriggerStay is called almost all the frames for every Collider other that is touching the trigger. The function is on the physics timer so it won't necessarily run every frame.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerStay(Collider other)
    {
        Debug.Log("CollisionReporter.OnTriggerStay()");
        foreach (ICollisionListener l in collisionListeners)
        {
            l.OnTriggerStay(other);
        }
    }
}
