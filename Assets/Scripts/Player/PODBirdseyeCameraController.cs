using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PODBirdseyeCameraController : MonoBehaviour
{
    [SerializeField] private GameObject povCamera;
    [SerializeField] private GameObject birdseyeCamera;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private GameObject ui;

    [SerializeField] private Vector3 povLocalRotation;
    [SerializeField] private Vector3 birdseyeLocalRotation;

    private void Awake()
    {
        Debug.Log($"PODBirdseyeCameraController is enabled. Remember to turn it off when making a non-developer build.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {            
            povCamera.SetActive(!povCamera.activeSelf);
            birdseyeCamera.SetActive(!povCamera.activeSelf);
            cameraTarget.localRotation = Quaternion.Euler(povCamera.activeSelf ? povLocalRotation : birdseyeLocalRotation);
            ui.SetActive(povCamera.activeSelf);
        }
    }
}
