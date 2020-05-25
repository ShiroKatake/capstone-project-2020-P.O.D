using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base component class for aiming the gun part of shotgun turrets.
/// </summary>
public class ShotgunTurretAiming : TurretAiming
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    
    
    [Header("Aiming Offsets")]
    [SerializeField] private Vector3 rotationColliderOffset;
    [SerializeField] private Vector3 rotationModelCounterOffset;
    [SerializeField] private Vector3 elevationColliderOffset;
    [SerializeField] private Vector3 elevationModelCounterOffset;

    [Header("Game Objects")]
    [SerializeField] private Transform targeter;
    [SerializeField] private Transform baseCollider;
    [SerializeField] private Transform baseModel;
    [SerializeField] private Transform barrelColliderPivot;
    [SerializeField] private Transform barrelModelPivot;

    //Non-Serialized Fields------------------------------------------------------------------------

    //[Header("ShotgunTurretAiming Testing")]
    //[SerializeField] private Transform target;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        building = gameObject.GetComponent<Building>();
        shooter = gameObject.GetComponent<TurretShooting>();
        currentTurretRotation = 0;
        currentBarrelElevation = 0;
        baseCollider.localRotation = Quaternion.Euler(rotationColliderOffset);
        baseModel.localRotation = Quaternion.Euler(rotationColliderOffset + rotationModelCounterOffset);
        barrelColliderPivot.localRotation = Quaternion.Euler(elevationColliderOffset);
        barrelModelPivot.localRotation = Quaternion.Euler(elevationColliderOffset + elevationModelCounterOffset);
        collisionReporters = GetCollisionReporters();
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        if (building.Operational)
        {
            if (shooter.Target != null)
            {
                CalculateRotationAndElevation();
                Aim();
            }            

            //ClampElevation();
            //Aim();
        }
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Calculates the local rotation the turret should have and the local elevation the barrel should have to aim at the target.
    /// </summary>
    protected override void CalculateRotationAndElevation()
    {
        //Setup
        targeter.LookAt(shooter.Target.transform.position);
        //targeter.LookAt(target.position);
        float rawRotation = targeter.rotation.eulerAngles.y;
        float rawElevation = targeter.rotation.eulerAngles.x + elevationColliderOffset.y;

        //Rotation
        targetTurretRotation = NormaliseAngle(rawRotation);

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

        //Debug.Log($"Targeter rotation: {targeter.rotation.eulerAngles}, rawElevation: {rawElevation}, target elevation: {targetBarrelElevation}, rawRotation: {rawRotation}, target rotation: {targetTurretRotation}");
    }

    /// <summary>
    /// Locally rotates the turret and elevates the barrel to aim at the target.
    /// </summary>
    protected override void Aim()
    {
        //Turret rotation on turret base's local z axis. All other local values remain static.
        if (currentTurretRotation != targetTurretRotation)
        {
            float deltaAngle = Mathf.DeltaAngle(currentTurretRotation, targetTurretRotation);
            float rotationDirection = Sign(deltaAngle);
            deltaAngle = Magnitude(deltaAngle);
            float fixedUpdateRotation = rotationSpeed * Time.fixedDeltaTime;

            currentTurretRotation += rotationDirection * Mathf.Min(deltaAngle, fixedUpdateRotation);
            currentTurretRotation = NormaliseAngle(currentTurretRotation);
            baseCollider.localRotation = Quaternion.Euler(rotationColliderOffset.x, rotationColliderOffset.y, currentTurretRotation + rotationColliderOffset.z);
            baseModel.localRotation = Quaternion.Euler(
                rotationColliderOffset.x + rotationModelCounterOffset.x, 
                rotationColliderOffset.y + rotationModelCounterOffset.y, 
                currentTurretRotation + rotationColliderOffset.z + rotationModelCounterOffset.z);
        }

        //Barrel pivoting on barrel pivot's local y axis. All other local values remain 0.
        if (currentBarrelElevation != targetBarrelElevation)
        {
            float deltaAngle = Mathf.DeltaAngle(currentBarrelElevation, targetBarrelElevation);
            float pivotDirection = Sign(deltaAngle);
            deltaAngle = Magnitude(deltaAngle);
            float fixedUpdatePivot = elevationSpeed * Time.fixedDeltaTime;

            currentBarrelElevation += pivotDirection * Mathf.Min(deltaAngle, fixedUpdatePivot);
            barrelColliderPivot.localRotation = Quaternion.Euler(elevationColliderOffset.x, currentBarrelElevation, elevationColliderOffset.z);
            barrelModelPivot.localRotation = Quaternion.Euler(
                elevationColliderOffset.x + elevationModelCounterOffset.x, 
                currentBarrelElevation + elevationModelCounterOffset.y, 
                elevationColliderOffset.z + elevationModelCounterOffset.z);
        }
    }
}
