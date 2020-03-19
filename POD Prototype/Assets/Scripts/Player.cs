using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [SerializeField] private GameObject drone;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject cameraPivot;

    //Non-Serialized Fields

    private float moveLeftRight;
    private float moveForwardsBackwards;
    private float moveUpDown;
    private float lookUpDown;
    private float lookLeftRight;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Awake()
    {
        if (drone == null)
        {
            Debug.Log("Player.drone needs to have a GameObject assigned to it.");
        }

        if (camera == null)
        {
            Debug.Log("Player.camera needs to have a Camera assigned to it.");
        }

        if (cameraPivot == null)
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
        //Reset old input
        moveLeftRight = 0f;
        moveForwardsBackwards = 0f;
        moveUpDown = 0f;
        lookUpDown = 0f;
        lookLeftRight = 0f;

        //Get new input
        moveLeftRight = InputController.Instance.GetAxis("MoveLeftRight");
        moveForwardsBackwards = InputController.Instance.GetAxis("MoveForwardsBackwards");
        moveUpDown = InputController.Instance.GetAxis("MoveUpDown");
        lookUpDown = InputController.Instance.GetAxis("LookUpDown");
        lookLeftRight = InputController.Instance.GetAxis("LookLeftRight");
    }

    private void UpdateDrone()
    {
        //Update Look
        if (lookLeftRight != 0)
        {
            Debug.Log("LookingLR");
            drone.transform.Rotate(0, lookLeftRight, 0);
        }

        if (lookUpDown != 0)
        {
            Debug.Log("LookingUD");
            cameraPivot.transform.Rotate(-lookUpDown, 0, 0);
        }

        if (moveForwardsBackwards != 0 || moveLeftRight != 0 || moveUpDown != 0)
        drone.transform.Translate(new Vector3(moveLeftRight, moveUpDown, moveForwardsBackwards), Space.Self);
    }
}
