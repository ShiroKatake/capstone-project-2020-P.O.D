using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A turret component for buildings that shoot.
/// </summary>
public class Turret : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Game Objects")]
    [SerializeField] private Transform turretCollider;
    [SerializeField] private Transform barrelColliderPivot;
    [SerializeField] private Transform barrelModelPivot;
    [SerializeField] private Transform barrelTip;

    [Header("Shooting Stats")]
    [SerializeField] private float shotCooldown;
    [SerializeField] private bool targetClosest;

    [Header("Aiming Stats")]
    [SerializeField] private float turretRotationSpeed;
    [SerializeField] private bool pivotBarrel;
    [SerializeField] private float barrelPivotSpeed;
    [SerializeField] private float minBarrelPivotAngle;
    [SerializeField] private float maxBarrelPivotAngle;

    [Header("Aiming Variance Stats")]
    [SerializeField] private bool scatteredShots;
    [SerializeField] private float numProjectiles;
    [SerializeField] private float scatteredShotsYRange;
    [SerializeField] private float scatteredShotsZRange;

    //Non-Serialized Fields------------------------------------------------------------------------      
    
    [Header("Testing")]

    private List<Alien> visibleTargets;
    private Alien target;
    [SerializeField] private bool aim;
    //[SerializeField] private Vector3 targetPos;
    [SerializeField] private float currentTurretRotation;
    [SerializeField] private float targetTurretRotation;
    [SerializeField] private float currentBarrelPivotAngle;
    [SerializeField] private float targetBarrelPivotAngle;
    private float timeOfLastShot;
    private Building building;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        visibleTargets = new List<Alien>();
        timeOfLastShot = shotCooldown * -1;
        building = gameObject.GetComponent<Building>();
        currentTurretRotation = 0;
        currentBarrelPivotAngle = 0;
        turretCollider.rotation = Quaternion.Euler(-90, 0, 0);
        barrelColliderPivot.rotation = Quaternion.Euler(0, 0, 0);
        barrelModelPivot.rotation = barrelColliderPivot.rotation;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        if (building.Operational && aim /*&& target != null*/)
        {
            //SelectTarget();
            CalculateTargetRotationAndPivotAngle();
            Aim();
            //Shoot();
        }
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

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
    /// Calculates the rotation the turret should have and the pivot angle the barrel should have to aim at the target.
    /// </summary>
    private void CalculateTargetRotationAndPivotAngle()
    {
        //TODO: calculate based on target position. Use angle from barrelColliderPivot? Is there a way to lock the x axis out of that calculation?
        if (targetBarrelPivotAngle > maxBarrelPivotAngle)
        {
            targetBarrelPivotAngle = maxBarrelPivotAngle;
        }
        else if (targetBarrelPivotAngle < minBarrelPivotAngle)
        {
            targetBarrelPivotAngle = minBarrelPivotAngle;
        }
    }

    /// <summary>
    /// Rotates the turret and pivots the barrel to aim at the target.
    /// </summary>
    private void Aim()
    {
        //Turret Rotation on turret base's z axis. All other values remain static.
        if (currentTurretRotation != targetTurretRotation)
        {
            float deltaAngle = Mathf.DeltaAngle(currentTurretRotation, targetTurretRotation);
            float rotationDirection = Sign(deltaAngle);
            deltaAngle = Magnitude(deltaAngle);
            float fixedUpdateRotation = turretRotationSpeed * Time.fixedDeltaTime;

            currentTurretRotation += rotationDirection * Mathf.Min(deltaAngle, fixedUpdateRotation);
            currentTurretRotation = (
                  currentTurretRotation > 360 ? currentTurretRotation - 360  
                : currentTurretRotation < 0   ? currentTurretRotation + 360
                :                               currentTurretRotation);
            turretCollider.rotation = Quaternion.Euler(-90, 0, currentTurretRotation); 
        }

        //Barrel Pivoting on barrel's y axis. All other values remain 0.
        if (currentBarrelPivotAngle != targetBarrelPivotAngle)
        {
            float deltaAngle = Mathf.DeltaAngle(currentBarrelPivotAngle, targetBarrelPivotAngle);
            float pivotDirection = Sign(deltaAngle);
            deltaAngle = Magnitude(deltaAngle);
            float fixedUpdatePivot = barrelPivotSpeed * Time.fixedDeltaTime;

            currentBarrelPivotAngle += pivotDirection * Mathf.Min(deltaAngle, fixedUpdatePivot);
            barrelColliderPivot.rotation = Quaternion.Euler(0, 0, -1 * currentBarrelPivotAngle);
            barrelModelPivot.rotation = barrelColliderPivot.rotation;
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
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
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
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Alien"))
        {
            visibleTargets.Remove(other.GetComponentInParent<Alien>());
        }
    }

    //Utility Methods

    private float Magnitude (float num)
    {
        if (num < 0)
        {
            num *= -1;
        }

        return num;
    }

    private float Sign(float num)
    {
        return (num < 0 ? -1 : 1);
    }
}
