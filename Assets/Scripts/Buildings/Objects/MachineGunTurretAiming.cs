using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base component class for aiming the gun part of machine gun turrets.
/// </summary>
public class MachineGunTurretAiming : TurretAiming
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Aiming Offsets")]
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private Vector3 elevationOffset;

    [Header("Game Objects")]
    [SerializeField] private Transform rotationTargeter;
    [SerializeField] private Transform elevationTargeter;
    [SerializeField] private Transform barrelBaseColliderPivot;
    [SerializeField] private Transform barrelBaseModelPivot;
    [SerializeField] private Transform barrelColliderPivot;
    [SerializeField] private Transform barrelModelPivot;

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
        barrelBaseColliderPivot.rotation = Quaternion.Euler(rotationOffset);
        barrelBaseModelPivot.rotation = barrelBaseColliderPivot.rotation;
        barrelColliderPivot.rotation = Quaternion.Euler(elevationOffset);
        barrelModelPivot.rotation = barrelColliderPivot.rotation;
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
            CalculateRotationAndElevation();
            Aim();
        }
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Calculates the local rotation the turret should have and the local elevation the barrel should have to aim at the target.
    /// </summary>
    protected override void CalculateRotationAndElevation()
    {
        //Variables
        float rawRotation;
        float rawElevation;

        //Rotation
        rotationTargeter.LookAt(shooter.Target.transform.position);
        rawRotation = rotationTargeter.rotation.eulerAngles.y;
        targetTurretRotation = NormaliseAngle(rawRotation + 90);

        //Elevation
        if (rotationTargeter == elevationTargeter)
        {
            rawElevation = rotationTargeter.rotation.eulerAngles.x;
        }
        else
        {
            elevationTargeter.LookAt(shooter.Target.transform.position);
            rawElevation = elevationTargeter.rotation.eulerAngles.x;
        }

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
            barrelBaseColliderPivot.localRotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, currentTurretRotation + rotationOffset.z);


            barrelBaseModelPivot.localRotation = barrelBaseColliderPivot.localRotation;
        }

        //Barrel pivoting on barrel pivot's local y axis. All other local values remain 0.
        if (currentBarrelElevation != targetBarrelElevation)
        {
            float deltaAngle = Mathf.DeltaAngle(currentBarrelElevation, targetBarrelElevation);
            float pivotDirection = Sign(deltaAngle);
            deltaAngle = Magnitude(deltaAngle);
            float fixedUpdatePivot = elevationSpeed * Time.fixedDeltaTime;

            currentBarrelElevation += pivotDirection * Mathf.Min(deltaAngle, fixedUpdatePivot);
            barrelColliderPivot.localRotation = Quaternion.Euler(elevationOffset.x, currentBarrelElevation + elevationOffset.y, elevationOffset.z);
            barrelModelPivot.localRotation = barrelColliderPivot.localRotation;
        }
    }
}
