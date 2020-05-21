using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface for objects whose children have colliders that need to report collisions and triggers via CollisionReporter.
/// </summary>
public abstract class CollisionListener : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields------------------------------------------------------------------------

    private List<CollisionReporter> collisionReporters;

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets all attached or childed collision reporters that report to this collision listener.
    /// </summary>
    /// <returns>A list of collision reporters that report to this collision listener.</returns>
    protected List<CollisionReporter> GetCollisionReporters()
    {
        List<CollisionReporter> myCollisionReporters = new List<CollisionReporter>();
        CollisionReporter[] allCollisionReporters = GetComponentsInChildren<CollisionReporter>();

        foreach (CollisionReporter c in allCollisionReporters)
        {
            if (c.ReportsTo(this))
            {
                myCollisionReporters.Add(c);
            }
        }

        return myCollisionReporters;
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public abstract void OnCollisionEnter(Collision collision);

    /// <summary>
    /// OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public abstract void OnCollisionExit(Collision collision);

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public abstract void OnCollisionStay(Collision collision);

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public abstract void OnTriggerEnter(Collider other);

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public abstract void OnTriggerExit(Collider other);

    /// <summary>
    /// OnTriggerStay is called almost all the frames for every Collider other that is touching the trigger. The function is on the physics timer so it won't necessarily run every frame.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public abstract void OnTriggerStay(Collider other);
}
