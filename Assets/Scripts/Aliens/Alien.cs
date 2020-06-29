using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo class for enemies.
/// </summary>
public class Alien : MonoBehaviour, IMessenger
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Components")]
    [SerializeField] private List<Collider> bodyColliders;

    [Header("Stats")] 
    [SerializeField] private int id;
    [SerializeField] private float speed;
    [SerializeField] private float turningSpeed;
    [SerializeField] private float hoverHeight;
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;

    //Non-Serialized Fields------------------------------------------------------------------------
    [Header("Testing")]
    //Componenets
    private List<Collider> colliders;
    private Health health;
    private Rigidbody rigidbody;

    //Movement
    private bool moving;
    [SerializeField] private float zRotation;

    //Turning
    private Quaternion oldRotation;
    private Quaternion targetRotation;
    private float slerpProgress;
    
    //Targeting
    //private CryoEgg CryoEgg;
    private List<Transform> visibleAliens;
    private List<Transform> visibleTargets;
    [SerializeField] private Transform target;
    [SerializeField] private Health targetHealth;
    [SerializeField] private Size targetSize;
    [SerializeField] private string shotByName;
    [SerializeField] private Transform shotByTransform;
    private float timeOfLastAttack;

    //[SerializeField] private bool conductSweepTesting;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The colliders that comprise the alien's body.
    /// </summary>
    public List<Collider> BodyColliders { get => bodyColliders; }

    /// <summary>
    /// Alien's Health component.
    /// </summary>
    public Health Health { get => health; }

    ///// <summary>
    ///// Alien's unique ID number. Id should only be set by Alien.Setup().
    ///// </summary>
    //public int Id { get => id; }

    ///// <summary>
    ///// Whether or not the alien is moving.
    ///// </summary>
    //public bool Moving { get => moving; set => moving = value; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
        colliders = new List<Collider>(GetComponents<Collider>());
        health = GetComponent<Health>();
        rigidbody = GetComponent<Rigidbody>();
        zRotation = transform.rotation.eulerAngles.z;

        visibleAliens = new List<Transform>();
        visibleTargets = new List<Transform>();
        moving = false;
    }

    ///// <summary>
    ///// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    ///// Start() runs after Awake().
    ///// </summary>
    //void Start()
    //{
    //    CryoEgg = CryoEgg.Instance;
    //}

    /// <summary>
    /// Prepares the Alien to chase its targets when AlienFactory puts it in the world. 
    /// </summary>
    public void Setup(int id)
    {
        this.id = id;
        gameObject.name = $"Alien {id}";
        health.Reset();

        //Debug.Log($"{this}.Setup(), CryoEgg is {CryoEgg.Instance}");
        //Debug.Log($"CryoEgg.ColliderTransform is {CryoEgg.Instance.ColliderTransform}, CryoEgg.Health is {CryoEgg.Instance.Health}, CryoEgg.Size is {CryoEgg.Instance.Size}");

        target = CryoEgg.Instance.ColliderTransform;
        targetHealth = CryoEgg.Instance.Health;
        timeOfLastAttack = attackCooldown * -1;
        moving = true;
        MessageDispatcher.Instance.Subscribe("Alien", this);

        //Rotate to face the Cryo egg
        Vector3 targetRotation = CryoEgg.Instance.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetRotation);

        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        if (moving)
        {
            CheckHealth();
            SelectTarget();
            Look();
            Move();
        }
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
        switch (visibleTargets.Count)
        {
            case 0:
                //Target Cryo egg
                if (target != CryoEgg.Instance.transform)
                {
                    target = CryoEgg.Instance.ColliderTransform;
                    targetHealth = CryoEgg.Instance.Health;
                    targetSize = CryoEgg.Instance.Size;
                }

                break;
            case 1:
                //Get only visible target
                if (target != visibleTargets[0])
                {
                    target = visibleTargets[0];
                    targetHealth = target.GetComponentInParent<Health>();   //Gets Health from target or any of its parents that has it.
                    targetSize = target.GetComponentInParent<Size>();   //Gets Radius from target or any of its parents that has it.
                }

                break;
            default:
                //Prioritise shooter
                if (shotByTransform != null && visibleTargets.Contains(shotByTransform))
                {
                    target = shotByTransform;
                    targetHealth = target.GetComponentInParent<Health>();   //Gets Health from target or any of its parents that has it.
                    targetSize = target.GetComponentInParent<Size>();   //Gets Radius from target or any of its parents that has it.
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
                        targetSize = target.GetComponentInParent<Size>();   //Gets Radius from target or any of its parents that has it.
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
        Vector3 newRotation = PositionAtSameHeight(target.position) - transform.position;

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
    /// Gets the position of a target as if it were at the same height as the alien. 
    /// </summary>
    /// <param name="targetPos">The target's position.</param>
    /// <returns>The target's position if it was at the same height as the alien.</returns>
    private Vector3 PositionAtSameHeight(Vector3 targetPos)
    {
        return new Vector3(targetPos.x, transform.position.y, targetPos.z);
    }

    /// <summary>
    /// Moves alien.
    /// </summary>
    private void Move()
    {
        if (Vector3.Distance(transform.position, PositionAtSameHeight(target.position)) > attackRange + targetSize.Radius)
        {
            RaycastHit hit;
            float movement = speed * Time.fixedDeltaTime;

            //if (conductSweepTesting && rigidbody.SweepTest(transform.forward, out hit, movement)) //Check if would collide with another object
            //{
            //    Debug.Log($"{this}.Move() sweep test, hit collider's game object is {hit.collider.gameObject}, tag is {hit.collider.tag}");

            //    if (hit.collider.tag == "Alien" && hit.rigidbody != rigidbody)
            //    {
            //        return;
            //    }
            //}

            transform.Translate(new Vector3(0, 0, movement));
            //Vector3 translatedPos = transform.position;

            LayerMask mask = LayerMask.GetMask("Ground");

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 25, mask))
            {
                float height = (transform.position - hit.point).y;

                //Toggle gravity if something has pushed the alien up above hoverHeight
                if (rigidbody.useGravity)
                {
                    if (height <= hoverHeight)
                    {
                        transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z);
                        rigidbody.useGravity = false;
                        rigidbody.drag = 100;
                        rigidbody.mass = 0;
                    }
                }
                else
                {
                    if (height > hoverHeight)
                    {
                        rigidbody.useGravity = true;
                        rigidbody.drag = 0;
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.mass = 1000;
                    }
                }
            }
        }
        else if (Time.time - timeOfLastAttack > attackCooldown)
        {
            timeOfLastAttack = Time.time;
            targetHealth.Value -= damage;
            AudioManager.Instance.PlaySound(AudioManager.Sound.Damage_To_Building, this.transform.position); //need to add a check to see what it is attacking if we want to diversify sound portfolio, non essencial
            //TODO: trigger attack animation
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Allows message-sending classes to deliver a message to this alien.
    /// </summary>
    /// <param name="message">The message to send to this messenger.</param>
    public void Receive(Message message)
    {
        if (message.SenderTag == "Turret" && message.MessageContents == "Dead")
        {
            Transform messenger = message.SenderObject.transform;

            if (shotByTransform == messenger)
            {
                shotByName = "";
                shotByTransform = null;
            }

            if (visibleTargets.Contains(messenger))
            {
                visibleTargets.Remove(messenger);
            }
        }
    }

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
    /// Resets the alien to its inactive state.
    /// </summary>
    public void Reset()
    {
        MessageDispatcher.Instance.SendMessage("Turret", new Message(gameObject.name, "Alien", this.gameObject, "Dead"));
        MessageDispatcher.Instance.Unsubscribe("Alien", this);
        moving = false;
        shotByName = "";
        shotByTransform = null;
        visibleTargets.Clear();
        visibleAliens.Clear();
        target = null;

        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (moving)
    //    {
    //        Debug.Log($"{this} collided with {collision.gameObject}, tag is {collision.gameObject.tag}, force applied is {collision.impulse}");
    //    }
    //}

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (/*bodyCollider.enabled &&*/ !other.isTrigger)
        {
            if (other.CompareTag("Alien"))
            {
                visibleAliens.Add(other.transform);
            }
            else if (other.CompareTag("Building"))
            {
                visibleTargets.Add(other.transform.parent);
            }
            else if (other.CompareTag("Player"))
            {
                visibleTargets.Add(other.transform);
            }
            else if (other.CompareTag("Projectile"))
            {
                Debug.Log("Alien.OnTriggerEnter; Alien hit by a projectile");
                Projectile projectile = other.GetComponent<Projectile>();
                shotByTransform = projectile.Owner.GetComponentInChildren<Collider>().transform;
            }
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerExit(Collider other)
    {
        if (/*bodyCollider.enabled && */!other.isTrigger)
        {
            if (visibleAliens.Contains(other.transform))
            {
                visibleAliens.Remove(other.transform);
            }
            else if (visibleTargets.Contains(other.transform))
            {
                visibleTargets.Remove(other.transform);
            }
        }
    }
}
