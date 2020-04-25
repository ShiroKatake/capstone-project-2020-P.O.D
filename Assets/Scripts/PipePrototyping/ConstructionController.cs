using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionController : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] Text materialPrint;
    [SerializeField] GameObject hoverStructure;
    [SerializeField] GridRendering placeGrid;
    [SerializeField] PipeManager pipeManager;

    int layermask = 0b100000000;

    private EMaterials selectedMaterial;

    private List<EMaterials> tempInputs = new List<EMaterials>();
    private List<EMaterials> tempOutputs = new List<EMaterials>();

    private GameObject tempHover;

    private bool hovering = false;

    // Start is called before the first frame update
    void Start()
    {
        selectedMaterial = EMaterials.Water;

        UpdateSelectedMaterialText();

    }

    // Update is called once per frame
    void Update()
    {
        (bool, Vector3) mouseCast = RaycastMouse();
        

        if (Input.GetMouseButtonDown(0)) {
            if (hovering) {
                pipeManager.RegisterPipeBuilding(tempHover.GetComponent<PipeBuilding>());
                tempHover = null;
                ResetBuildingHover();
            } else {
                StartBuildingHover();
            }
        }
            

        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.F)) {
            CycleSelectedMaterial();
            UpdateSelectedMaterialText();
        }

        if (Input.GetKeyDown(KeyCode.Q) && hovering) {
            if (!tempInputs.Contains(selectedMaterial)) {
                tempInputs.Add(selectedMaterial);
            } else {
                tempInputs.Remove(selectedMaterial);
            }
            UpdateSelectedMaterialText();
            UpdateHoverIOs();
        }

        if (Input.GetKeyDown(KeyCode.E) && hovering) {
            if (!tempOutputs.Contains(selectedMaterial)) {
                tempOutputs.Add(selectedMaterial);
            }
            else {
                tempOutputs.Remove(selectedMaterial);
            }
            UpdateSelectedMaterialText();
            UpdateHoverIOs();
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (hovering) {
                ResetBuildingHover();
                Destroy(tempHover);
            } else {
                StartBuildingHover();
            }

        }

        if (hovering && mouseCast.Item1) {
            UpdateHoverPosition(mouseCast.Item2);
        }

    }

    private void ResetBuildingHover() {
        tempOutputs.Clear();
        tempInputs.Clear();
        UpdateSelectedMaterialText();

        hovering = false;
    }
    private void StartBuildingHover() {
        tempHover = Instantiate(hoverStructure, placeGrid.transform);
        tempHover.transform.localScale = new Vector3(placeGrid.scale, placeGrid.scale, placeGrid.scale);

        hovering = true;
    }

    private void UpdateHoverPosition(Vector3 position) {

        float gridScale = placeGrid.scale;
        
        Vector3 gridSnapPos = new Vector3(Mathf.Round(position.x / gridScale) * gridScale,
                                            position.y,
                                            Mathf.Round(position.z / gridScale) * gridScale);

        tempHover.transform.position = gridSnapPos;
    }

    private void UpdateHoverIOs() {
        PipeBuilding pipeBuilding = tempHover.GetComponent<PipeBuilding>();
        pipeBuilding.inputs = new List<EMaterials>(tempInputs);
        pipeBuilding.outputs = new List<EMaterials>(tempOutputs);
    }

    private (bool,Vector3) RaycastMouse() {

        Vector3 hitPoint = Vector3.zero;

        RaycastHit hit;

        bool raycast = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layermask);

        if (raycast) {
            //Debug.Log(hit.point);
            hitPoint = hit.point;
        }



        return (raycast, hitPoint);
    }

    private void CycleSelectedMaterial() {

        EMaterials[] vals = (EMaterials[])Enum.GetValues(typeof(EMaterials));

        for (int i = 0; i < vals.Length; i++) {
            if (vals[i] == selectedMaterial) {
                int nextSelect = i + 1;
                if (nextSelect >= vals.Length)
                    nextSelect = 0;
                selectedMaterial = vals[nextSelect];
                break;
            }
        }
    }

    private void UpdateSelectedMaterialText() {
        string debugText = Enum.GetName(typeof(EMaterials), selectedMaterial) + "\n";

        if (tempInputs.Contains(selectedMaterial)) {
            debugText += "(IN)";
        }
        if (tempOutputs.Contains(selectedMaterial)) {
            debugText += "\t(OUT)";
        }

        materialPrint.text = debugText;
    }

    public (int, int) PointToGrid(Vector3 point) {
        //return ((int))
        return ((int)Mathf.Round(point.x), (int)Mathf.Round(point.z));
    }
}
