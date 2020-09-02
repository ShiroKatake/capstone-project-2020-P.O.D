using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TilemapTerrainGenerator : MonoBehaviour
{
    MeshFilter filter;

    [SerializeField] public Texture2D heightmapTexture;
    [SerializeField] private GameObject straightRamp_Object;
    [SerializeField] private GameObject innerRamp_Object;
    [SerializeField] private GameObject outerRamp_Object;

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

        for (int xx = 0; xx < image.width; xx += 2) {
            for (int yy = 0; yy < image.height; yy += 2) {
                Color pixelCol = image.GetPixel(xx, yy);

                // 0 indexed, anti-clockwise from lower left
                float[] corners = new float[4];

                if (pixelCol.g > 0) {
                    corners[0] = pixelCol.g;
                    corners[1] = image.GetPixel(xx + 1, yy).g;
                    corners[2] = image.GetPixel(xx + 1, yy + 1).g;
                    corners[3] = image.GetPixel(xx, yy + 1).g;


                    // Find the minimum and maximum heights
                    float minHeight = corners[0];
                    float maxHeight = corners[0];
                    float cornerSum = 0;
                    foreach (float corner in corners) {
                        if (corner < minHeight)
                            minHeight = corner;
                        if (corner > maxHeight)
                            maxHeight = corner;
                        cornerSum += corner;
                    }

                    float heightDelta = maxHeight - minHeight;

                    GameObject cliffObj = default;
                    float angle = 0;

                    if (Mathf.Approximately(cornerSum - 2 * minHeight, 2 * maxHeight)) {
                        // If quad has straight parralel edges
                        cliffObj = straightRamp_Object;

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
                            cliffObj = innerRamp_Object;

                            if (corners[0] == maxHeight)
                                angle = -90;
                            else if (corners[1] == maxHeight)
                                angle = 180;
                            else if (corners[2] == maxHeight)
                                angle = 90;


                        } else {
                            // If quad is an outer corner
                            cliffObj = outerRamp_Object;

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
                // heights are average of 4 pixels surrounding a point (intersection of 4 pixels)

                float sum = 0;
                int numSamples = 0;

                sum += image.GetPixel(xx, yy).r;
                numSamples += 1;


                float height = sum / ((float)numSamples);

                spheres.Add((new Vector3(xx, height, yy), 0.2f, image.GetPixel(xx,yy)));



                heights[xx, yy] = height;

            }
        }

        heightmap = heights;

    }

    private bool IsInRange((int,int) position, (int,int) delta, float maxSize) {
        if (position.Item1 + delta.Item1 >= 0 && position.Item1 + delta.Item1 < maxSize)
            if (position.Item2 + delta.Item2 >= 0 && position.Item2 + delta.Item2 < maxSize)
                return true;
        return false;
    }

    public void GenerateMesh() {
        FindMeshFilter();
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //HashSet<Vector3> verts = new HashSet<Vector3>();

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

    public void GenerateFlatMesh((int, int) size) {
        FindMeshFilter();
        Mesh mesh = new Mesh();

        //HashSet<Vector3> verts = new HashSet<Vector3>();

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        //List<Vector3> verts = new List<Vector3>();

        double time = EditorApplication.timeSinceStartup;

        for (int xx = 0; xx < size.Item1; xx++) {
            for (int yy = 0; yy < size.Item2; yy++) {
                int[] tris = new int[6];

                tris[0] = AddVertex(verts, new Vector3(xx, 0, yy));
                tris[1] = AddVertex(verts, new Vector3(xx, 0, yy + 1));
                tris[2] = AddVertex(verts, new Vector3(xx+1, 0, yy+1));


                tris[3] = AddVertex(verts, new Vector3(xx, 0, yy));
                tris[4] = AddVertex(verts, new Vector3(xx+1, 0, yy+1));
                tris[5] = AddVertex(verts, new Vector3(xx+1, 0, yy));

                indices.AddRange(tris);
            }
        }

        mesh.SetVertices(verts);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        filter.mesh = mesh;

        double delay = EditorApplication.timeSinceStartup - time;
        Debug.Log(delay);
    }

    private void FindMeshFilter() {
        if (filter == null) {
            filter = GetComponent<MeshFilter>();
        }
    }

    //private void AddQuad

    /// <summary>
    /// Adds a vertex to the verts if it is not yet present,
    /// Uses existing verts if already present.
    /// </summary>
    /// <param name="verts">List of vertices to check</param>
    /// <param name="vertex">Vertex to check against</param>
    /// <returns></returns>
    private int AddVertex(List<Vector3> verts, Vector3 vertex) {

        int foundValue = -1;
        //verts.fin
        //foundValue = verts.BinarySearch(vertex, new VectorComparer());



        /*for(int i = 0; i < verts.Count; i ++) {
            if (verts[i] == vertex) {
                foundValue = i;
                break;
            }
                //return i;
        }
        */

        if (foundValue < 0) {
            verts.Add(vertex);
            foundValue = verts.Count - 1;
        }

        
        return foundValue;
    }

    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.zero, 1);

        if (spheres != null) {
            foreach (var set in spheres) {
                Gizmos.color = set.Item3;
                Gizmos.DrawSphere(set.Item1, set.Item2);
            }
        }
    }

}
