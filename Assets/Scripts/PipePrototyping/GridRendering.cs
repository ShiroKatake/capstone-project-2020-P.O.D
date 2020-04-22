using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridRendering : MonoBehaviour
{

    [SerializeField] private (int, int) gridSize = (20, 20);
    [SerializeField] private float scale = 0.5f;
    [SerializeField] private Camera cam;
    [SerializeField] Text materialPrint;

    int layermask = 0b100000000;

    private EMaterials selectedMaterial;


    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            child.localScale = Vector3.one * 0.5f;
        }

        selectedMaterial = EMaterials.Water;

        CycleSelectedMaterial();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
            RaycastMouse();

        if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
            CycleSelectedMaterial();
        }

    }

    private void RaycastMouse() {

        RaycastHit hit;

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit, Mathf.Infinity, layermask)) {
            Debug.Log(hit.point);
        }
    }

    private void CycleSelectedMaterial() {

        EMaterials[] vals = (EMaterials[])Enum.GetValues(typeof(EMaterials));

        for (int i = 0; i < vals.Length; i ++) {
            if (vals[i] == selectedMaterial) {
                int nextSelect = i + 1;
                if (nextSelect >= vals.Length)
                    nextSelect = 0;
                selectedMaterial = vals[nextSelect];
                break;
            }
        }

        

        materialPrint.text = Enum.GetName(typeof(EMaterials), selectedMaterial);
    }


    public (int,int) PointToGrid(Vector3 point) {
        return (0,0);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.black;

        float width = gridSize.Item1;
        float height = gridSize.Item2;

        for (int xx = 0; xx < gridSize.Item1+1; xx++) {

            Vector3 startH = new Vector3(-width / 2 + xx, 0.01f, -height / 2) * scale;
            Vector3 endH = new Vector3(-width / 2 + xx, 0.01f, height / 2) * scale;
            Gizmos.DrawLine(startH, endH);
            for (int yy = 0; yy < gridSize.Item2+1; yy++) {

                Vector3 startV = new Vector3(-width / 2, 0.01f, -height / 2 + yy) * scale;
                Vector3 endV = new Vector3(width / 2, 0.01f, -height / 2 + yy) * scale;

                Gizmos.DrawLine(startV, endV);
            }
        }
    }

}
