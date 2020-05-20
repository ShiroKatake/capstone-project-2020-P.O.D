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
    [SerializeField] private Transform barrelCollider;
    [SerializeField] private Transform turretModel;
    [SerializeField] private Transform barrelModel;
    [SerializeField] private Transform barrelTip;

    [Header("Shooting Stats")]
    [SerializeField] private float shotCooldown;
    [SerializeField] private bool targetClosest;

    [Header("Aiming Stats")]
    [SerializeField] private float turretRotationSpeed;
    [SerializeField] private bool pivotBarrel;
    [SerializeField] private float barrelPivotSpeed;
    [SerializeField] private float barrelPivotMinAngle;
    [SerializeField] private float barrelPivotMaxAngle;

    [Header("Aiming Variance Stats")]
    [SerializeField] private bool scatteredShots;
    [SerializeField] private float numProjectiles;
    [SerializeField] private float scatteredShotsYRange;
    [SerializeField] private float scatteredShotsZRange;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<Alien> visibleTargets;
    private Alien target;
    private Quaternion targetRotation;
    private Quaternion oldRotation;
    private float turretRotationSlerpProgress;
    private float barrelPivotSlerpProgress;
    private float timeOfLastShot;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        visibleTargets = new List<Alien>();
        timeOfLastShot = shotCooldown * -1;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        SelectTarget();
        Aim();
        Shoot();
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
    /// Rotates the turret and pivots the barrel to aim at the target.
    /// </summary>
    private void Aim()
    {
        if (target != null)
        {
            Vector3 newRotation = target.transform.position - barrel.position;

            if (newRotation != targetRotation.eulerAngles)
            {
                oldRotation = transform.rotation;
                targetRotation = Quaternion.LookRotation(newRotation);
                turretRotationSlerpProgress = 0f;
                barrelPivotSlerpProgress = 0f;
            }

            //Turret Rotation
            if (turretRotationSlerpProgress < 1)
            {
                turretRotationSlerpProgress = Mathf.Min(1, turretRotationSlerpProgress + turretRotationSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.LookRotation(Vector3.Slerp(
                    new Vector3(turret.rotation.eulerAngles.x, oldRotation.eulerAngles.y, turret.rotation.eulerAngles.z),
                    new Vector3(turret.rotation.eulerAngles.x, targetRotation.eulerAngles.y, turret.rotation.eulerAngles.z),
                    turretRotationSlerpProgress));
            }

            //Barrel Pivoting
            if (pivotBarrel && barrelPivotSlerpProgress < 1)
            {
                barrelPivotSlerpProgress = Mathf.Min(1, barrelPivotSlerpProgress + barrelPivotSpeed * Time.fixedDeltaTime);
                Vector3 rotation = Vector3.Slerp(
                    new Vector3(barrel.rotation.eulerAngles.x, oldRotation.eulerAngles.z, barrel.rotation.eulerAngles.z),
                    new Vector3(barrel.rotation.eulerAngles.x, targetRotation.eulerAngles.z, barrel.rotation.eulerAngles.z),
                    barrelPivotSlerpProgress);
                
                if (rotation.y < barrelPivotMinAngle)
                {
                    rotation.y = barrelPivotMinAngle;
                }
                else if (rotation.y > barrelPivotMaxAngle)
                {
                    rotation.y = barrelPivotMaxAngle;
                }

                transform.rotation = Quaternion.LookRotation(rotation);
            }
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
}
