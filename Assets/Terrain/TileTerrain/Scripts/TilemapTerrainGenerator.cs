using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TilemapTerrainGenerator : MonoBehaviour
{
    MeshFilter filter;

    [SerializeField] public Texture2D heightmap;
    [SerializeField] private Mesh rampMesh;

    // Start is called before the first frame update
    void Start()
    {
        FindMeshFilter();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        foundValue = verts.BinarySearch(vertex, new VectorComparer());



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

}
