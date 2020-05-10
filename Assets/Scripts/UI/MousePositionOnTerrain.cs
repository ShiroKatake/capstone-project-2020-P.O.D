using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MousePositionOnTerrain : MonoBehaviour
{
    private Plane plane;
    [SerializeField] private Camera camera;
    private TerrainCollider terrainCollider;
    private Vector3 worldPosition;
    private Ray ray;

    public static MousePositionOnTerrain Instance {get; protected set;}

    public Vector3 GetWorldPosition {get => worldPosition;}

    private void Awake() {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more Mouse Position On Terrain's.");
        }

        Instance = this;
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

        plane = new Plane(/*Vector3.up*/ new Vector3(0,1,0), -0.5f);
        float dist;
        ray = camera.ScreenPointToRay(ReInput.controllers.Mouse.screenPosition);
        if (plane.Raycast(ray, out dist)){
            worldPosition = ray.GetPoint(dist);
        }

        //print("World Position from GameManager: " + worldPosition);
    }
}
