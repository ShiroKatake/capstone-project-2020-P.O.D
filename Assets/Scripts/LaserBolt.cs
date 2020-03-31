using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBolt : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [Header("Laser Bolt Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    [Header("Laser Bolt Components")]
    [SerializeField] private Rigidbody rigidbody;

    //Non-Serialized Fields

    //private Vector3 vector;
    private bool active = false;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties

    //public Rigidbody Rigidbody { get => rigidbody; }
    //public float Speed { get => speed; }

    //Recurring Methods (Fixed)----------------------------------------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        if (active)
        {
            //transform.Translate(vector * speed * Time.fixedDeltaTime);

            if (transform.position.y < 0)
            {
                LaserBoltCollision();
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    public void Shoot(Vector3 vector)
    {
        active = true;
        rigidbody.isKinematic = false;
        //rigidbody.AddForce(vector * speed, ForceMode.VelocityChange);
        rigidbody.velocity = vector * speed;
    }

    public void OnTriggerEnter(Collider other)
    {
        LaserBoltCollision(other);
    }

    private void LaserBoltCollision(Collider collidedWith)
    {
        Enemy enemy = collidedWith.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Health.Value -= damage;
        }

        LaserBoltCollision();
    }

    private void LaserBoltCollision()
    {
        rigidbody.isKinematic = true;
        Player.Instance.LaserBattery.Add(this);
        transform.position = Player.Instance.LaserBatteryPoint.position;
        active = false;
    }
}
