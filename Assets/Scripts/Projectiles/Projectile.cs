﻿using System.Collections;
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
    [SerializeField] private EProjectileType type;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float lifespan;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private Collider collider;
    private Rigidbody rigidbody;

    //Other
    private bool active = false;
    private Transform owner;
    private bool leftOwnerCollider;
    private float timeOfLastShot;

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

    /// <summary>
    /// The type of projectile a projectile is.
    /// </summary>
    public EProjectileType Type { get => type; }

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
        if (active && (transform.position.y < 0 || Time.time - timeOfLastShot > lifespan))
        {
            ProjectileFactory.Instance.DestroyProjectile(this);
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //Shooting-------------------------------------------------------------------------------------
    
    /// <summary>
    /// Starts the coroutine that activates the projectile in the next frame.
    /// </summary>
    /// <param name="vector">The normalised direction of the projectile's velocity.</param>
    /// <param name="movementSpeed">The speed of the shooter in the direction the projectile will travel.</param>
    public void Shoot(Vector3 vector, float movementSpeed)
    {
        StartCoroutine(Shooting(vector, movementSpeed));
    }

    /// <summary>
    /// Activates a projectile, applying a velocity to it.
    /// </summary>
    /// <param name="vector">The normalised direction of the projectile's velocity.</param>
    IEnumerator Shooting(Vector3 vector, float movementSpeed)
    {
        yield return null;

        timeOfLastShot = Time.time;
        active = true;
        rigidbody.isKinematic = false;
        collider.enabled = true;
        rigidbody.velocity = vector * (speed + movementSpeed);
        leftOwnerCollider = false;
    }

    //Collisions-----------------------------------------------------------------------------------

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public void OnTriggerEnter(Collider other)
    {
        ProjectileCollision(other);
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("ProjectileCollision OnTriggerExit");
        if (!leftOwnerCollider && other.CompareTag(owner.tag))
        {
            //Debug.Log("Left owner collider");
            leftOwnerCollider = true;
        }
    }

    /// <summary>
    /// Deals damage to enemies upon collision, before cleaning the projectile up.
    /// </summary>
    /// <param name="collidedWith">The collider of the other object the projectile collided with.</param>
    private void ProjectileCollision(Collider collidedWith)
    {
        if (collidedWith.CompareTag("Alien"))
        {           
            Alien a = collidedWith.gameObject.GetComponent<Alien>();
            a.ShotBy(owner.name, owner.GetComponentInChildren<Collider>().transform);
            a.Health.Value -= damage;
            AudioManager.Instance.PlaySound(AudioManager.ESound.Alien_Takes_Damage, this.gameObject);
            //Debug.Log($"{gameObject.name} reduced {a.gameObject.name}'s health to {a.Health.Value}; {a.gameObject.name}.ShotBy is now {owner.name}");
        }

        if (!collidedWith.CompareTag("Projectile") && !collidedWith.isTrigger && (!collidedWith.CompareTag(owner.tag) || leftOwnerCollider))
        {
            //Debug.Log($"ProjectileCollision, not Player or Projectile; tag is {collidedWith.tag}; position is {transform.position}");
            ProjectileFactory.Instance.DestroyProjectile(this);
        }
    }
}
