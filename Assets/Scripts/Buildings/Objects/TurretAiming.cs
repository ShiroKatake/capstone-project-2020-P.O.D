using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base component class for aiming the gun part of buildings that shoot.
/// </summary>
public class TurretAiming : MonoBehaviour //CollisionListener
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    
    
    [Header("Rotation Aiming Stats")]
    [SerializeField] protected float rotationSpeed;

    [Header("Elevation Aiming Stats")]
    [SerializeField] protected float elevationSpeed;
    [SerializeField] protected float minBarrelElevation;
    [SerializeField] protected float maxBarrelElevation;

    [Header("Aiming Offsets")]
    [SerializeField] protected Vector3 crawlerPositionOffset;
    //[SerializeField] protected Vector3 targetHornetPositionOffset;
    [SerializeField] protected Vector3 rotationColliderOffset;
    [SerializeField] protected Vector3 rotationModelCounterOffset;
    [SerializeField] protected Vector3 elevationColliderOffset;
    [SerializeField] protected Vector3 elevationModelCounterOffset;

    //Non-Serialized Fields------------------------------------------------------------------------      

    //Components
    protected Building building;
    protected TurretShooting shooter;

    //Aiming Variables
    //[Header("TurretAiming Testing")]
    //[SerializeField] protected bool detecting;
    /*[SerializeField]*/ protected float currentTurretRotation;
    /*[SerializeField]*/ protected float targetTurretRotation;
    /*[SerializeField]*/ protected float currentBarrelElevation;
    /*[SerializeField]*/ protected float targetBarrelElevation;
    //[SerializeField] protected Transform target;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected virtual void Awake()
    {
        building = gameObject.GetComponent<Building>();
        shooter = gameObject.GetComponent<TurretShooting>();
        //collisionReporters = GetCollisionReporters();
    }

    /// <summary>
    /// Setup / resetting code used by TurretAiming and all of its child classes.
    /// </summary>
    public virtual void Reset()
    {
        //Debug.Log("TurretAiming.Reset()");
        currentTurretRotation = 0;
        currentBarrelElevation = 0;
        targetTurretRotation = 0;
        targetBarrelElevation = 0;
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Calculates the local rotation the turret should have and the local elevation the barrel should have to aim at the target.
    /// </summary>
    protected virtual void CalculateRotationAndElevation()
    {
        Debug.LogError("Not Implemented");
    }

    /// <summary>
    /// Locally rotates the turret and elevates the barrel to aim at the target.
    /// </summary>
    protected virtual void Aim()
    {
        Debug.LogError("Not Implemented");
    }

    /// <summary>
    /// Restricts the elevation of the turret's barrel to within predefined limits.
    /// </summary>
    protected virtual void ClampElevation()
    {
        if (targetBarrelElevation > maxBarrelElevation)
        {
            targetBarrelElevation = maxBarrelElevation;
        }
        else if (targetBarrelElevation < minBarrelElevation)
        {
            targetBarrelElevation = minBarrelElevation;
        }
    }
}
