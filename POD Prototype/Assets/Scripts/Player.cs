using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [Header("Player Objects")]
    [SerializeField] private GameObject drone;
    [SerializeField] private GameObject cameraTarget;

    [Header("Player Movement Speeds")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    //Non-Serialized Fields

    //Variables for moving & determining if rotation is necessary
    private Vector3 movement;
    private Vector3 previousMovement = Vector3.zero;

    //Variables for rotating smoothly
    private Quaternion newRotation;
    private Quaternion oldRotation;
    private float slerpProgress = 1;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Awake()
    {
        if (drone == null)
        {
            Debug.Log("Player.drone needs to have a GameObject assigned to it.");
        }

        if (cameraTarget == null)
        {
            Debug.Log("Player.cameraPivot needs to have a Camera assigned to it.");
        }
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

    // Update is called once per frame
    void Update()
    {
        GetInput();
        UpdateDrone();
    }

    private void GetInput()
    {
        movement = new Vector3(InputController.Instance.GetAxis("MoveLeftRight"), 0, InputController.Instance.GetAxis("MoveForwardsBackwards"));
    }

    private void UpdateDrone()
    {
        Look();
        Move();    
    }

    private void Look()
    {
        //Player wants to move in a new direction? Update Slerp variables.
        if (movement != previousMovement)
        {
            slerpProgress = 0;
            oldRotation = drone.transform.rotation;
            newRotation = Quaternion.LookRotation(movement);
        }

        //Still turning? Rotate towards direction player wants to move in, but smoothly.
        if (slerpProgress < 1/* && movement != Vector3.zero*/)
        {
            slerpProgress = Mathf.Min(1, slerpProgress + rotationSpeed * Time.deltaTime);
            drone.transform.rotation = Quaternion.Slerp(oldRotation, newRotation, slerpProgress);
        }       
    }

    private void Move()
    {
        //Player wants to move? Move the drone.
        if (movement != Vector3.zero)
        {
            drone.transform.Translate(new Vector3(0, 0, movementSpeed * movement.magnitude * Time.deltaTime), Space.Self);
            cameraTarget.transform.position = drone.transform.position;
        }
    }
}
