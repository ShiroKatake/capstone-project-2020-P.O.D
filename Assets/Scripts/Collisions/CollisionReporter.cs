using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reports collisions to entity classes on parent transforms.
/// </summary>
public class CollisionReporter : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private string purpose;
    [SerializeField] private List<CollisionListener> collisionListeners;
    [SerializeField] private bool reportOnCollisionEnter;
    [SerializeField] private bool reportOnCollisionExit;
    [SerializeField] private bool reportOnCollisionStay;
    [SerializeField] private bool reportOnTriggerEnter;
    [SerializeField] private bool reportOnTriggerExit;
    [SerializeField] private bool reportOnTriggerStay;
    
    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private List<Collider> colliders;
    private Rigidbody rigidbody;

    //TODO: check if reportX switches are turned off when no longer needed AND reportX switches are reset properly when a building is pooled

    //Public Properties------------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// The purpose of the colliders attached to the collision reporter.
    /// </summary>
    public string Purpose { get => purpose; }

    /// <summary>
    /// Should CollisionReporter report OnCollisionEnter messages to its ICollisionListeners?
    /// </summary>
    public bool ReportOnCollisionEnter { get => reportOnCollisionEnter; set => reportOnCollisionExit = value; }

    /// <summary>
    /// Should CollisionReporter report OnCollisionExit messages to its ICollisionListeners?
    /// </summary>
    public bool ReportOnCollisionExit { get => reportOnCollisionExit; set => reportOnCollisionExit = value; }

    /// <summary>
    /// Should CollisionReporter report OnCollisionStay messages to its ICollisionListeners?
    /// </summary>
    public bool ReportOnCollisionStay { get => reportOnCollisionStay; set => reportOnCollisionStay = value; }

    /// <summary>
    /// Should CollisionReporter report OnTriggerEnter messages to its ICollisionListeners?
    /// </summary>
    public bool ReportOnTriggerEnter { get => reportOnTriggerEnter; set => reportOnTriggerEnter = value; }

    /// <summary>
    /// Should CollisionReporter report OnTriggerExit messages to its ICollisionListeners?
    /// </summary>
    public bool ReportOnTriggerExit { get => reportOnTriggerExit; set => reportOnTriggerExit = value; }

    /// <summary>
    /// Should CollisionReporter report OnTriggerStay messages to its ICollisionListeners?
    /// </summary>
    public bool ReportOnTriggerStay { get => reportOnTriggerStay; set => reportOnTriggerStay = value; }

    /// <summary>
    /// This collision reporter's rigidbody component.
    /// </summary>
    public Rigidbody Rigidbody { get => rigidbody; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        colliders = new List<Collider>(GetComponents<Collider>());
        rigidbody = GetComponent<Rigidbody>();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Does this collision reporter report to the given collision listener?
    /// </summary>
    /// <param name="collisionListener">The collision listener that this collision reporter might report to.</param>
    /// <returns>Whether this collision reporter reports to the given collision listener or not.</returns>
    public bool ReportsTo(CollisionListener collisionListener)
    {
        return collisionListeners.Contains(collisionListener);
    }

    /// <summary>
    /// Sets the enabled property of all of the collision reporter's colliders to the passed value.
    /// </summary>
    /// <param name="value">What the colliders' enabled properties are being set to.</param>
    public void SetCollidersEnabled(bool value)
    {
        foreach (Collider c in colliders)
        {
            c.enabled = value;
        }
    }

    /// <summary>
    /// Sets the isTrigger property of all of the collision reporter's colliders to the passed value.
    /// </summary>
    /// <param name="value">What the colliders' isTrigger properties are being set to.</param>
    public void SetCollidersIsTrigger(bool value)
    {
        foreach (Collider c in colliders)
        {
            c.isTrigger = value;
        }
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    void OnCollisionEnter(Collision collision)
    {
        if (reportOnCollisionEnter)
        {
            //Debug.Log("CollisionReporter.OnCollisionEnter()");
            foreach (CollisionListener l in collisionListeners)
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
        if (reportOnCollisionExit)
        {
            //Debug.Log("CollisionReporter.OnCollisionExit()");
            foreach (CollisionListener l in collisionListeners)
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
        if (reportOnCollisionStay)
        {
            //Debug.Log("CollisionReporter.OnCollisionStay()");
            foreach (CollisionListener l in collisionListeners)
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
        if (reportOnTriggerEnter)
        {
            //Debug.Log("CollisionReporter.OnTriggerEnter()");
            foreach (CollisionListener l in collisionListeners)
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
        if (reportOnTriggerExit)
        {
            //Debug.Log("CollisionReporter.OnTriggerExit()");
            foreach (CollisionListener l in collisionListeners)
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
        if (reportOnTriggerStay)
        {
            //Debug.Log("CollisionReporter.OnTriggerStay()");
            foreach (CollisionListener l in collisionListeners)
            {
                l.OnTriggerStay(other);
            }
        }
    }
}
