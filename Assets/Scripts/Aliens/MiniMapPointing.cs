using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MiniMapPointing : MonoBehaviour
{
    //Minimap pointer
    private PointatonMinimap pointer;
    private PointatonMinimap.Pointer whilenotonscreenPointer;
    private Alien alien;

    private void Start()
    {
        alien = this.gameObject.GetComponent<Alien>();
        pointer = GameObject.Find("Pointer Controller").GetComponent<PointatonMinimap>();
        whilenotonscreenPointer = pointer.CreatePointer(this.transform, "Enemy Pointer");
        Debug.Log("I have been created from the MiniMapPointing");
    }

    private void Update()
    {
        if (alien.NavMeshAgent.enabled == false)
        {
            whilenotonscreenPointer.Hide();
        }
        else whilenotonscreenPointer.Show();
    }

    private void OnDestroy()
    {
        pointer.DestroyPointers(whilenotonscreenPointer);
    }
}
