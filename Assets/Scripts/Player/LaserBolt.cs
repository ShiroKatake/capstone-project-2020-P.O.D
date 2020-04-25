using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Laser bolts the player shoots at enemies.
/// </summary>
public class LaserBolt : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Laser Bolt Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private Collider collider;
    private Rigidbody rigidbody;

    //Other
    private bool active = false;
    private bool leftPlayerCollider;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Whether or not the laser bolt is active (i.e. has it been fired and is it currently moving).
    /// </summary>
    public bool Active { get => active; set => active = value; }

    /// <summary>
    /// The laser bolt's collider component.
    /// </summary>
    public Collider Collider { get => collider; }

    /// <summary>
    /// The laser bolt's rigidbody component.
    /// </summary>
    public Rigidbody Rigidbody { get => rigidbody; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        if (active)
        {
            //transform.Translate(vector * speed * Time.fixedDeltaTime);

            if (transform.position.y < 0)
            {
                Player.Instance.DestroyLaserBolt(this);
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Starts the coroutine that activates the laser bolt in the next frame.
    /// </summary>
    /// <param name="vector">The normalised direction of the laser bolt's velocity.</param>
    public void Shoot(Vector3 vector)
    {
        //Debug.Log($"Shooting laser bolt with vector {vector}");
        StartCoroutine(Shooting(vector));
    }

    /// <summary>
    /// Activates a laser bolt, applying a velocity to it.
    /// </summary>
    /// <param name="vector">The normalised direction of the laser bolt's velocity.</param>
    IEnumerator Shooting(Vector3 vector)
    {
        yield return null;

        active = true;
        rigidbody.isKinematic = false;
        collider.enabled = true;
        //rigidbody.AddForce(vector * speed, ForceMode.VelocityChange);
        rigidbody.velocity = vector * speed;
        leftPlayerCollider = false;
    }

    /// <summary>
    /// Triggered if a laser bolt collides with another object.
    /// </summary>
    /// <param name="other">The collider of the other object the laser bolt collided with.</param>
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("LaserBolt.OnTriggerEnter()");
        LaserBoltCollision(other);
    }

    /// <summary>
    /// Deals damage to enemies upon collision, before cleaning the laser bolt up.
    /// </summary>
    /// <param name="collidedWith">The collider of the other object the laser bolt collided with.</param>
    private void LaserBoltCollision(Collider collidedWith)
    {
        if (collidedWith.CompareTag("Enemy"))
        {
            collidedWith.gameObject.GetComponent<Enemy>().Health.Value -= damage;
        }

        if (!collidedWith.CompareTag("Player") && !collidedWith.CompareTag("Laser Bolt"))
        {
            //Debug.Log($"Destroying laser bolt that collided with {collidedWith.gameObject.name}");
            Player.Instance.DestroyLaserBolt(this);
        }
    }
}
