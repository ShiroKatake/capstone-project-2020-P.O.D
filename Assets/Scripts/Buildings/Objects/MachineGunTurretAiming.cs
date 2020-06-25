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

    [Header("Game Objects")]
    [SerializeField] private Transform rotationTargeter;
    [SerializeField] private Transform elevationTargeter;
    [SerializeField] private Transform armColliderPivot;
    [SerializeField] private Transform armModelPivot;
    [SerializeField] private Transform barrelColliderPivot;
    [SerializeField] private Transform barrelModelPivot;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        Reset();
    }

    /// <summary>
    /// Setup / reset code for the machine gun turret.
    /// </summary>
    public override void Reset()
    {
        //Debug.Log("MachineGunTurretAiming.Reset()");
        base.Reset();
        armColliderPivot.localRotation = Quaternion.Euler(rotationColliderOffset);
        armModelPivot.localRotation = Quaternion.Euler(rotationColliderOffset + rotationModelCounterOffset);
        barrelColliderPivot.localRotation = Quaternion.Euler(elevationColliderOffset);
        barrelModelPivot.localRotation = Quaternion.Euler(elevationColliderOffset + elevationModelCounterOffset);
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
        rotationTargeter.LookAt(shooter.Target.transform.position + crawlerPositionOffset);
        elevationTargeter.LookAt(shooter.Target.transform.position + crawlerPositionOffset);
        //rotationTargeter.LookAt(target.position);
        //elevationTargeter.LookAt(target.position);
        float rawRotation = rotationTargeter.rotation.eulerAngles.y;
        float rawElevation = elevationTargeter.rotation.eulerAngles.x + elevationColliderOffset.z;

        //Rotation
        targetTurretRotation = MathUtility.Instance.NormaliseAngle(rawRotation);

        //Elevation
        targetBarrelElevation = (rawElevation > 90 ? 360 - rawElevation : rawElevation * -1);

        ClampElevation();

        //Debug.Log($"Rotation targeter rotation: {rotationTargeter.rotation.eulerAngles}, Elevation targeter rotation: {elevationTargeter.rotation.eulerAngles}, rawElevation: {rawElevation}, target elevation: {targetBarrelElevation}, rawRotation: {rawRotation}, target rotation: {targetTurretRotation}");
    }

    /// <summary>
    /// Locally rotates the turret and elevates the barrel to aim at the target.
    /// </summary>
    protected override void Aim()
    {
        //Turret rotation on turret base's local horizontal axis. All other local values remain static.
        if (currentTurretRotation != targetTurretRotation)
        //if (true)
        {
            float deltaAngle = Mathf.DeltaAngle(currentTurretRotation, targetTurretRotation);
            float rotationDirection = MathUtility.Instance.Sign(deltaAngle);
            deltaAngle = MathUtility.Instance.FloatMagnitude(deltaAngle);
            float fixedUpdateRotation = rotationSpeed * Time.fixedDeltaTime;

            currentTurretRotation += rotationDirection * Mathf.Min(deltaAngle, fixedUpdateRotation);
            currentTurretRotation = MathUtility.Instance.NormaliseAngle(currentTurretRotation);
            armColliderPivot.localRotation = Quaternion.Euler(rotationColliderOffset.x + currentTurretRotation, rotationColliderOffset.y, rotationColliderOffset.z);
            armModelPivot.localRotation = Quaternion.Euler(
                currentTurretRotation + rotationColliderOffset.x + rotationModelCounterOffset.x,
                rotationColliderOffset.y + rotationModelCounterOffset.y,
                rotationColliderOffset.z + rotationModelCounterOffset.z);
        }

        //Barrel pivoting on barrel pivot's local vertical axis. All other local values remain static.
        if (currentBarrelElevation != targetBarrelElevation)
        //if (true)
        {
            float deltaAngle = Mathf.DeltaAngle(currentBarrelElevation, targetBarrelElevation);
            float pivotDirection = MathUtility.Instance.Sign(deltaAngle);
            deltaAngle = MathUtility.Instance.FloatMagnitude(deltaAngle);
            float fixedUpdatePivot = elevationSpeed * Time.fixedDeltaTime;

            currentBarrelElevation += pivotDirection * Mathf.Min(deltaAngle, fixedUpdatePivot);
            barrelColliderPivot.localRotation = Quaternion.Euler(elevationColliderOffset.x, elevationColliderOffset.z, - elevationColliderOffset.y - currentBarrelElevation);
            barrelModelPivot.localRotation = Quaternion.Euler(
                elevationColliderOffset.x + elevationModelCounterOffset.x,
                elevationColliderOffset.y + elevationModelCounterOffset.y + currentBarrelElevation,
                elevationColliderOffset.z + elevationModelCounterOffset.z);
        }
    }
}
