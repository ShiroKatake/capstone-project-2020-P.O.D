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
    [SerializeField] private Mesh rampMesh;

    private float [,] heightmap;


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

                /*if (IsInRange((xx,yy), (-1,0), image.width)) {
                    float val = image.GetPixel(xx - 1, yy).r;
                    if (val > 0) {
                        sum += val;
                        numSamples += 1;
                    }
                }

                if (IsInRange((xx, yy), (0, -1), image.width)) {
                    float val = image.GetPixel(xx, yy-1).r;
                    if (val > 0) {
                        sum += val;
                        numSamples += 1;
                    }
                }

                if (IsInRange((xx, yy), (-1, -1), image.width)) {
                    float val = image.GetPixel(xx - 1, -1).r;
                    if (val > 0) {
                        sum += val;
                        numSamples += 1;
                    }
                }*/

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

        //HashSet<Vector3> verts = new HashSet<Vector3>();

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        for (int xx = 0; xx < heightmap.GetLength(0)-1; xx++) {
            for (int yy = 0; yy < heightmap.GetLength(1)-1; yy++) {
                int[] tris = new int[6];

                float height = heightmap[xx, yy] * 2;
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
