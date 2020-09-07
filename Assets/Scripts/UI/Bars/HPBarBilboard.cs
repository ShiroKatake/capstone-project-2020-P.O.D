using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarBilboard : MonoBehaviour
{
    [SerializeField] private Transform cam;

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
