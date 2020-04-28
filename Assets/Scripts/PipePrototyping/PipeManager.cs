using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PipeManager : MonoBehaviour {

    List<PipeBuilding> buildings = new List<PipeBuilding>();
    List<PipeNode> nodes = new List<PipeNode>();

    List<LineSegment> lines = new List<LineSegment>();
    List<Label> labels = new List<Label>();

    MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start() {
        meshFilter = GetComponent<MeshFilter>();


    }

    // Update is called once per frame
    void Update() {

    }


    public void RegisterPipeBuilding(PipeBuilding pipeBuilding) {

        //nodes.Add(new PipeNode(pipeBuilding.transform.position));

        Vector3 fromPos = pipeBuilding.transform.position;

        List<LineSegment> tempLines = new List<LineSegment>();



        // get a list of buildings and their distances
        List<(PipeNode, float, MajorAxis, Label)> distances = new List<(PipeNode, float, MajorAxis, Label)>();
        foreach (PipeNode node in nodes) {
            Vector3 toPos = node.position;

            Vector3 halfPos = toPos + (fromPos - toPos) / 2;

            float diffX = Math.Abs(fromPos.x - toPos.x);
            float diffY = Math.Abs(fromPos.z - toPos.z);
            float dist = diffX + diffY;

            MajorAxis axis;
            if (diffX > diffY)
                axis = MajorAxis.X;
            else if (diffY > diffX)
                axis = MajorAxis.Y;
            else axis = MajorAxis.Equal;

            distances.Add((node, dist, axis, new Label(halfPos, dist.ToString(), Color.red)));


        }

        //Distances sorted, shortest distance first
        distances = distances.OrderBy(o => o.Item2).ToList();
        if (distances.Count > 0) {

            /*switch(distances[0].Item3) {
                case all://MajorAxis.X:*/
            Vector3 toPos = distances[0].Item1.position;

            Vector3 verticalSegment = new Vector3(0, 0, toPos.z - fromPos.z);
            Vector3 horizontalSegment = new Vector3(toPos.x - fromPos.x, 0, 0);

            lines.Add(new LineSegment(fromPos, fromPos + horizontalSegment, Color.green));
            lines.Add(new LineSegment(toPos, toPos - verticalSegment, Color.green));
            nodes.Add(new PipeNode(fromPos + horizontalSegment));
            //      break;
            //}

            //lines.Add(new LineSegment(fromPos, distances[0].Item1.position, Color.blue));
            labels.Add(distances[0].Item4);
        }

        buildings.Add(pipeBuilding);
        nodes.Add(new PipeNode(fromPos));

        UpdateMesh();
    }

    private void UpdateMesh() {

        Mesh m = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        //        foreach (LineSegment line in lines) {
        for (int i = 0; i < lines.Count; i ++) {
            verts.Add(lines[i].from);
            verts.Add(lines[i].to);
            indices.Add(2 * i);
            indices.Add(2 * i + 1);
        }


        m.SetVertices(verts);
        m.SetIndices(indices, MeshTopology.Lines, 0);
        //m.mesh MeshTopology.Lines

        meshFilter.mesh = m;

    }


    private class PipeNode {

        public Vector3 position;

        public PipeNode(Vector3 position) {
            this.position = position;
        }

        //public void AddNode

    }

    

    private class Label {
        public Vector3 loc;
        public string label;
        public Color col;

        public Label(Vector3 loc, string label, Color col) {
            this.loc = loc;
            this.label = label;
            this.col = col;
        }
    }

    private float PerpendicularDistance(Vector3 from, Vector3 to, Vector3 point) {

        Vector3 dirPos = to - from;
        float dist = dirPos.magnitude;

        float x0 = point.x;
        float y0 = point.z;

        float x1 = from.x;
        float x2 = to.x;
        float y1 = from.z;
        float y2 = to.z;

        float distPerp = Mathf.Abs((y2 - y1) * x0 - (x2 - x1) * y0 + x2 * y1 - y2 * x1) / dist;
        return distPerp;
    }

    public float LineSide(Vector3 from, Vector3 to, Vector3 point) {
        float x0 = point.x;
        float y0 = point.z;

        float x1 = from.x;
        float x2 = to.x;
        float y1 = from.z;
        float y2 = to.z;

        float d = (x0 - x1) * (y2 - y1) - (y0 - y1) * (x2 - x1);

        return d;

    }

    private void OnDrawGizmos() {
        if (Application.isPlaying) {

            foreach (LineSegment line in lines) {
                Gizmos.color = line.col;
                Gizmos.DrawLine(line.from, line.to);
            }

            foreach (var label in labels) {
                Handles.color = label.col;
                Handles.Label(label.loc, label.label);
            }

        }
    }

    private enum MajorAxis {
        X,
        Y,
        Equal
    }

    private class PipeSegment {
        public PipeNode from;
        public PipeNode to;
    }

    private class LineSegment {
        public Vector3 from;
        public Vector3 to;
        public Color col;

        public LineSegment(Vector3 from, Vector3 to, Color col) {
            this.from = from;
            this.to = to;
            this.col = col;
        }
    }

}


