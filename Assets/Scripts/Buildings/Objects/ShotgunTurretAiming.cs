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

    [Header("Game Objects")]
    [SerializeField] private Transform targeter;
    [SerializeField] private Transform baseCollider;
    [SerializeField] private Transform baseModel;
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
    /// Setup / reset code for shotgun turrets.
    /// </summary>
    public override void Reset()
    {
        //Debug.Log("ShotgunTurretAiming.Reset()");
        base.Reset();
        baseCollider.localRotation = Quaternion.Euler(rotationColliderOffset);
        baseModel.localRotation = Quaternion.Euler(rotationColliderOffset + rotationModelCounterOffset);
        barrelColliderPivot.localRotation = Quaternion.Euler(elevationColliderOffset);  //TODO: add inspector-set initial elevation
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
        targeter.LookAt(shooter.Target.transform.position);
        //targeter.LookAt(target.position);
        float rawRotation = targeter.rotation.eulerAngles.y;
        float rawElevation = targeter.rotation.eulerAngles.x + elevationColliderOffset.y;

        //Rotation
        targetTurretRotation = MathUtility.Instance.NormaliseAngle(rawRotation);

        //Elevation
        targetBarrelElevation = (rawElevation > 90 ? 360 - rawElevation : rawElevation * -1);

        ClampElevation();

        //Debug.Log($"Targeter rotation: {targeter.rotation.eulerAngles}, rawElevation: {rawElevation}, target elevation: {targetBarrelElevation}, rawRotation: {rawRotation}, target rotation: {targetTurretRotation}");
    }

    /// <summary>
    /// Locally rotates the turret and elevates the barrel to aim at the target.
    /// </summary>
    protected override void Aim()
    {
        //Turret rotation on turret base's local horizontal axis. All other local values remain static.
        if (currentTurretRotation != targetTurretRotation)
        {
            float deltaAngle = Mathf.DeltaAngle(currentTurretRotation, targetTurretRotation);
            float rotationDirection = MathUtility.Instance.Sign(deltaAngle);
            deltaAngle = MathUtility.Instance.FloatMagnitude(deltaAngle);
            float fixedUpdateRotation = rotationSpeed * Time.fixedDeltaTime;

            currentTurretRotation += rotationDirection * Mathf.Min(deltaAngle, fixedUpdateRotation);
            currentTurretRotation = MathUtility.Instance.NormaliseAngle(currentTurretRotation);
            baseCollider.localRotation = Quaternion.Euler(rotationColliderOffset.x, rotationColliderOffset.y, currentTurretRotation + rotationColliderOffset.z);
            baseModel.localRotation = Quaternion.Euler(
                rotationColliderOffset.x + rotationModelCounterOffset.x, 
                rotationColliderOffset.y + rotationModelCounterOffset.y, 
                currentTurretRotation + rotationColliderOffset.z + rotationModelCounterOffset.z);
        }

        //Barrel pivoting on barrel pivot's local vertical axis. All other local values remain static.
        if (currentBarrelElevation != targetBarrelElevation)
        {
            float deltaAngle = Mathf.DeltaAngle(currentBarrelElevation, targetBarrelElevation);
            float pivotDirection = MathUtility.Instance.Sign(deltaAngle);
            deltaAngle = MathUtility.Instance.FloatMagnitude(deltaAngle);
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
