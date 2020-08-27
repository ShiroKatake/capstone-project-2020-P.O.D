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
        targeter.LookAt(shooter.Target.transform.position + crawlerPositionOffset);
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
        //if (currentTurretRotation != targetTurretRotation)  //Left here in comments in case we want to put it back into usage for optimisation purposes.
        //{
        float deltaRotation = Mathf.DeltaAngle(currentTurretRotation, targetTurretRotation);
        float rotationDirection = MathUtility.Instance.Sign(deltaRotation);
        deltaRotation = MathUtility.Instance.FloatMagnitude(deltaRotation);
        float fixedUpdateRotation = rotationSpeed * Time.fixedDeltaTime;

        currentTurretRotation += rotationDirection * Mathf.Min(deltaRotation, fixedUpdateRotation);
        currentTurretRotation = MathUtility.Instance.NormaliseAngle(currentTurretRotation);
        baseCollider.localRotation = Quaternion.Euler(rotationColliderOffset.x, rotationColliderOffset.y, rotationColliderOffset.z + currentTurretRotation);
        baseModel.localRotation = Quaternion.Euler(
            rotationColliderOffset.x + rotationModelCounterOffset.x, 
            rotationColliderOffset.y + rotationModelCounterOffset.y, 
            rotationColliderOffset.z + rotationModelCounterOffset.z + currentTurretRotation);
        //}

        //Barrel pivoting on barrel pivot's local vertical axis. All other local values remain static.
        //if (currentBarrelElevation != targetBarrelElevation)  //Left here in comments in case we want to put it back into usage for optimisation purposes.
        //{
        float deltaElevation = Mathf.DeltaAngle(currentBarrelElevation, targetBarrelElevation);
        float elevationDirection = MathUtility.Instance.Sign(deltaElevation);
        deltaElevation = MathUtility.Instance.FloatMagnitude(deltaElevation);
        float fixedUpdateElevation = elevationSpeed * Time.fixedDeltaTime;

        currentBarrelElevation += elevationDirection * Mathf.Min(deltaElevation, fixedUpdateElevation);
        barrelColliderPivot.localRotation = Quaternion.Euler(elevationColliderOffset.x, elevationColliderOffset.y + currentBarrelElevation, elevationColliderOffset.z);
        barrelModelPivot.localRotation = Quaternion.Euler(
            elevationColliderOffset.x + elevationModelCounterOffset.x, 
            elevationColliderOffset.y + elevationModelCounterOffset.y + currentBarrelElevation, 
            elevationColliderOffset.z + elevationModelCounterOffset.z);
        //}
    }
}
