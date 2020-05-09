using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MouseObject : MonoBehaviour
{
    [SerializeField] private GameObject mouse;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouse.transform.position = ReInput.controllers.Mouse.screenPosition;
    }
}
