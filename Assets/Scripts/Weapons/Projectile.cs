using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Projectiles to be shot at enemies.
/// </summary>
public class Projectile : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Projectile Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private Collider collider;
    private Rigidbody rigidbody;

    //Other
    private bool active = false;
    private bool leftOwnerCollider;
    private Transform owner;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Whether or not the projectile is active (i.e. has it been fired and is it currently moving).
    /// </summary>
    public bool Active { get => active; set => active = value; }

    /// <summary>
    /// The projectile's collider component.
    /// </summary>
    public Collider Collider { get => collider; }

    /// <summary>
    /// The entity that fired the projectile. Should only be set by ProjectileFactory.
    /// </summary>
    public Transform Owner { get => owner; set => owner = value; }

    /// <summary>
    /// The projectile's rigidbody component.
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
                ProjectileFactory.Instance.DestroyProjectile(this);
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Starts the coroutine that activates the projectile in the next frame.
    /// </summary>
    /// <param name="vector">The normalised direction of the projectile's velocity.</param>
    public void Shoot(Vector3 vector)
    {
        StartCoroutine(Shooting(vector));
    }

    /// <summary>
    /// Activates a projectile, applying a velocity to it.
    /// </summary>
    /// <param name="vector">The normalised direction of the projectile's velocity.</param>
    IEnumerator Shooting(Vector3 vector)
    {
        yield return null;

        active = true;
        rigidbody.isKinematic = false;
        collider.enabled = true;
        rigidbody.velocity = vector * speed;
        leftOwnerCollider = false;
    }

    /// <summary>
    /// Triggered if a projectile collides with another object.
    /// </summary>
    /// <param name="other">The collider of the other object the projectile collided with.</param>
    public void OnTriggerEnter(Collider other)
    {
        ProjectileCollision(other);
    }

    //TODO: on trigger exit check for the owner

    /// <summary>
    /// Deals damage to enemies upon collision, before cleaning the projectile up.
    /// </summary>
    /// <param name="collidedWith">The collider of the other object the projectile collided with.</param>
    private void ProjectileCollision(Collider collidedWith)
    {
        //Debug.Log("ProjectileCollision");
        if (collidedWith.CompareTag("Enemy"))
        {
            //Debug.Log("ProjectileCollision, Enemy");
            Enemy e = collidedWith.gameObject.GetComponent<Enemy>();
            e.Health.Value -= damage;
            e.ShotBy = owner.GetComponentInChildren<Collider>().transform;
            Debug.Log($"{gameObject.name} reduced {e.gameObject.name}'s health to {e.Health.Value}; {e.gameObject.name}.ShotBy is now {owner.name}");
        }

        if (!collidedWith.CompareTag("Player") && !collidedWith.CompareTag("Projectile"))
        {
            //Debug.Log($"ProjectileCollision, not Player or Projectile; tag is {collidedWith.tag}; position is {transform.position}");
            //TODO: once the projectile leaves the shooter's collider, it needs to be able to die on contact with said shooter; use leftOwnerCollider to indicate this.
            ProjectileFactory.Instance.DestroyProjectile(this);
        }
    }
}
