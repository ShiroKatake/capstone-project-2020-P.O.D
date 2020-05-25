//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// A turret component for buildings that shoot.
///// </summary>
//public class TurretAiming : CollisionListener
//{
//    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

//    //Serialized Fields----------------------------------------------------------------------------                                                    

//    [Header("Game Objects")]
//    [SerializeField] private Transform rotationTargeter;
//    [SerializeField] private Transform elevationTargeter;
//    [SerializeField] private Transform barrelBaseColliderPivot;
//    [SerializeField] private Transform barrelBaseModelPivot;
//    [SerializeField] private Transform barrelColliderPivot;
//    [SerializeField] private Transform barrelModelPivot;

//    [Header("Rotation Aiming Stats")]
//    [SerializeField] private float rotationSpeed;
//    [SerializeField] private Vector3 rotationOffset;

//    [Header("Elevation Aiming Stats")]
//    [SerializeField] private float elevationSpeed;
//    [SerializeField] private float minBarrelElevation;
//    [SerializeField] private float maxBarrelElevation;
//    [SerializeField] private Vector3 elevationOffset;

//    //Non-Serialized Fields------------------------------------------------------------------------      

//    [Header("Testing")]
//    [SerializeField] private bool shoot;
//    //Components
//    private Building building;

//    //Target Variables
//    [SerializeField] private List<Alien> visibleTargets;
//    [SerializeField] private Alien target;

//    //Aiming Variables
//    [SerializeField] private bool detecting;
//    [SerializeField] private float currentTurretRotation;
//    [SerializeField] private float targetTurretRotation;
//    [SerializeField] private float currentBarrelElevation;
//    [SerializeField] private float targetBarrelElevation;

//    //Shooting Variables
//    private float timeOfLastShot;

//    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

//    /// <summary>
//    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
//    /// Awake() runs before Start().
//    /// </summary>
//    private void Awake()
//    {
//        detecting = false;
//        visibleTargets = new List<Alien>();
//        timeOfLastShot = shotCooldown * -1;
//        building = gameObject.GetComponent<Building>();
//        currentTurretRotation = 0;
//        currentBarrelElevation = 0;
//        barrelBaseColliderPivot.rotation = Quaternion.Euler(rotationOffset);
//        barrelBaseModelPivot.rotation = barrelBaseColliderPivot.rotation;
//        barrelColliderPivot.rotation = Quaternion.Euler(elevationOffset);
//        barrelModelPivot.rotation = barrelColliderPivot.rotation;
//        collisionReporters = GetCollisionReporters();
//    }

//    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

//    /// <summary>
//    /// FixedUpdate() is run at a fixed interval independant of framerate.
//    /// </summary>
//    private void FixedUpdate()
//    {
//        if (building.Operational)
//        {            
//            CalculateTargetRotationAndElevation();
//            Aim();
//        }
//    }

//    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

//    /// <summary>
//    /// Calculates the local rotation the turret should have and the local elevation the barrel should have to aim at the target.
//    /// </summary>
//    private void CalculateTargetRotationAndElevation()
//    {
//        //Variables
//        float rawRotation;
//        float rawElevation;

//        //Rotation
//        rotationTargeter.LookAt(target.transform.position);
//        rawRotation = rotationTargeter.rotation.eulerAngles.y;
//        targetTurretRotation = NormaliseAngle(rawRotation + 90);

//        //Elevation
//        if (rotationTargeter == elevationTargeter)
//        {
//            rawElevation = rotationTargeter.rotation.eulerAngles.x;
//        }
//        else
//        {
//            elevationTargeter.LookAt(target.transform.position);
//            rawElevation = elevationTargeter.rotation.eulerAngles.x;
//        }

//        targetBarrelElevation = (rawElevation > 90 ? 360 - rawElevation : rawElevation * -1);

//        if (targetBarrelElevation > maxBarrelElevation)
//        {
//            targetBarrelElevation = maxBarrelElevation;
//        }
//        else if (targetBarrelElevation < minBarrelElevation)
//        {
//            targetBarrelElevation = minBarrelElevation;
//        }

//        //Debug.Log($"Pos: {barrelColliderPivot}, target pos: {target.transform.position}, targeter rotation: {targeter.rotation.eulerAngles}, target elevation: {targetBarrelElevation}, target rotation: {targetTurretRotation}");
//    }

//    /// <summary>
//    /// Locally rotates the turret and elevates the barrel to aim at the target.
//    /// </summary>
//    private void Aim()
//    {
//        //Turret rotation on turret base's local z axis. All other local values remain static.
//        if (currentTurretRotation != targetTurretRotation)
//        {
//            float deltaAngle = Mathf.DeltaAngle(currentTurretRotation, targetTurretRotation);
//            float rotationDirection = Sign(deltaAngle);
//            deltaAngle = Magnitude(deltaAngle);
//            float fixedUpdateRotation = rotationSpeed * Time.fixedDeltaTime;

//            currentTurretRotation += rotationDirection * Mathf.Min(deltaAngle, fixedUpdateRotation);
//            currentTurretRotation = NormaliseAngle(currentTurretRotation);
//            barrelBaseColliderPivot.localRotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, currentTurretRotation + rotationOffset.z);


//            barrelBaseModelPivot.localRotation = barrelBaseColliderPivot.localRotation;
//        }

//        //Barrel pivoting on barrel pivot's local y axis. All other local values remain 0.
//        if (currentBarrelElevation != targetBarrelElevation)
//        {
//            float deltaAngle = Mathf.DeltaAngle(currentBarrelElevation, targetBarrelElevation);
//            float pivotDirection = Sign(deltaAngle);
//            deltaAngle = Magnitude(deltaAngle);
//            float fixedUpdatePivot = elevationSpeed * Time.fixedDeltaTime;

//            currentBarrelElevation += pivotDirection * Mathf.Min(deltaAngle, fixedUpdatePivot);
//            barrelColliderPivot.localRotation = Quaternion.Euler(elevationOffset.x, currentBarrelElevation + elevationOffset.y, elevationOffset.z);
//            barrelModelPivot.localRotation = barrelColliderPivot.localRotation;
//        }
//    }

//    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

//    /// <summary>
//    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
//    /// </summary>
//    /// <param name="other">The other Collider involved in this collision.</param>
//    public override void OnTriggerEnter(Collider other)
//    {
//        //Debug.Log("Turret OnTriggerEnter");
//        if (other.CompareTag("Alien"))
//        {
//            //Debug.Log("Alien entered turret trigger collider");
//            Alien a = other.GetComponentInParent<Alien>();

//            if (other == a.BodyCollider && !visibleTargets.Contains(a))
//            {
//                //Debug.Log("Added alien body to visibleTargets");
//                visibleTargets.Add(a);
//            }
//        }
//    }

//    /// <summary>
//    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
//    /// </summary>
//    /// <param name="other">The other Collider involved in this collision.</param>
//    public override void OnTriggerExit(Collider other)
//    {
//        //Debug.Log("Turret OnTriggerExit");
//        if (other.CompareTag("Alien"))
//        {
//            //Debug.Log("Alien exited turret trigger collider");
//            Alien a = other.GetComponentInParent<Alien>();

//            if (other == a.BodyCollider && visibleTargets.Contains(a))
//            {
//                //Debug.Log("Removed alien body from visibleTargets");
//                visibleTargets.Remove(a);
//            }
//        }
//    }

//    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------

//    /// <summary>
//    /// Returns the magnitude of a number.
//    /// </summary>
//    /// <param name="num">The number to calculate the magnitude of.</param>
//    /// <returns>The magnitude of the number.</returns>
//    private float Magnitude(float num)
//    {
//        if (num < 0)
//        {
//            num *= -1;
//        }

//        return num;
//    }

//    /// <summary>
//    /// Converts the provided angle to an angle between 0 degrees and 360 degrees
//    /// </summary>
//    /// <param name="angle">The raw angle.</param>
//    /// <returns>The normalised angle.</returns>
//    private float NormaliseAngle(float angle)
//    {
//        while (angle > 360)
//        {
//            angle -= 360;
//        }

//        while (angle < 0)
//        {
//            angle += 360;
//        }

//        return angle;
//    }

//    /// <summary>
//    /// Returns the sign of a number, i.e. +1 if it's positive or 0, and -1 if it's negative.
//    /// </summary>
//    /// <param name="num">The number to determine the sign of.</param>
//    /// <returns>The sign (+1 or -1) of the number.</returns>
//    private float Sign(float num)
//    {
//        return (num < 0 ? -1 : 1);
//    }
//}
