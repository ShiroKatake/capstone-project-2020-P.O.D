using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component for firing the gun part of buildings that shoot.
/// </summary>
public class TurretShooting : CollisionListener, IMessenger
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Game Objects")]
    [SerializeField] private Transform barrelMagazine;
    [SerializeField] private Transform barrelTip;

    [Header("Shooting Stats")]
    [SerializeField] private EProjectileType projectileType;
    [SerializeField] private bool targetClosest;
    [SerializeField] private float numProjectiles;
    [SerializeField] private float yAxisVariance;
    [SerializeField] private float zAxisVariance;
    [SerializeField] private float shotCooldown;

    //Non-Serialized Fields------------------------------------------------------------------------      

    [Header("Testing")]
    //Components
    private Building building;

    //Target Variables
    //[SerializeField] private bool detecting;
    [SerializeField] private List<Alien> visibleTargets;
    [SerializeField] private Alien target;
    
    //Shooting Variables
    //[SerializeField] private bool shoot;
    private float timeOfLastShot;

    //Public Properties----------------------------------------------------------------------------

    /// <summary>
    /// The target that the turret has selected.
    /// </summary>
    public Alien Target { get => target; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        building = gameObject.GetComponent<Building>();
        collisionReporters = GetCollisionReporters();
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        Reset();
    }

    /// <summary>
    /// Setup / reset code for TurretShooting.
    /// </summary>
    public void Reset()
    {
        //Debug.Log("TurretShooting.Reset()");
        MessageDispatcher.Instance.SendMessage("Alien", new Message(gameObject.name, "Turret", this.gameObject, "Dead"));
        MessageDispatcher.Instance.Unsubscribe("Turret", this);
        visibleTargets = new List<Alien>();
        timeOfLastShot = shotCooldown * -1;
        ToggleDetectionCollider(false);
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        if (building.Operational)
        {
            //CheckTargetDeaths();
            SelectTarget();

            if (/*shoot || */target != null)
            {
                Shoot();
            }
        }
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Selects a target for the turret.
    /// </summary>
    private void SelectTarget()
    {
        //Debug.Log($"Visible targets: {visibleTargets.Count}");
        switch (visibleTargets.Count)
        {
            case 0:
                if (target != null)
                {
                    target = null;
                }

                break;
            case 1:
                //Get only visible target
                if (target != visibleTargets[0])
                {
                    target = visibleTargets[0];
                }

                break;
            default:
                //Prioritise shooter
                float distance;
                float bestDistance;
                Alien bestTarget = null;

                if (targetClosest)
                {
                    //Get closest visible target
                    distance = 9999;
                    bestDistance = 9999;

                    foreach (Alien a in visibleTargets)
                    {
                        distance = Vector3.Distance(transform.position, a.transform.position);

                        if (bestTarget == null || distance < bestDistance)
                        {
                            bestTarget = a;
                            bestDistance = distance;
                        }
                    }                    
                }
                else
                {
                    //Get farthest visible target
                    distance = 0;
                    bestDistance = 0;

                    foreach (Alien a in visibleTargets)
                    {
                        distance = Vector3.Distance(transform.position, a.transform.position);

                        if (bestTarget == null || distance > bestDistance)
                        {
                            bestTarget = a;
                            bestDistance = distance;
                        }
                    }
                }

                if (bestTarget != null && target != bestTarget)
                {
                    target = bestTarget;
                }

                break;
        }
    }

    /// <summary>
    /// Shoots at the target.
    /// </summary>
    private void Shoot()
    {
        if (/*shoot || (*/target != null && Time.time - timeOfLastShot > shotCooldown)//)
        {
            //shoot = false;
            timeOfLastShot = Time.time;
            
            for(int i = 0; i < numProjectiles; i++)
            {
                Projectile projectile = ProjectileFactory.Instance.GetProjectile(projectileType, transform, barrelTip.position);
                Vector3 vector = barrelTip.position - barrelMagazine.position;

                if (yAxisVariance > 0 || zAxisVariance > 0)
                {
                    Vector3 rotationVariance = Vector3.zero;
                    rotationVariance.y = (yAxisVariance > 0 ? Random.Range(-yAxisVariance, yAxisVariance) : 0);
                    rotationVariance.z = (zAxisVariance > 0 ? Random.Range(-zAxisVariance, zAxisVariance) : 0);
                    vector += rotationVariance;
                    //Debug.Log($"Introducing variance of {rotationVariance}");               
                }

                projectile.Shoot((vector).normalized, 0);
               
            }
            AudioManager.Instance.PlaySound(AudioManager.ESound.MachineGun_Shoot, this.transform.position);
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Informs TurretShooting that it has been placed and can start shooting.
    /// </summary>
    public void Place()
    {
        ToggleDetectionCollider(true);
        MessageDispatcher.Instance.Subscribe("Turret", this);
    }

    /// <summary>
    /// Toggles the detection trigger collider on and off according to whether the building is operational or not.
    /// </summary>
    /// <param name="active">Whether the turret's detection collider should be enabled.</param>
    private void ToggleDetectionCollider(bool active)
    {
        //detecting = active; //for testing

        foreach (CollisionReporter c in collisionReporters)
        {
            c.Collider.enabled = active;
        }
    }

    /// <summary>
    /// Allows message-sending classes to deliver a message to this turret.
    /// </summary>
    /// <param name="message">The message to send to this messenger.</param>
    public void Receive(Message message)
    {
        if (message.SenderTag == "Alien" && message.MessageContents == "Dead")
        {
            Alien messenger = message.SenderObject.GetComponent<Alien>();

            if (target == messenger)
            {
                target = null;
            }

            if (visibleTargets.Contains(messenger))
            {
                visibleTargets.Remove(messenger);
            }
        }
    }

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public override void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Turret OnTriggerEnter");
        if (other.CompareTag("Alien"))
        {
            //Debug.Log("Alien entered turret trigger collider");
            Alien a = other.GetComponentInParent<Alien>();

            if (a.BodyColliders.Contains(other) && !visibleTargets.Contains(a))
            {
                //Debug.Log("Added alien body to visibleTargets");
                visibleTargets.Add(a);
            }
        }
    }

    //TODO: have a look at what's going on here; this doesn't seem to be pulling its weight, necessitating messaging to ascertain if an alien has gone out of the turret's range once it dies.
    //Keep it intact for now though, since it should still account for aliens that move out of range without dying.
    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public override void OnTriggerExit(Collider other)
    {
        //Debug.Log("Turret OnTriggerExit");
        if (other.CompareTag("Alien"))
        {
            //Debug.Log("Alien exited turret trigger collider");
            Alien a = other.GetComponentInParent<Alien>();

            if (a.BodyColliders.Contains(other) && visibleTargets.Contains(a))
            {
                //Debug.Log("Removed alien body from visibleTargets");
                visibleTargets.Remove(a);
            }
        }
    }
}
