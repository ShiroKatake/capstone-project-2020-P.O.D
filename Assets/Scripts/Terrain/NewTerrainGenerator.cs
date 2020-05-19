using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewTerrainGenerator : MonoBehaviour
{

    private struct Line {
        public Vector3 from;
        public Vector3 to;

        public Line(Vector3 from, Vector3 to) {
            this.from = from;
            this.to = to;
        }
    }

    float[,] heights;
    List<Vector3> edgePoints;
    List<Line> lines;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMesh() {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh newMesh = new Mesh();






        filter.mesh = newMesh;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xx"></param>
    /// <param name="yy"></param>
    /// <returns>True if all neighbours are the same height</returns>
    private bool EvaluateNeighbours(int xx, int yy) {
        float centreHeight = heights[xx, yy];

        bool allSame = true;

        //Left edge
        if (xx != 0) {
            if (heights[xx - 1, yy] > centreHeight)
                allSame = false;
        }
        //Right Edge
        if (xx != heights.GetLength(0)-1) {
            if (heights[xx + 1, yy] > centreHeight)
                allSame = false;
        }

        //bottom edge
        if (yy != 0) {
            if (heights[xx, yy - 1] > centreHeight)
                allSame = false;
        }
        //top Edge
        if (yy != heights.GetLength(1)-1) {
            if (heights[xx, yy + 1] > centreHeight)
                allSame = false;
        }

        if (xx == 0 || yy == 0 || xx == heights.GetLength(0)-1 || yy == heights.GetLength(1)-1) {
            allSame = false;
        }

        return allSame;

    }

    public void RegenerateHeightmap(AnimationCurve curve, float noiseScale) {
        int heightmapRes = 100;
        
        // Create raw heightmap
        heights = new float[heightmapRes, heightmapRes];

        edgePoints = new List<Vector3>();
        lines = new List<Line>();

        //heightex = new Texture2D(heightmapRes, heightmapRes);
        for (int xx = 0; xx < heightmapRes; xx++) {
            for (int yy = 0; yy < heightmapRes; yy++) {
                Vector2 pos = new Vector2(xx, yy);
                float height;

                height = Mathf.PerlinNoise((float)xx * noiseScale + 0.1f, (float)yy * noiseScale + 0.1f);
                height = curve.Evaluate(height);

                heights[xx, yy] = height;
            }
        }

        // Find edge points between layers
        // Find rising edges on each layer
        
        for (int xx = 0; xx < heightmapRes; xx++) {
            for (int yy = 0; yy < heightmapRes; yy++) {
                if (!EvaluateNeighbours(xx, yy)) {
                    if (heights [xx,yy] != 1)
                        edgePoints.Add(new Vector3(xx, heights[xx, yy] * yScale, yy));
                }
            }
        }

        //edgePoints.Sort((x, y) => x.y.CompareTo(y.y));


        List<Vector3> processedPoints = new List<Vector3>();
        Vector3 nextPoint = new Vector3(-1, -1, -1);
        int maxAttempts = 100;
        int currentAttempts = 0;

        do {
            currentAttempts++;
            nextPoint = new Vector3(-1, -1, -1);
            // Find first point
            foreach (Vector3 point in edgePoints) {
                if (point.y < 0.3f && !processedPoints.Contains(point)) {
                    nextPoint = point;
                    break;
                }
            }

            bool willExit = false;
            while (nextPoint.z >= 0 || willExit) {

                List<Vector3> neighbours = new List<Vector3>();

                foreach (Vector3 otherPoint in edgePoints) {
                    if (nextPoint != otherPoint) {
                        if ((nextPoint - otherPoint).sqrMagnitude <= 2 && !processedPoints.Contains(otherPoint)) {
                            neighbours.Add(otherPoint);
                        }
                    }
                }
                
                if (neighbours.Count > 0) {
                    int northIndex, eastIndex, southIndex, westIndex;
                    int neIndex, seIndex, swIndex, nwIndex;
                    //var ;
                    //var southIndex = neighbours.IndexOf(nextPoint + new Vector3(0, 0, 1));

                    int selectIndex = -1;
                    if ((northIndex = neighbours.IndexOf(nextPoint + new Vector3(0, 0, 1))) != -1)
                        selectIndex = northIndex;
                    else if ((eastIndex = neighbours.IndexOf(nextPoint + new Vector3(1, 0, 0))) != -1)
                        selectIndex = eastIndex;
                    else if ((southIndex = neighbours.IndexOf(nextPoint + new Vector3(0, 0, -1))) != -1)
                        selectIndex = southIndex;
                    else if ((swIndex = neighbours.IndexOf(nextPoint + new Vector3(-1, 0, -1))) != -1)
                        selectIndex = swIndex;
                    else if ((westIndex = neighbours.IndexOf(nextPoint + new Vector3(-1, 0, 0))) != -1)
                        selectIndex = westIndex;
                    else if ((nwIndex = neighbours.IndexOf(nextPoint + new Vector3(-1, 0, 1))) != -1)
                        selectIndex = nwIndex;

                    if (selectIndex != -1) {
                        processedPoints.Add(nextPoint);
                        lines.Add(new Line(nextPoint, neighbours[selectIndex]));
                        nextPoint = neighbours[selectIndex];
                        willExit = false;
                        currentAttempts = 0;
                    } else {
                        willExit = true;
                    }
                    /*foreach(Vector3 neighbour in neighbours) {
                        if (neighbour.x)
                    }*/
                }
                //if (willExit) {
                //    nextPoint = new Vector3(-1, -1, -1);
                //}

            }
        } while (nextPoint.z >= 0 || currentAttempts > maxAttempts);


        /*foreach (Vector3 point in edgePoints) {
            if (point.y < 0.3f) {
                
                foreach (Vector3 otherPoint in edgePoints) {
                    if ((point - otherPoint).sqrMagnitude <= 2 && !processedPoints.Contains(otherPoint)) {
                    }
                }

            }
        }*/


                //Find edges from edge points
                

        /*

        Vector3 nextPoint;

        //float maxZ = 0;
        bool stillLooping = false;
        do {
            nextPoint = new Vector3(-1, -1, -1);


            // Find the next point to process, at start or if loop complete
            for (int i = 0; i < edgePoints.Count; i++) {
                if (!processedPoints.Contains(edgePoints[i])) {
                    nextPoint = edgePoints[i];
                    break;
                }
            }
            //bool startOfLoop = true;
            Vector3 startOfLoop = nextPoint;

            
            // Find the neighbours for this point
            List<Vector3> neighbours;// = new List<Vector3>();
            do {
                neighbours = new List<Vector3>();
                foreach (Vector3 otherPoint in edgePoints) {
                    if (otherPoint != nextPoint) {
                        // If the test point is within range, and not already attached to an edge
                        if ((nextPoint - otherPoint).sqrMagnitude <= 2 && !processedPoints.Contains(otherPoint)) {
                            neighbours.Add(otherPoint);
                        }
                    }
                }

                // Find the closest neighbour (preferencing axis aligned neighbours)
                neighbours.Sort((x, y) => (x - nextPoint).sqrMagnitude.CompareTo((y - nextPoint).sqrMagnitude));
                //if (!startOfLoop)
                processedPoints.Add(nextPoint);

                stillLooping = neighbours.Count > 0;

                if (stillLooping) {
                    lines.Add(new Line(nextPoint, neighbours[0]));
                    nextPoint = neighbours[0];
                } else {
                    if (nextPoint.z == heightmapRes-1) {
                        lines.Add(new Line(nextPoint, startOfLoop));
                        nextPoint = startOfLoop;
                        //stillLooping = true;
                    }

                    //lines.Add(new Line(nextPoint, startOfLoop));
                }
                
                
            } while (stillLooping);

            /*if (nextPoint.z >= 0)
                processedPoints.Add(nextPoint);*/
        /*} while (nextPoint.z >= 0);*/

        //for (int i = 0; i < edgePoints.Count; i ++) {
        /*for (int i = 0; i < 50; i++) {
            // Get the current point and create an empty list for neighbours
            Vector3 thisPoint = edgePoints[i];
            List<Vector3> neighbours = new List<Vector3>();

            // For all the other points on the graph
            foreach (Vector3 otherPoint in edgePoints) {
                if (otherPoint != thisPoint) {
                    // If the test point is within range, and not already attached to an edge
                    if ((thisPoint - otherPoint).sqrMagnitude <= 2 && !processedPoints.Contains(otherPoint)) {
                        neighbours.Add(otherPoint);
                    }
                }
            }
            neighbours.Sort((x, y) => (x - thisPoint).sqrMagnitude.CompareTo((y - thisPoint).sqrMagnitude));

            lines.Add(new Line(thisPoint, neighbours[0]));

            processedPoints.Add(thisPoint);

            if (neighbours.Count > 1)
                Debug.Log("bigh array");
        }*/

        /*foreach (Vector3 point in edgePoints) {
            Vector3 neighbour1 = Vector3.zero;// = new Vector3 (-;
            bool neighbour1Found = false;
            Vector3 neighbour2 = Vector3.zero;

            foreach (Vector3 otherPoint in edgePoints) {
                if (otherPoint != point) {

                    if ((point - otherPoint).sqrMagnitude <= 2) {
                        if (!neighbour1Found) {
                            neighbour1 = otherPoint;
                            neighbour1Found = true;
                        } else if (otherPoint != neighbour1) {
                            neighbour2 = otherPoint;
                            break;
                        }
                    }
                }
            }

            lines.Add(new Line(point, neighbour1));

            if (!IsEdgePoint(point))
                lines.Add(new Line(point, neighbour2));

        }*/

    }
    float yScale = 3;
    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.zero, 1);
        if (heights != null) {
            for (int xx = 0; xx < heights.GetLength(0); xx++) {
                for (int yy = 0; yy < heights.GetLength(0); yy++) {
                    float height = heights[xx, yy];
                    Gizmos.color = new Color(height, height, height);
                    Gizmos.DrawCube(new Vector3(xx, height * yScale, yy), Vector3.one * 0.2f);
                }
            }

            if (edgePoints != null) {
                foreach (Vector3 pos in edgePoints) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(pos, 0.25f);
                }
            }
            if (lines != null) {
                foreach(Line line in lines) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(line.from, line.to);
                }
            }

        }



        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
    }

    private bool IsEdgePoint(Vector3 point) {
        float xx = point.x;
        float yy = point.z;

        if (xx == 0 || xx == heights.GetLength(0) - 1) {
            return true;
        }
        else if (yy == 0 || yy == heights.GetLength(1) - 1) {
            return true;
        }
        else
            return false;
    }
}
