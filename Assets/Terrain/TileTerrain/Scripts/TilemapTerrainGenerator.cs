using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityMeshSimplifier;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TilemapTerrainGenerator : MonoBehaviour
{
    MeshFilter filter;
    MeshCollider collider; 

    [SerializeField] public Texture2D heightmapTexture;
    [SerializeField] private GameObject straightCliff_Object;
    [SerializeField] private GameObject innerCliff_Object;
    [SerializeField] private GameObject outerCliff_Object;

    [SerializeField] private GameObject RampStraight_Object;
    [SerializeField] private GameObject RampLeft_Object;
    [SerializeField] private GameObject RampRight_Object;
    


    private float [,] heightmap;

    float yScale = 2;

    private List<(Vector3, float, Color)> spheres;

    // Start is called before the first frame update
    void Start()
    {
        FindMeshFilter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateCliffsFromImage(Texture2D image) {

        List<(int, int)> processedRamps = new List<(int, int)>();

        for (int xx = 0; xx < image.width; xx += 2) {
            for (int yy = 0; yy < image.height; yy += 2) {
                Color pixelCol = image.GetPixel(xx, yy);

                // 0 indexed, anti-clockwise from lower left
                float[] corners = new float[4];

                // Create cliffs where green pixels exist
                if (pixelCol.g > 0) {
                    corners[0] = pixelCol.g;
                    corners[1] = image.GetPixel(xx + 1, yy).g;
                    corners[2] = image.GetPixel(xx + 1, yy + 1).g;
                    corners[3] = image.GetPixel(xx, yy + 1).g;


                    // Find the minimum and maximum heights
                    float minHeight = Mathf.Min(corners);
                    float maxHeight = Mathf.Max(corners);
                    float cornerSum = corners.Sum();

                    float heightDelta = maxHeight - minHeight;

                    GameObject cliffObj = default;
                    float angle = 0;

                    if (Mathf.Approximately(cornerSum - 2 * minHeight, 2 * maxHeight)) {
                        // If quad has straight parralel edges
                        cliffObj = straightCliff_Object;

                        // If X axis has same values, and lower edge raised
                        if (corners[0] == corners[1] && corners[0] == maxHeight) {
                            angle = 180;
                        }
                        else
                        // If Y axis has same values
                        if (corners[0] == corners[3]) {
                            // If left edge is raised
                            if (corners[0] == maxHeight) {
                                angle = -90;
                            }
                            else {
                                angle = 90;
                            }
                        }

                    }
                    else {

                        if (Mathf.Approximately(cornerSum - maxHeight, 3 * minHeight)) {
                            // If quad is an inner corner
                            cliffObj = innerCliff_Object;

                            if (corners[0] == maxHeight)
                                angle = -90;
                            else if (corners[1] == maxHeight)
                                angle = 180;
                            else if (corners[2] == maxHeight)
                                angle = 90;


                        } else {
                            // If quad is an outer corner
                            cliffObj = outerCliff_Object;

                            if (corners[0] == minHeight)
                                angle = 90;
                            else if (corners[2] == minHeight)
                                angle = -90;
                            else if (corners[3] == minHeight)
                                angle = 180;
                        }

                    }

                    var newCliff = Instantiate(cliffObj, transform);
                    newCliff.transform.localPosition = new Vector3(xx+1, minHeight * yScale, yy+1);
                    newCliff.transform.localScale = new Vector3(1, heightDelta * yScale, 1);
                    newCliff.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);

                }

                

                // Create ramps where blue pixels exist
                if (pixelCol.b > 0) {
                    if (!processedRamps.Contains((xx,yy))) {
                        GameObject rampShape = RampLeft_Object;

                        float thisHeight = pixelCol.b;

                        float[] edgeHeights = new float[2];
                        edgeHeights[0] = image.GetPixel(xx + 2, yy).b;  // Right
                        edgeHeights[1] = image.GetPixel(xx, yy + 2).b;  // Up

                        // Because the image is sampled from bottom left, going up and right, any matches
                        // will be found either to the right or up from the test pixel

                        // Find the edge index that corresponds to the paired tile
                        int pairIndex = -1;
                        for(int i = 0; i < edgeHeights.Length; i ++) {
                            if (edgeHeights[i] != 0 && edgeHeights[i] != thisHeight) {
                                pairIndex = i;
                            }
                        }
                        if (pairIndex < 0) {
                            Debug.LogError("Image format incorrect. No pair for ramp at (" + xx +","+yy+") was found");
                        }

                        Vector3 positionOffset = default;

                        float minHeight = Mathf.Min(thisHeight, edgeHeights[pairIndex]);
                        float maxHeight = Mathf.Max(thisHeight, edgeHeights[pairIndex]);
                        float heightDelta = maxHeight - minHeight;

                        float angle = 0;

                        processedRamps.Add((xx, yy));

                        // Apply position and rotation offsets
                        switch (pairIndex) {
                            case 0:     // Right
                                processedRamps.Add((xx + 2, yy));
                                positionOffset = new Vector3(2, minHeight * yScale, 1);

                                float upGreenCheck = -1;
                                float downGreenCheck = -1;

                                if (Mathf.Approximately(minHeight, thisHeight)) {
                                    // If this sample is the low pixel (Ramp going down left (-X))
                                    angle = -90;
                                    upGreenCheck = image.GetPixel(xx + 2, yy + 2).g;
                                    downGreenCheck = image.GetPixel(xx + 2, yy - 2).g;
                                } else {
                                    // If this sample is the high pixel (Ramp going down right (+X))
                                    angle = 90;
                                    downGreenCheck = image.GetPixel(xx, yy + 2).g;
                                    upGreenCheck = image.GetPixel(xx, yy - 2).g;
                                }

                                if (upGreenCheck > 0)
                                    rampShape = RampLeft_Object;
                                else if (downGreenCheck > 0)
                                    rampShape = RampRight_Object;
                                else
                                    rampShape = RampStraight_Object;

                                break;
                            case 1:
                                processedRamps.Add((xx, yy + 2));
                                positionOffset = new Vector3(1, minHeight * yScale, 2);
                                float leftGreenCheck = -1;
                                float rightGreenCheck = -1;

                                if (Mathf.Approximately(minHeight, thisHeight)) {
                                    // If this sample is the low pixel (ramp going down down (-Z))
                                    angle = 180;
                                    leftGreenCheck = image.GetPixel(xx - 2, yy + 2).g;
                                    rightGreenCheck = image.GetPixel(xx + 2, yy + 2).g;
                                } else {
                                    angle = 0;
                                    leftGreenCheck = image.GetPixel(xx + 2, yy).g;
                                    rightGreenCheck = image.GetPixel(xx - 2, yy).g;
                                }

                                if (leftGreenCheck > 0)
                                    rampShape = RampLeft_Object;
                                else if (rightGreenCheck > 0)
                                    rampShape = RampRight_Object;
                                else
                                    rampShape = RampStraight_Object;

                                break;
                        }

                        GameObject newRamp = Instantiate(rampShape, transform);
                        newRamp.transform.localPosition = new Vector3(xx, 0, yy) + positionOffset;
                        newRamp.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
                        newRamp.transform.localScale = new Vector3(1, heightDelta * yScale, 1);
                        
                    }
                }
            }
        }
    }


    public void ConvertImageToHeightmap(Texture2D image) {
        spheres = new List<(Vector3, float, Color)>();

        int resolution = image.width + 1;
        float[,] heights = new float[resolution, resolution];

        for (int xx = 0; xx < image.width; xx ++) {
            for (int yy = 0; yy < image.height; yy++) {
                // xx,yy is pixel position.

                float sum = 0;

                sum += image.GetPixel(xx, yy).r;

                float height = sum;

                heights[xx, yy] = height;

            }
        }

        heightmap = heights;

    }

    public void GenerateMesh() {
        FindMeshFilter();
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        for (int xx = 0; xx < heightmap.GetLength(0)-1; xx++) {
            for (int yy = 0; yy < heightmap.GetLength(1)-1; yy++) {
                int[] tris = new int[6];

                float height = heightmap[xx, yy] * yScale;

                if (height > 0) {

                    tris[0] = AddVertex(verts, new Vector3(xx, height, yy));
                    tris[1] = AddVertex(verts, new Vector3(xx, height, yy + 1));
                    tris[2] = AddVertex(verts, new Vector3(xx + 1, height, yy + 1));


                    tris[3] = AddVertex(verts, new Vector3(xx, height, yy));
                    tris[4] = AddVertex(verts, new Vector3(xx + 1, height, yy + 1));
                    tris[5] = AddVertex(verts, new Vector3(xx + 1, height, yy));

                    indices.AddRange(tris);
                }
            }
        }

        mesh.SetVertices(verts);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        mesh.RecalculateNormals();
        filter.mesh = mesh;
    }

   

    public void OptimiseMesh() {
        FindMeshFilter();
        Mesh m = filter.sharedMesh;

        m.Weld(Mathf.Epsilon, 64);
        m.Simplify();

        m.RecalculateNormals();

        filter.sharedMesh = m;
    }

    public void SetCollisionMesh() {
        FindMeshFilter();
        FindMeshCollider();
        collider.sharedMesh = filter.sharedMesh;
    }

    private void FindMeshFilter() {
        if (filter == null) {
            filter = GetComponent<MeshFilter>();
        }
    }
    
    private void FindMeshCollider() {
        if (collider == null) {
            collider = GetComponent<MeshCollider>();
        }
    }


    //private void AddQuad

    /// <summary>
    /// Adds a vertex to the verts if it is not yet present,
    /// 
    /// TODO: Currently removed due to excessive generation time:
    /// Uses existing verts if already present.
    /// </summary>
    /// <param name="verts">List of vertices to check</param>
    /// <param name="vertex">Vertex to check against</param>
    /// <returns></returns>
    private int AddVertex(List<Vector3> verts, Vector3 vertex) {

        int foundValue = -1;


        if (foundValue < 0) {
            verts.Add(vertex);
            foundValue = verts.Count - 1;
        }

        
        return foundValue;
    }

}
