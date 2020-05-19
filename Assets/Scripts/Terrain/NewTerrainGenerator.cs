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
    List<Vector3> cubes;
    private int heightRes;
    private float yScale = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHeightRes(int res) {
        heightRes = res;
    }

    public void GenerateMesh() {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh newMesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        cubes = new List<Vector3>();

        // Number of layers to check
        int layers = (int)Mathf.Log(heightRes - 1, 2) + 1;

        List<((int, int), (int, int))> filledQuads = new List<((int, int), (int, int))>();


        int layerSize = heightRes - 1;
        int quadStartX = 0;
        int quadStartY = 0;

        for (int i = 0; i < layers; i++) {
            //int checksThisLayer = (i + 1) * (i + 1);
            int thisLayerAxisLength = (int)Mathf.Pow(2,(i));
            //Debug.Log("This Layer: " + i + "\nLength: " + thisLayerAxisLength);
            //for (int j = 0; j < checksThisLayer; j++) {
            for (int xx = 0; xx < thisLayerAxisLength; xx++) {
                for (int yy = 0; yy < thisLayerAxisLength; yy++) {

                    quadStartX = xx * layerSize;
                    quadStartY = yy * layerSize;
                    bool isFilled = false;
                    foreach (var field in filledQuads) {
                        if (quadStartX >= field.Item1.Item1 && quadStartX + layerSize <= field.Item2.Item1) {
                            if (quadStartY >= field.Item1.Item2 && quadStartY  + layerSize <= field.Item2.Item2) {
                                isFilled = true;
                                break;
                            }
                        }
                    }
                    if (!isFilled) {

                        bool isPlane = CheckForPlane(heights, (quadStartX, quadStartY), (quadStartX + layerSize, quadStartY + layerSize));

                        // Fill flat planes
                        if (isPlane) {
                            int indexLength = verts.Count;

                            filledQuads.Add(((quadStartX, quadStartY), (quadStartX + layerSize, quadStartY + layerSize)));

                            verts.Add(new Vector3(quadStartX, heights[quadStartX, quadStartY] * yScale, quadStartY));
                            verts.Add(new Vector3(quadStartX + layerSize, heights[quadStartX + layerSize, quadStartY] * yScale, quadStartY));
                            verts.Add(new Vector3(quadStartX + layerSize, heights[quadStartX, quadStartY] * yScale, quadStartY + layerSize));
                            verts.Add(new Vector3(quadStartX, heights[quadStartX, quadStartY] * yScale, quadStartY + layerSize));



                            indices.Add(indexLength);
                            indices.Add(indexLength + 2);
                            indices.Add(indexLength + 1);

                            indices.Add(indexLength);
                            indices.Add(indexLength + 3);
                            indices.Add(indexLength + 2);
                        }

                        /* SHOULD ONLY BE TRIGGERED AT SMALLEST CHUNK SIZE */
                        if (!isPlane) {
                            float[] corners = {heights[quadStartX, quadStartY],
                                            heights[quadStartX+1, quadStartY],
                                            heights[quadStartX+1, quadStartY+1],
                                            heights[quadStartX, quadStartY+1]};

                            bool isLowerXEdge = (corners[0] - corners[1]) == 0;
                            bool isUpperXEdge = (corners[0] - corners[3]) == 0;

                            // Fill Straight edges
                            if (isLowerXEdge && isUpperXEdge) {
                                int indexLength = verts.Count;

                                verts.Add(new Vector3(quadStartX, corners[0] * yScale, quadStartY));
                                verts.Add(new Vector3(quadStartX + 1, corners[1] * yScale, quadStartY));

                                verts.Add(new Vector3(quadStartX, corners[0] * yScale, quadStartY + 0.3f));
                                verts.Add(new Vector3(quadStartX + 1, corners[1] * yScale, quadStartY + 0.3f));


                                indices.Add(indexLength);
                                indices.Add(indexLength + 2);
                                indices.Add(indexLength + 1);

                                indices.Add(indexLength + 1);
                                indices.Add(indexLength + 2);
                                indices.Add(indexLength + 3);
                            }




                            // Fill corner triangles
                        }
                    }
                }
            }

            layerSize /= 2;

        }


        newMesh.SetVertices(verts);
        newMesh.SetTriangles(indices, 0);

        

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
        //int heightmapRes = 24 + 1;
        

        // Create raw heightmap
        heights = new float[heightRes, heightRes];

        edgePoints = new List<Vector3>();
        lines = new List<Line>();

        //heightex = new Texture2D(heightmapRes, heightmapRes);
        for (int xx = 0; xx < heightRes; xx++) {
            for (int yy = 0; yy < heightRes; yy++) {
                Vector2 pos = new Vector2(xx, yy);
                float height;

                height = Mathf.PerlinNoise((float)xx * noiseScale + 0.1f, (float)yy * noiseScale + 0.1f);
                height = curve.Evaluate(height);

                heights[xx, yy] = height;
            }
        }

        // Find edge points between layers
        // Find rising edges on each layer
        
        for (int xx = 0; xx < heightRes; xx++) {
            for (int yy = 0; yy < heightRes; yy++) {
                if (!EvaluateNeighbours(xx, yy)) {
                    if (heights [xx,yy] != 1)
                        edgePoints.Add(new Vector3(xx, heights[xx, yy] * yScale, yy));
                }
            }
        }
        //Mathf.Log()
        Debug.Log(Mathf.Log(heightRes-1, 2));
        //Debug.Log(Mathf.Log(heightmapRes, 2));

    }

    private bool CheckForPlane(float[,] heights, (int,int) start, (int,int) end) {
        int width  = end.Item1 - start.Item1;
        int height = end.Item2 - start.Item2;
        bool isPlane = true;

        float corner1 = heights[start.Item1, start.Item2];
        float corner2 = heights[start.Item1 + width, start.Item2];
        float corner3 = heights[start.Item1 + width, start.Item2 + height];
        float corner4 = heights[start.Item1, start.Item2 + height];
        //Check if corners make a plane
        if (corner1 != corner2  || corner1 != corner3 || corner1 != corner4) {
            // If any of the corners are not equal
            return false;
        } else {
            // if all corners are level, check all the interstitial points for levelness
            for (int xx = 0; xx < width+1; xx++) {
                for (int yy = 0; yy < height+1; yy++) {

                    if (heights[start.Item1 + xx, start.Item2 + yy] != corner1) {
                        return false;
                    }

                }
            }
        }

        return isPlane;

    }

    //private bool CheckFor
    

    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.zero, 1);
        if (heights != null) {


            /*for (int xx = 0; xx < heights.GetLength(0); xx++) {
                for (int yy = 0; yy < heights.GetLength(0); yy++) {
                    float height = heights[xx, yy];
                    Gizmos.color = new Color(height, height, height);
                    Gizmos.DrawCube(new Vector3(xx, height * yScale, yy), Vector3.one * 0.2f);
                }
            }*/

            /*if (edgePoints != null) {
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
            }*/

        }



        //Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
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
