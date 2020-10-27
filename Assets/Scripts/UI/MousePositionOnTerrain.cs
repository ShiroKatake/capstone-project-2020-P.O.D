using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MousePositionOnTerrain : PublicInstanceSerializableSingleton<MousePositionOnTerrain>
{
    private Plane plane;
    [SerializeField] private Camera camera;
    private TerrainCollider terrainCollider;
    private Vector3 worldPosition;
    private Ray ray;

    public Vector3 GetWorldPosition {get => worldPosition;}

    protected override void Awake() {
        base.Awake();
        plane = new Plane(Vector3.up, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        //terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 camPos = ReInput.controllers.Mouse.screenPosition;
        camPos.z = 0.3f;
        /*ray = Camera.main.ScreenPointToRay(camPos);
        RaycastHit hitData;

        if(terrainCollider.Raycast(ray, out hitData, 1000))
        {
            worldPosition = hitData.point;
        }*/

        /*
            Code for using a Plane, im pretty sure this needs to change to use the above Terrain object code
        */

        float dist;
        ray = camera.ScreenPointToRay(ReInput.controllers.Mouse.screenPosition);

        if (plane.Raycast(ray, out dist))
        {
            worldPosition = ray.GetPoint(dist);
            //Debug.DrawLine(worldPosition, worldPosition + Vector3.up * 10);
        }

        //print("World Position from GameManager: " + worldPosition);
    }
}
