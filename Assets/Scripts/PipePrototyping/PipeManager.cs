using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PipeManager : MonoBehaviour {

    

    public static PipeManager Instance { get; protected set; }

    List<PipeBuilding> buildings = new List<PipeBuilding>();
    List<PipeNode> nodes = new List<PipeNode>();
    List<PipeSegment> pipeSegments = new List<PipeSegment>();


    List<LineSegment> lines = new List<LineSegment>();
    List<Label> labels = new List<Label>();

    MeshFilter meshFilter;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There should never be more than one Pipe manager");
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void RegisterPipeBuilding(PipeBuilding pipeBuilding) {
        Vector3 fromPos = pipeBuilding.transform.position;

        RegisterPipeBuilding(fromPos, ENodeType.BUILDING);
    }

    public void RegisterPipeBuilding(Vector3 pos, ENodeType nodeType = ENodeType.INTERSECTION) {
        //RegisterPipeBuilding(pos, nodeType);

        PipeNode newNode = new PipeNode(pos, nodeType);

        nodes.Add(newNode);

        RecalculatePipeMesh(newNode);
    }


    private void RecalculatePipeMesh(PipeNode newNode) {

        if (nodes.Count > 1) {

            Vector3 startPosition = Vector3.zero;
            Vector3 endPosition = newNode.position;

            PipeNode closestNode = nodes[0];
            float closestNodeDistance = 9999999;

            // Check all the existing nodes and find the closest one

            foreach (PipeNode node in nodes) {
                if (node != newNode) {

                    Vector3 deltaVector = newNode.position - node.position;
                    float thisDistance = deltaVector.sqrMagnitude;

                    if (thisDistance < closestNodeDistance) {
                        closestNodeDistance = thisDistance;
                        closestNode = node;
                    }

                }
            }

            startPosition = closestNode.position;

            if (pipeSegments.Count > 0) {

                PipeSegment closestSegment = pipeSegments[0];
                float closestSegmentDistance = 999999;

                // Find the closest segment
                foreach (PipeSegment segment in pipeSegments) {
                    Vector3 segmentDelta = segment.toNode.position - segment.fromNode.position;
                    bool withinSegment = false;

                    // Check if point is along segment
                    if (segmentDelta.x == 0) {
                        //Segment is a vertical line
                        Vector3 testVec = newNode.position - segment.toNode.position;
                        if (Mathf.Abs(testVec.z) < Mathf.Abs(segmentDelta.z/2)) {
                            withinSegment = true;
                        }
                    }

                    if (withinSegment) {
                        float thisDistance = PerpendicularDistance(segment.fromNode.position, segment.toNode.position, newNode.position);

                        if (thisDistance < closestSegmentDistance) {
                            closestSegmentDistance = thisDistance;
                            closestSegment = segment;
                        }
                    }
                }


                if (closestSegmentDistance < closestNodeDistance) {
                    Vector3 segmentVector = (closestSegment.toNode.position - closestSegment.fromNode.position).normalized;
                    segmentVector = Quaternion.AngleAxis(90, Vector3.up) * segmentVector;

                    startPosition = endPosition - segmentVector * closestSegmentDistance;
                }

            }

            AddOrthogonalSegments(startPosition, endPosition);
        }
        

    }

    private void AddOrthogonalSegments(Vector3 start, Vector3 end) {
        Vector3 deltaVector = end - start;

        if (deltaVector.z ==0 || deltaVector.x == 0) {
            AddPipeSegment(start, end);
        } else if (deltaVector.z >= deltaVector.x) {
            AddPipeSegment(start, start + new Vector3(deltaVector.x / 2, 0, 0));
            AddPipeSegment(start + new Vector3(deltaVector.x / 2, 0, 0), end - new Vector3(deltaVector.x / 2, 0, 0));
            AddPipeSegment(end - new Vector3(deltaVector.x / 2, 0, 0), end);
        } else {
            AddPipeSegment(start, start + new Vector3(0, 0, deltaVector.z / 2));
            AddPipeSegment(start + new Vector3(0, 0, deltaVector.z / 2), end - new Vector3(0, 0, deltaVector.z / 2));
            AddPipeSegment(end - new Vector3(0, 0, deltaVector.z / 2), end);
        }
    }

    private void AddPipeSegment(Vector3 start, Vector3 end) {
        PipeNode startNode = null;
        PipeNode endNode = null;

       

        for (int i = 0; i < nodes.Count; i ++) {
            if (nodes[i].position == start)
                startNode = nodes[i];
            if (nodes[i].position == end)
                endNode = nodes[i];
        }

        if (startNode == null) {
            startNode = new PipeNode(start, ENodeType.INTERSECTION);
            nodes.Add(startNode);
            //Debug.Log("Added Start Node");
        }
        if (endNode == null) {
            endNode = new PipeNode(end, ENodeType.INTERSECTION);
            nodes.Add(endNode);
            //Debug.Log("Added End Node");
        }

        pipeSegments.Add(new PipeSegment(startNode, endNode));
        /*foreach (PipeNode node in nodes) {
            if (node.position == start)
                startNode = node;
            if (node.position == end)
                endNode = node;
        }*/



        //pipeSegments
    }
        

    private class PipeNode {

        public Vector3 position;
        public ENodeType nodeType;

        public PipeNode(Vector3 position, ENodeType nodeType) {
            this.position = position;
            this.nodeType = nodeType;
        }

    }
    private class PipeSegment {
        public PipeNode fromNode, toNode;

        public PipeSegment(PipeNode fromNode, PipeNode toNode) {
            this.fromNode = fromNode;
            this.toNode = toNode;
        }
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
            UnityEngine.Random.InitState(100);


            foreach (LineSegment line in lines) {
                Gizmos.color = line.col;
                Gizmos.DrawLine(line.from, line.to);
            }

            foreach (var label in labels) {
                Handles.color = label.col;
                Handles.Label(label.loc, label.label);
            }

            Gizmos.color = Color.cyan;
            foreach (PipeSegment seg in pipeSegments) {
                Gizmos.DrawLine(seg.fromNode.position, seg.toNode.position);
            }
            foreach(PipeNode node in nodes) {
                Gizmos.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                Gizmos.DrawWireSphere(node.position, UnityEngine.Random.value * 0.05f + 0.05f);
            }

        }
    }

    private enum MajorAxis {
        X,
        Y,
        Equal
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


