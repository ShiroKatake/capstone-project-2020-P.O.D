using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo class for enemies.
/// </summary>
public class Enemy : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Enemy Stats")] 
    [SerializeField] private int id;
    [SerializeField] private float speed;
    [SerializeField] private float turningSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;

    //Non-Serialized Fields------------------------------------------------------------------------

    private Health health;
    private Rigidbody rigidbody;

    private bool moving;
    private float groundHeight;

    private Building cryoEgg;
    private List<Transform> visibleAliens;
    private List<Transform> visibleTargets;
    [SerializeField] private Transform target;
    [SerializeField] private Health targetHealth;
    private Transform shooter;
    private float timeOfLastAttack;

    private Quaternion oldRotation;
    private Quaternion targetRotation;
    private float slerpProgress;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Enemy's Health component.
    /// </summary>
    public Health Health { get => health; }

    /// <summary>
    /// Whether or not the Enemy is moving.
    /// </summary>
    public bool Moving { get => moving; set => moving = value; }

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// Enemy's unique ID number. Id should only be set by Enemy.Setup().
    /// </summary>
    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
            gameObject.name = $"Enemy {id}";
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
        health = GetComponent<Health>();
        rigidbody = GetComponent<Rigidbody>();

        cryoEgg = BuildingController.Instance.CryoEgg;

        groundHeight = transform.position.y;

        visibleAliens = new List<Transform>();
        visibleTargets = new List<Transform>();
    }

    /// <summary>
    /// Prepares the Enemy to chase its targets when EnemyFactory puts it in the world. 
    /// </summary>
    public void Setup(int id)
    {
        Id = id;
        health.Reset();
        target = cryoEgg.transform;
        timeOfLastAttack = attackCooldown * -1;

        //Rotate to face the cryo egg
        //Vector3 targetRotation = cryoEgg.transform.position - transform.position;
        //transform.rotation = Quaternion.LookRotation(targetRotation);
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        
    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        CheckHealth();
        //TODO: swarm-based behaviour
        SelectTarget();
        Look();
        Move();
    }

    //Recurring Methods (FixedUpdate())-------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Checks if Enemy has 0 health, destroying it if it has.
    /// </summary>
    private void CheckHealth()
    {
        if (health.IsDead())
        {
            EnemyFactory.Instance.DestroyEnemy(this);
        }
    }

    /// <summary>
    /// Selects the most appropriate target for the enemy.
    /// </summary>
    private void SelectTarget()
    {
        if (visibleTargets.Count > 0)
        {
            float distance = 99999999999;
            float closestDistance = 9999999999999999;
            Transform closestTarget = null;

            foreach (Transform t in visibleTargets)
            {
                distance = Vector3.Distance(transform.position, t.position);

                if (closestTarget == null || distance < closestDistance)
                {
                    closestTarget = t;
                    closestDistance = distance;
                }
            }

            if (target != closestTarget)
            {
                target = closestTarget;
                targetHealth = target.GetComponent<Health>();

                while (targetHealth == null && target.parent != null)
                {
                    target = target.parent;
                    targetHealth = target.GetComponent<Health>();
                }

                if (targetHealth == null)
                {
                    Debug.LogError($"Enemy.SelectTarget cannot find {target}'s Health component.");
                }
            }
        }
        else if (target != cryoEgg.transform)
        {
            target = cryoEgg.transform;
            targetHealth = target.GetComponent<Health>();
        }
    }

    /// <summary>
    /// Alien uses input information to determine which direction it should be facing
    /// </summary>
    private void Look()
    {





        Vector3 newRotation = target.position - transform.position;

        if (newRotation != targetRotation.eulerAngles)
        {
            oldRotation = transform.rotation;
            targetRotation = Quaternion.LookRotation(newRotation);
            slerpProgress = 0f;
        }

        if (slerpProgress < 1)
        {
            slerpProgress = Mathf.Min(1, slerpProgress + turningSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(oldRotation, targetRotation, slerpProgress);
        }
    }

    /// <summary>
    /// Moves Enemy.
    /// </summary>
    private void Move()
    {
        transform.Translate(new Vector3(0, 0, speed * Time.fixedDeltaTime));

        //Toggle gravity if something has pushed the enemy up above groundHeight
        if (rigidbody.useGravity)
        {
            if (transform.position.y <= groundHeight)
            {
                transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
                rigidbody.useGravity = false;
            }
        }
        else
        {
            if (transform.position.y > groundHeight)   //TODO: account for terrain pushing the enemy up, if it can move up hills?
            {
                rigidbody.useGravity = true;
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.isTrigger && (collision.collider.CompareTag("Building") || collision.collider.CompareTag("Player")))
        {
            Debug.Log($"Enemy.OnCollisionStay, non-trigger targetable. Collider: {collision.collider.gameObject}, target: {target.gameObject}, time: {Time.time}, timeOfLastAttack: {timeOfLastAttack}, attack cooldown: {attackCooldown}");
            if (Time.time - timeOfLastAttack > attackCooldown)
            {
                Transform temp = collision.collider.transform;


                if (temp.gameObject.GetComponent<Health>() == null)
                {
                    //TODO: damage targetHealth; when finding target health, keep target as the transform with the solid collider, but make targetHealth the health component somewhere in its hierarchy.
                }
                Debug.Log($"Enemy Attack on {target.gameObject}");
                timeOfLastAttack = Time.time;
                targetHealth.Value -= damage;
            }
        }
        //TODO: else if the colliding thing is a laser bolt, target the shooter
    }

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger)
        {
            if (collider.CompareTag("Enemy"))
            {
                visibleAliens.Add(collider.transform);
            }
            else if (collider.CompareTag("Building"))
            {
                visibleTargets.Add(collider.transform.parent);
            }
            else if (collider.CompareTag("Player"))
            {
                visibleTargets.Add(collider.transform);
            }
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerExit(Collider collider)
    {
        if (!collider.isTrigger)
        {
            if (visibleAliens.Contains(collider.transform))
            {
                visibleAliens.Remove(collider.transform);
                return;
            }

            if (visibleTargets.Contains(collider.transform))
            {
                visibleTargets.Remove(collider.transform);
                //return;
            }
        }
    }

    //TODO: if collides with and damaged by a laser bolt, target the shooter if visible.
}
