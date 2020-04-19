using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A template for organising comments that can be copied and pasted into a new class.
/// </summary>
public class CollisionReporter : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private bool onCollisionEnter;
    [SerializeField] private bool onCollisionExit;
    [SerializeField] private bool onCollisionStay;
    [SerializeField] private bool onTriggerEnter;
    [SerializeField] private bool onTriggerExit;
    [SerializeField] private bool onTriggerStay;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<ICollisionListener> collisionListeners;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        collisionListeners = new List<ICollisionListener>(GetComponentsInParent<ICollisionListener>());
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    void OnCollisionEnter(Collision collision)
    {
        if (onCollisionEnter)
        {
            Debug.Log("CollisionReporter.OnCollisionEnter()");
            foreach (ICollisionListener l in collisionListeners)
            {
                l.OnCollisionEnter(collision);
            }
        }
    }

    /// <summary>
    /// OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    void OnCollisionExit(Collision collision)
    {
        if (onCollisionExit)
        {
            Debug.Log("CollisionReporter.OnCollisionExit()");
            foreach (ICollisionListener l in collisionListeners)
            {
                l.OnCollisionExit(collision);
            }
        }
    }

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    void OnCollisionStay(Collision collision)
    {
        if (onCollisionStay)
        {
            Debug.Log("CollisionReporter.OnCollisionStay()");
            foreach (ICollisionListener l in collisionListeners)
            {
                l.OnCollisionStay(collision);
            }
        }
    }

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter)
        {
            Debug.Log("CollisionReporter.OnTriggerEnter()");
            foreach (ICollisionListener l in collisionListeners)
            {
                l.OnTriggerEnter(other);
            }
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        if (onTriggerExit)
        {
            Debug.Log("CollisionReporter.OnTriggerExit()");
            foreach (ICollisionListener l in collisionListeners)
            {
                l.OnTriggerExit(other);
            }
        }
    }

    /// <summary>
    /// OnTriggerStay is called almost all the frames for every Collider other that is touching the trigger. The function is on the physics timer so it won't necessarily run every frame.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerStay(Collider other)
    {
        if (onTriggerStay)
        {
            Debug.Log("CollisionReporter.OnTriggerStay()");
            foreach (ICollisionListener l in collisionListeners)
            {
                l.OnTriggerStay(other);
            }
        }
    }
}
