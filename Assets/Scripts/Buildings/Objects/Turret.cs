using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A turret component for buildings that shoot.
/// </summary>
public class Turret : CollisionListener
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Game Objects")]
    [SerializeField] private Collider detectionCollider;
    [SerializeField] private Transform targeter;
    [SerializeField] private Transform turretCollider;
    [SerializeField] private Transform barrelColliderPivot;
    [SerializeField] private Transform barrelModelPivot;
    [SerializeField] private Transform barrelTip;

    [Header("Shooting Stats")]
    [SerializeField] private float shotCooldown;
    [SerializeField] private bool targetClosest;

    [Header("Aiming Stats")]
    [SerializeField] private float turretRotationSpeed;
    [SerializeField] private bool elevateBarrel;
    [SerializeField] private float barrelElevationSpeed;
    [SerializeField] private float minBarrelElevation;
    [SerializeField] private float maxBarrelElevation;

    [Header("Aiming Variance Stats")]
    [SerializeField] private bool scatteredShots;
    [SerializeField] private float numProjectiles;
    [SerializeField] private float scatteredShotsYRange;
    [SerializeField] private float scatteredShotsZRange;

    //Non-Serialized Fields------------------------------------------------------------------------      
    
    //Components
    private Building building;
    private List<CollisionReporter> collisionReporters;

    //Target Variables
    private List<Alien> visibleTargets;
    private Alien target;

    //Aiming Variables
    private bool detecting;
    private float currentTurretRotation;
    private float targetTurretRotation;
    private float currentBarrelElevation;
    private float targetBarrelElevation;

    //Shooting Variables
    private float timeOfLastShot;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        detecting = false;
        visibleTargets = new List<Alien>();
        timeOfLastShot = shotCooldown * -1;
        building = gameObject.GetComponent<Building>();
        currentTurretRotation = 0;
        currentBarrelElevation = 0;
        turretCollider.rotation = Quaternion.Euler(-90, 0, 0);
        barrelColliderPivot.rotation = Quaternion.Euler(0, 0, 0);
        barrelModelPivot.rotation = barrelColliderPivot.rotation;

        collisionReporters = new List<CollisionReporter>();
        CollisionReporter[] allCollisionReporters = GetComponentsInChildren<CollisionReporter>();

        foreach (CollisionReporter c in allCollisionReporters)
        {
            if (c.ReportsTo(this))
            {
                collisionReporters.Add(c);
            }
        }
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        if (building.Operational)
        {
            RegulateDetectionCollider();

            if (target != null)
            {
                SelectTarget();
                CalculateTargetRotationAndElevation();
                Aim();
                //Shoot();
            }
        }
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Toggles the detection trigger collider on and off according to whether the building is operational or not.
    /// </summary>
    private void RegulateDetectionCollider()
    {
        if (detecting != building.Operational)
        {
            detecting = building.Operational;

            foreach (CollisionReporter c in collisionReporters)
            {
                c.Collider.enabled = detecting;
            }
        }
    }

    /// <summary>
    /// Selects a target for the turret.
    /// </summary>
    private void SelectTarget()
    {
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
                    //Get closest visible target
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
    /// Calculates the local rotation the turret should have and the local elevation the barrel should have to aim at the target.
    /// </summary>
    private void CalculateTargetRotationAndElevation()
    {
        //Raw Rotation
        targeter.LookAt(target.transform.position);
        float rawElevation = targeter.rotation.eulerAngles.x;
        float rawRotation = targeter.rotation.eulerAngles.y;

        //Rotation
        targetTurretRotation = NormaliseAngle(rawRotation + 90);

        //Elevation
        targetBarrelElevation = (rawElevation > 90 ? 360 - rawElevation : rawElevation * -1);

        if (targetBarrelElevation > maxBarrelElevation)
        {
            targetBarrelElevation = maxBarrelElevation;
        }
        else if (targetBarrelElevation < minBarrelElevation)
        {
            targetBarrelElevation = minBarrelElevation;
        }

        //Debug.Log($"Pos: {barrelColliderPivot}, target pos: {target.transform.position}, targeter rotation: {targeter.rotation.eulerAngles}, target elevation: {targetBarrelElevation}, target rotation: {targetTurretRotation}");
    }

    /// <summary>
    /// Locally rotates the turret and elevates the barrel to aim at the target.
    /// </summary>
    private void Aim()
    {
        //Turret rotation on turret base's local z axis. All other local values remain static.
        if (currentTurretRotation != targetTurretRotation)
        {
            float deltaAngle = Mathf.DeltaAngle(currentTurretRotation, targetTurretRotation);
            float rotationDirection = Sign(deltaAngle);
            deltaAngle = Magnitude(deltaAngle);
            float fixedUpdateRotation = turretRotationSpeed * Time.fixedDeltaTime;

            currentTurretRotation += rotationDirection * Mathf.Min(deltaAngle, fixedUpdateRotation);
            currentTurretRotation = NormaliseAngle(currentTurretRotation);
            turretCollider.localRotation = Quaternion.Euler(-90, 0, currentTurretRotation); 
        }

        //Barrel pivoting on barrel pivot's local y axis. All other local values remain 0.
        if (currentBarrelElevation != targetBarrelElevation)
        {
            float deltaAngle = Mathf.DeltaAngle(currentBarrelElevation, targetBarrelElevation);
            float pivotDirection = Sign(deltaAngle);
            deltaAngle = Magnitude(deltaAngle);
            float fixedUpdatePivot = barrelElevationSpeed * Time.fixedDeltaTime;

            currentBarrelElevation += pivotDirection * Mathf.Min(deltaAngle, fixedUpdatePivot);
            barrelColliderPivot.localRotation = Quaternion.Euler(0, currentBarrelElevation, 0);
            barrelModelPivot.localRotation = barrelColliderPivot.localRotation;
        }
    }

    /// <summary>
    /// Shoots at the target.
    /// </summary>
    private void Shoot()
    {
        if (target != null && Time.time - timeOfLastShot > shotCooldown)
        {
            timeOfLastShot = Time.time;

            //Shoot
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public override void OnCollisionEnter(Collision collision)
    {
        //if (active)
        //{
        //    Debug.Log($"Building {building.Id} OnCollisionEnter()");
        //}
    }

    /// <summary>
    /// OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public override void OnCollisionExit(Collision collision)
    {
        //if (active)
        //{
        //    Debug.Log($"Building {building.Id} OnCollisionExit()");
        //}
    }

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public override void OnCollisionStay(Collision collision)
    {
        //if (active)
        //{
        //    Debug.Log($"Building {building.Id} OnCollisionStay()");
        //}
    }

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Alien"))
        {
            visibleTargets.Add(other.GetComponentInParent<Alien>());
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Alien"))
        {
            visibleTargets.Remove(other.GetComponentInParent<Alien>());
        }
    }

    /// <summary>
    /// OnTriggerStay is called almost all the frames for every Collider other that is touching the trigger. The function is on the physics timer so it won't necessarily run every frame.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public override void OnTriggerStay(Collider other)
    {
        //if (active)
        //{
        //    Debug.Log($"Building {building.Id} OnTriggerStay()");
        //}
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Returns the magnitude of a number.
    /// </summary>
    /// <param name="num">The number to calculate the magnitude of.</param>
    /// <returns>The magnitude of the number.</returns>
    private float Magnitude (float num)
    {
        if (num < 0)
        {
            num *= -1;
        }

        return num;
    }

    /// <summary>
    /// Converts the provided angle to an angle between 0 degrees and 360 degrees
    /// </summary>
    /// <param name="angle">The raw angle.</param>
    /// <returns>The normalised angle.</returns>
    private float NormaliseAngle(float angle)
    {
        while (angle > 360)
        {
            angle -= 360;
        }

        while (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    /// <summary>
    /// Returns the sign of a number, i.e. +1 if it's positive or 0, and -1 if it's negative.
    /// </summary>
    /// <param name="num">The number to determine the sign of.</param>
    /// <returns>The sign (+1 or -1) of the number.</returns>
    private float Sign(float num)
    {
        return (num < 0 ? -1 : 1);
    }
}
