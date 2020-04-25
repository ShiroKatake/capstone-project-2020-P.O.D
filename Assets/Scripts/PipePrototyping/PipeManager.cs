using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{

    List<PipeBuilding> buildings = new List<PipeBuilding>();

    List<MaterialIO> waterIOs = new List<MaterialIO>();

    Dictionary<EMaterials, List<MaterialIO>> IOCollection;
    Dictionary<EMaterials, List<LineGizmo>> PipeNetwork;

    // Start is called before the first frame update
    void Start()
    {
        PipeNetwork = new Dictionary<EMaterials, List<LineGizmo>>();
        PipeNetwork[EMaterials.Water] = new List<LineGizmo>();
        PipeNetwork[EMaterials.Waste] = new List<LineGizmo>();
        PipeNetwork[EMaterials.Ore] =   new List<LineGizmo>();
        PipeNetwork[EMaterials.Power] = new List<LineGizmo>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PopulateIOs() {
        IOCollection = new Dictionary<EMaterials, List<MaterialIO>>();

        IOCollection[EMaterials.Water] = new List<MaterialIO>();
        IOCollection[EMaterials.Waste] = new List<MaterialIO>();
        IOCollection[EMaterials.Ore] = new List<MaterialIO>();
        IOCollection[EMaterials.Power] = new List<MaterialIO>();

        foreach (PipeBuilding building in buildings) {

            foreach (EMaterials mat in Enum.GetValues(typeof(EMaterials))) {
                //IOCollection[mat]

                bool input = building.inputs.Contains(mat);
                bool output = building.outputs.Contains(mat);

                if (input || output)
                    IOCollection[mat].Add(new MaterialIO(building.transform.position, input, output));
            }
        }
    }
    

    public void RegisterPipeBuilding(PipeBuilding building) {
        buildings.Add(building);

        PopulateIOs();
        ConstructAllPipeNetworks();
    }


    private void ConstructAllPipeNetworks() {
        ConstructPipeNetwork(EMaterials.Water);
        //ConstructPipeNetwork(EMaterials.Waste);
        //ConstructPipeNetwork(EMaterials.Ore);
        //ConstructPipeNetwork(EMaterials.Power);
    }

    private void ConstructPipeNetwork(EMaterials mat) {
        List<MaterialIO> materialIOs = IOCollection[mat];

        Queue<Vector3> inputPositions = new Queue<Vector3>();
        Queue<Vector3> outputPositions = new Queue<Vector3>();

        //If there is at least one IO pair
        foreach (MaterialIO matIO in materialIOs) {
            if (matIO.input)
                inputPositions.Enqueue(matIO.position);
            if (matIO.output)
                outputPositions.Enqueue(matIO.position);
        }
        if (inputPositions.Count > 0 && outputPositions.Count > 0) {
            //CREATE PIPE NETWORK

            while (inputPositions.Count > 0 && outputPositions.Count > 0) { 
                PipeNetwork[mat].Add(new LineGizmo(inputPositions.Dequeue(), outputPositions.Dequeue()));
            }

        } else {
            Debug.Log("No IO pair present yet. No network generated for " + mat);
        }


    }

    private struct MaterialIO {
        public Vector3 position;
        public bool input;
        public bool output;

        public MaterialIO(Vector3 position, bool input, bool output) {
            this.position = position;
            this.input = input;
            this.output = output;
        }
    }

    private struct LineGizmo {
        public Vector3 from;
        public Vector3 to;

        public LineGizmo(Vector3 from, Vector3 to) {
            this.from = from;
            this.to = to;
        }
    }

    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.blue;
            foreach (LineGizmo line in PipeNetwork[EMaterials.Water]) {
                Gizmos.DrawLine(line.from, line.to);
            }
        }
        //Gizmos.DrawLine()
    }
}


