using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo class for enemies.
/// </summary>
public class Alien : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Alien Stats")] 
    [SerializeField] private int id;
    [SerializeField] private float speed;
    [SerializeField] private float turningSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;

    //Non-Serialized Fields------------------------------------------------------------------------
    [Header("Testing")]
    //Componenets
    private Health health;
    private Rigidbody rigidbody;

    //Movement
    private bool moving;
    [SerializeField] private float hoverHeight;
    [SerializeField] private float zRotation;

    //Turning
    private Quaternion oldRotation;
    private Quaternion targetRotation;
    private float slerpProgress;
    
    //Targeting
    private Building cryoEgg;
    private List<Transform> visibleAliens;
    private List<Transform> visibleTargets;
    [SerializeField] private Transform target;
    [SerializeField] private Health targetHealth;
    [SerializeField] private string shotByName;
    [SerializeField] private Transform shotByTransform;
    private float timeOfLastAttack;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Alien's Health component.
    /// </summary>
    public Health Health { get => health; }

    /// <summary>
    /// Whether or not the alien is moving.
    /// </summary>
    public bool Moving { get => moving; set => moving = value; }

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// Alien's unique ID number. Id should only be set by Alien.Setup().
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
            gameObject.name = $"Alien {id}";
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

        hoverHeight = transform.position.y;
        zRotation = transform.rotation.eulerAngles.z;

        visibleAliens = new List<Transform>();
        visibleTargets = new List<Transform>();
    }

    /// <summary>
    /// Prepares the Alien to chase its targets when AlienFactory puts it in the world. 
    /// </summary>
    public void Setup(int id)
    {
        Id = id;
        health.Reset();
        target = cryoEgg.GetComponentInChildren<Collider>().transform;
        targetHealth = cryoEgg.Health;
        timeOfLastAttack = attackCooldown * -1;

        //Rotate to face the cryo egg
        Vector3 targetRotation = cryoEgg.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetRotation);
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    //private void Update()
    //{
        
    //}

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        CheckHealth();
        SelectTarget();
        Look();
        Move();
    }

    //Recurring Methods (FixedUpdate())-------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Checks if alien has 0 health, destroying it if it has.
    /// </summary>
    private void CheckHealth()
    {
        if (health.IsDead())
        {
            AlienFactory.Instance.DestroyAlien(this);
        }
    }

    /// <summary>
    /// Selects the most appropriate target for the alien.
    /// </summary>
    private void SelectTarget()
    {
        //Check shooter is alive
        if (shotByTransform != null)
        {
            foreach (Message msg in MessageBoard.Instance.Messages)
            {
                if (msg.SenderName == shotByName && msg.MessageContents == "Dead")
                {
                    shotByName = "";
                    shotByTransform = null;
                }
            }
        }
        else if (shotByName != "")
        {
            shotByName = "";
        }

        switch (visibleTargets.Count)
        {
            case 0:
                //Target cryo egg
                if (target != cryoEgg.transform)
                {
                    target = cryoEgg.GetComponentInChildren<Collider>().transform;
                    targetHealth = cryoEgg.Health;
                }

                break;
            case 1:
                //Get only visible target
                if (target != visibleTargets[0])
                {
                    target = visibleTargets[0];
                    targetHealth = target.GetComponentInParent<Health>();   //Gets Health from target or any of its parents that has it.
                }

                break;
            default:
                //Prioritise shooter
                if (shotByTransform != null && visibleTargets.Contains(shotByTransform))
                {
                    target = shotByTransform;
                    targetHealth = target.GetComponentInParent<Health>();   //Gets Health from target or any of its parents that has it.
                }
                else
                {
                    //Get closest visible target
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
                        targetHealth = target.GetComponentInParent<Health>();   //Gets Health from target or any of its parents that has it.
                    }
                }

                break;
        }
    }

    /// <summary>
    /// Alien uses input information to determine which direction it should be facing
    /// </summary>
    private void Look()
    {
        //TODO: swarm-based looking behaviour
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
    /// Moves alien.
    /// </summary>
    private void Move()
    {
        transform.Translate(new Vector3(0, 0, speed * Time.fixedDeltaTime));

        //Fly up if below hover height
        if (transform.position.y < hoverHeight)
        {
            if (rigidbody.useGravity)
            {
                rigidbody.useGravity = false;
            }

            transform.Translate(new Vector3(0, Mathf.Min(hoverHeight - transform.position.y, speed * 0.5f * Time.fixedDeltaTime, 0)));
        }
        //Activate gravity if above hover height
        else if (transform.position.y > hoverHeight)
        {
            if (!rigidbody.useGravity)   //TODO: account for terrain pushing the alien up, if it can move up hills?
            {
                rigidbody.useGravity = true;
            }
        }
        //Disable gravity if at hover height
        else
        {
            if (rigidbody.useGravity)
            {
                transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z);
                rigidbody.useGravity = false;
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Registers with an alien the name and transform of an entity that shot it.
    /// </summary>
    /// <param name="name">The name of the entity that shot the alien.</param>
    /// <param name="transform">The transform of the entity that shot the alien.</param>
    public void ShotBy(string name, Transform transform)
    {
        shotByName = name;
        shotByTransform = transform;
    }

    /// <summary>
    /// The transform of the player or building the alien was shot by most recently.
    /// </summary>
    public Transform ShotByTransform { get => shotByTransform; set => shotByTransform = value; }

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.isTrigger && (collision.collider.CompareTag("Building") || collision.collider.CompareTag("Player")))
        {
            if (Time.time - timeOfLastAttack > attackCooldown)
            {
                timeOfLastAttack = Time.time;
                targetHealth.Value -= damage;
            }
        }
        //TODO: if made contact with target and target is a building, step back a smidge and attack, so that OnCollisionStay is not called every single frame. For player, check if within attack range to verify that the alien can still attack them?
    }

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger)
        {
            if (collider.CompareTag("Alien"))
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
            else if (collider.CompareTag("Projectile"))
            {
                Debug.Log("Alien.OnTriggerEnter; Alien hit by a projectile");
                Projectile projectile = collider.GetComponent<Projectile>();
                shotByTransform = projectile.Owner.GetComponentInChildren<Collider>().transform;
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
}
