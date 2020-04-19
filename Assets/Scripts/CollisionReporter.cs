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

    [SerializeField] private bool reportOnCollisionEnter;
    [SerializeField] private bool reportOnCollisionExit;
    [SerializeField] private bool reportOnCollisionStay;
    [SerializeField] private bool reportOnTriggerEnter;
    [SerializeField] private bool reportOnTriggerExit;
    [SerializeField] private bool reportOnTriggerStay;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<ICollisionListener> collisionListeners;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

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
        if (reportOnCollisionEnter)
        {
            //Debug.Log("CollisionReporter.OnCollisionEnter()");
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
        if (reportOnCollisionExit)
        {
            //Debug.Log("CollisionReporter.OnCollisionExit()");
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
        if (reportOnCollisionStay)
        {
            //Debug.Log("CollisionReporter.OnCollisionStay()");
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
        if (reportOnTriggerEnter)
        {
            //Debug.Log("CollisionReporter.OnTriggerEnter()");
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
        if (reportOnTriggerExit)
        {
            //Debug.Log("CollisionReporter.OnTriggerExit()");
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
        if (reportOnTriggerStay)
        {
            //Debug.Log("CollisionReporter.OnTriggerStay()");
            foreach (ICollisionListener l in collisionListeners)
            {
                l.OnTriggerStay(other);
            }
        }
    }
}
