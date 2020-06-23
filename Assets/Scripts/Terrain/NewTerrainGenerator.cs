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
    List<(Vector3, Vector3, Color)> cubes;
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

    public void GenerateMesh(float smoothness) {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh newMesh = new Mesh();
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        cubes = new List<(Vector3, Vector3, Color)>();

        // Number of layers to check
        int layers = (int)Mathf.Log(heightRes - 1, 2) + 1;

        //List<((int, int), (int, int))> filledQuads = new List<((int, int), (int, int))>();

        //float steepness = 0;
        float steepness = smoothness;

        int layerSize = heightRes - 1;
        List<QuadtreeChunk> chunks = new List<QuadtreeChunk>();
        chunks.Add(new QuadtreeChunk(new Rect(0,0, layerSize, layerSize)));

        bool isFinished = false;

        while (!isFinished) {
            QuadtreeChunk chunk = chunks[0];
            Rect quad = chunk.Quad;
            float thisLayerSize = quad.width;

            if (CheckForPlane(heights, quad)) {
                
                float height = heights[(int)quad.xMin, (int)quad.yMin] * yScale;

                int indexLength = verts.Count;

                AddQuad(verts, indices, new Vector3(chunk.Quad.xMin, height, chunk.Quad.yMin),
                                        new Vector3(chunk.Quad.xMax, height, chunk.Quad.yMin),
                                        new Vector3(chunk.Quad.xMax, height, chunk.Quad.yMax),
                                        new Vector3(chunk.Quad.xMin, height, chunk.Quad.yMax));
            } else {

                float xMin = chunk.Quad.xMin;
                float xMax = chunk.Quad.xMax;
                float yMin = chunk.Quad.yMin;
                float yMax = chunk.Quad.yMax;

                Vector2[] cornerPos = { new Vector2(xMin, yMin),
                                        new Vector2(xMax, yMin),
                                        new Vector2(xMax, yMax),
                                        new Vector2(xMin, yMax)};

                if (thisLayerSize > 1) {
                    thisLayerSize /= 2;

                    Vector2 layerQuad = new Vector2(thisLayerSize, thisLayerSize);

                    chunks.Add(new QuadtreeChunk(new Rect(quad.min, layerQuad)));
                    chunks.Add(new QuadtreeChunk(new Rect(quad.min + new Vector2(0, thisLayerSize), layerQuad)));
                    chunks.Add(new QuadtreeChunk(new Rect(quad.min + new Vector2(thisLayerSize, thisLayerSize), layerQuad)));
                    chunks.Add(new QuadtreeChunk(new Rect(quad.min + new Vector2(thisLayerSize, 0), layerQuad)));
                } else {
                    float[] corners = { heights[(int)quad.xMin, (int)quad.yMin],
                                        heights[(int)quad.xMax, (int)quad.yMin],
                                        heights[(int)quad.xMax, (int)quad.yMax],
                                        heights[(int)quad.xMin, (int)quad.yMax]};

                    float lowVal = corners[0];
                    float highVal = corners[0];
                    float cornerSum = 0;

                    foreach (float corner in corners) {
                        if (corner < lowVal)
                            lowVal = corner;
                        if (corner > highVal)
                            highVal = corner;
                        cornerSum += corner;
                    }

                    
                    if (cornerSum - 2 * lowVal == 2 * highVal) {
                        // parrallel edges
                        // cubes.Add((new Vector3(quad.xMin + 0.5f, lowVal, quad.yMin + 0.5f), Vector3.one, Color.red));

                        int indexLength = verts.Count;

                        float xSteep = 0;
                        float ySteep = 0;

                        if (corners[1] == corners[2])
                            xSteep = steepness;
                        else
                            ySteep = steepness;

                        

                        AddQuad(verts, indices, new Vector3(xMin + xSteep, corners[0] * yScale, yMin + ySteep),
                                                new Vector3(xMax - xSteep, corners[1] * yScale, yMin + ySteep),
                                                new Vector3(xMax - xSteep, corners[2] * yScale, yMax - ySteep),
                                                new Vector3(xMin + xSteep, corners[3] * yScale, yMax - ySteep));

                        if (steepness != 0) {

                            if (xSteep > 0) {
                                AddQuad(verts, indices, new Vector3(xMin, corners[0] * yScale, yMin),
                                                        new Vector3(xMin + steepness, corners[0] * yScale, yMin),
                                                        new Vector3(xMin + steepness, corners[3] * yScale, yMax),
                                                        new Vector3(xMin, corners[3] * yScale, yMax));

                                AddQuad(verts, indices, new Vector3(xMax - steepness, corners[2] * yScale, yMin),
                                                        new Vector3(xMax, corners[2] * yScale, yMin),
                                                        new Vector3(xMax, corners[1] * yScale, yMax),
                                                        new Vector3(xMax - steepness, corners[1] * yScale, yMax));
                            }
                            if (ySteep > 0) {
                                AddQuad(verts, indices, new Vector3(xMin, corners[2] * yScale, yMax),
                                                        new Vector3(xMin, corners[2] * yScale, yMax - steepness),
                                                        new Vector3(xMax, corners[2] * yScale, yMax - steepness),
                                                        new Vector3(xMax, corners[2] * yScale, yMax));

                                AddQuad(verts, indices, new Vector3(xMin, corners[0] * yScale, yMin),
                                                        new Vector3(xMax, corners[0] * yScale, yMin),
                                                        new Vector3(xMax, corners[0] * yScale, yMin + steepness),
                                                        new Vector3(xMin, corners[0] * yScale, yMin + steepness));
                            }
                        }

                    } else {// if (cornerSum - lowVal == 3 * highVal) {

                        int rotIndex = 0;
                        bool isLowCorner = true;

                        for (int i = 0; i < corners.Count(); i++) {
                            if (cornerSum - lowVal == 3 * highVal) {
                                if (corners[i] == lowVal) {
                                    rotIndex = i;
                                    isLowCorner = true;
                                    break;
                                }
                            } else {
                                if (corners[i] == highVal) {
                                    rotIndex = i;
                                    isLowCorner = false;
                                    break;
                                }
                            }
                        }

                        float low;
                        float high;
                        if (isLowCorner) {
                            low = 0;
                            high = 1;
                        } else {
                            low = 1;
                            high = 0;
                        }
                        

                        Quaternion rotationQuat = Quaternion.AngleAxis(-rotIndex * 90, Vector3.up);

                        Vector3[] angleQuad = { new Vector3(0, low, steepness),
                                                new Vector3(steepness, low, 0),
                                                new Vector3(1-steepness, high, 0),
                                                new Vector3(0, high, 1-steepness)};

                        Vector3[] upperQuad = { new Vector3(0, high, 1-steepness),
                                                new Vector3(1-steepness, high, 0),
                                                new Vector3(1, high, 0),
                                                new Vector3(0, high, 1)};

                        Vector3[] lowerTriangle = { new Vector3(0, low, 0),
                                                    new Vector3(steepness, low, 0),
                                                    new Vector3(0, low, steepness)};

                        Vector3[] upperTriangle = { new Vector3(1, high, 1),
                                                    new Vector3(0, high, 1),
                                                    new Vector3(1, high, 0)};

                        for (int i = 0; i < angleQuad.Count(); i++) {
                            angleQuad[i] -= new Vector3(0.5f, 0, 0.5f);
                            angleQuad[i] = rotationQuat * angleQuad[i];
                            angleQuad[i].y *= (highVal - lowVal) * yScale;
                            angleQuad[i] += new Vector3(chunk.Quad.xMin + 0.5f, lowVal * yScale, chunk.Quad.yMin + 0.5f);

                            upperQuad[i] -= new Vector3(0.5f, 0, 0.5f);
                            upperQuad[i] = rotationQuat * upperQuad[i];
                            upperQuad[i].y *= (highVal - lowVal) * yScale;
                            upperQuad[i] += new Vector3(chunk.Quad.xMin + 0.5f, lowVal * yScale, chunk.Quad.yMin + 0.5f);
                        }

                        for (int i = 0; i < lowerTriangle.Count(); i++) {
                            lowerTriangle[i] -= new Vector3(0.5f, 0, 0.5f);
                            lowerTriangle[i] = rotationQuat * lowerTriangle[i];
                            lowerTriangle[i].y *= (highVal - lowVal) * yScale;
                            lowerTriangle[i] += new Vector3(chunk.Quad.xMin + 0.5f, lowVal * yScale, chunk.Quad.yMin + 0.5f);

                            upperTriangle[i] -= new Vector3(0.5f, 0, 0.5f);
                            upperTriangle[i] = rotationQuat * upperTriangle[i];
                            upperTriangle[i].y *= (highVal - lowVal) * yScale;
                            upperTriangle[i] += new Vector3(chunk.Quad.xMin + 0.5f, lowVal * yScale, chunk.Quad.yMin + 0.5f);
                        }

                        AddQuad(verts, indices, angleQuad[0], angleQuad[1], angleQuad[2], angleQuad[3]);
                        AddQuad(verts, indices, upperQuad[0], upperQuad[1], upperQuad[2], upperQuad[3]);
                        AddTriangle(verts, indices, lowerTriangle[0], lowerTriangle[1], lowerTriangle[2]);
                        AddTriangle(verts, indices, upperTriangle[0], upperTriangle[1], upperTriangle[2]);
                        //}

                    //} else { 
                        // Three low corners
                        //cubes.Add((new Vector3(quad.xMin + 0.5f, lowVal, quad.yMin + 0.5f), Vector3.one, Color.cyan));
                    }

                }
            }

            chunks.Remove(chunk);

            isFinished = chunks.Count == 0;

        }

        


        newMesh.SetVertices(verts);
        newMesh.SetTriangles(indices, 0);


        newMesh.RecalculateNormals();
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
                float offset = 0;
                /*if (Random.value > 0.95f) {
                    offset = 0.25f;
                }*/
                heights[xx, yy] = height + offset;
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
        //Debug.Log(Mathf.Log(heightRes-1, 2));
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

    public int RotateNumber(int value, int itterator, int max) {

        if (Mathf.Abs(itterator) > max) {
            max = itterator % max;
        }

        if (value + itterator < max && value + itterator >= 0) {
            // Value in range
            return value + itterator;
        } else if (value + itterator >= max) {
            // Value over range
            return value + itterator - max;
        } else {
            // Value under range
            return value + itterator + max;
        }
    }

    private bool CheckForPlane(float[,] heights, Rect quad) {
        //return CheckForPlane(heights, ((int)quad.xMin, (int)quad.yMin), ((int)(quad.xMin + quad.xMax), (int)(quad.yMin + quad.yMax)));
        return CheckForPlane(heights, ((int)quad.xMin, (int)quad.yMin), ((int)(quad.xMax), (int)(quad.yMax)));
    }

    private void AddQuad(List<Vector3>verts,List<int>indices, Vector3 xMinyMin, Vector3 xMaxyMin, Vector3 xMinyMax, Vector3 xMaxyMax) {
    
        int indexLength = verts.Count;

        //float angleOneDist = (xMinyMin - xMaxyMax).magnitude;
        //float angleTwoDist = (xMaxyMin - xMinyMax).magnitude;


        verts.Add(xMinyMin);
        verts.Add(xMaxyMin);
        verts.Add(xMinyMax);
        verts.Add(xMaxyMax);

        //if (angleOneDist <= angleTwoDist) {
        indices.Add(indexLength);
        indices.Add(indexLength + 2);
        indices.Add(indexLength + 1);

        indices.Add(indexLength);
        indices.Add(indexLength + 3);
        indices.Add(indexLength + 2);
        //}
    }

    private void AddTriangle(List<Vector3> verts, List<int> indices, Vector3 corner1, Vector3 corner2, Vector3 corner3) {

        int indexLength = verts.Count;

        verts.Add(corner1);
        verts.Add(corner2);
        verts.Add(corner3);

        //if (angleOneDist <= angleTwoDist) {
        indices.Add(indexLength);
        indices.Add(indexLength + 2);
        indices.Add(indexLength + 1);
    }


    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.zero, 1);

        if (cubes != null) {
            //Gizmos.color = Color.cyan;
            foreach((Vector3, Vector3, Color) cube in cubes) {
                Gizmos.color = cube.Item3;
                Gizmos.DrawCube(cube.Item1, cube.Item2);
            }
        }

        if (heights != null) {


            for (int xx = 0; xx < heights.GetLength(0); xx++) {
                for (int yy = 0; yy < heights.GetLength(0); yy++) {
                    float height = heights[xx, yy];
                    Gizmos.color = new Color(height, height, height);
                    Gizmos.DrawCube(new Vector3(xx, height * yScale, yy), Vector3.one * 0.2f);
                }
            }

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

    private class QuadtreeChunk {

        private Rect quad;

        public QuadtreeChunk(Rect quad) {
            this.Quad = quad;
        }

        public Rect Quad { get => quad; set => quad = value; }
    }
}
