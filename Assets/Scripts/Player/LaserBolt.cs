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

    [Header("Laser Bolt Components")]
    [SerializeField] private Rigidbody rigidbody;
    //TODO: disable trigger collider while pooled.

    //Non-Serialized Fields------------------------------------------------------------------------

    //private Vector3 vector;
    private bool active = false;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Whether or not the laser bolt is active (i.e. has it been fired and is it currently moving).
    /// </summary>
    public bool Active { get => active; set => active = value; }

    /// <summary>
    /// The laser bolt's rigidbody component.
    /// </summary>
    public Rigidbody Rigidbody { get => rigidbody; }

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
    /// Activates a laser bolt, applying a velocity to it.
    /// </summary>
    /// <param name="vector">The normalised direction of the laser bolt's velocity.</param>
    public void Shoot(Vector3 vector)
    {
        active = true;
        rigidbody.isKinematic = false;
        //rigidbody.AddForce(vector * speed, ForceMode.VelocityChange);
        rigidbody.velocity = vector * speed;
    }

    /// <summary>
    /// Triggered if a laser bolt collides with another object.
    /// </summary>
    /// <param name="other">The collider of the other object the laser bolt collided with.</param>
    public void OnTriggerEnter(Collider other)
    {
        LaserBoltCollision(other);
    }

    /// <summary>
    /// Deals damage to enemies upon collision, before cleaning the laser bolt up.
    /// </summary>
    /// <param name="collidedWith">The collider of the other object the laser bolt collided with.</param>
    private void LaserBoltCollision(Collider collidedWith)
    {
        Enemy enemy = collidedWith.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Health.Value -= damage;
        }

        Player.Instance.DestroyLaserBolt(this);
    }
}
