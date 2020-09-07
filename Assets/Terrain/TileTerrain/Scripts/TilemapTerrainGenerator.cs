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
    MeshRenderer renderer;

    [SerializeField] public TilemapPrefabData prefabData;

    [SerializeField] public Texture2D heightmapTexture;
    [SerializeField] public Texture2D itemTexture;
    [SerializeField] public Texture2D resourceTexture;

    [SerializeField] public int blurDistance;
    //[SerializeField] private GameObject straightCliff_Object;
    //[SerializeField] private GameObject innerCliff_Object;
    //[SerializeField] private GameObject outerCliff_Object;

    //[SerializeField] private GameObject RampStraight_Object;
    //[SerializeField] private GameObject RampLeft_Object;
    //[SerializeField] private GameObject RampRight_Object;
    


    private float [,] heightmap;

    float yScale = 2;

    private List<(Vector3, float, Color)> spheres;

    // Start is called before the first frame update
    void Start()
    {
        FindMeshFilter();

        GenerateResourceMasks(1024);
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
                        cliffObj = prefabData.straightCliff_Prefab;

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
                            cliffObj = prefabData.innerCornerCliff_Prefab;

                            if (corners[0] == maxHeight)
                                angle = -90;
                            else if (corners[1] == maxHeight)
                                angle = 180;
                            else if (corners[2] == maxHeight)
                                angle = 90;


                        } else {
                            // If quad is an outer corner
                            cliffObj = prefabData.outerCornerCliff_Prefab;

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
                        GameObject rampShape = prefabData.leftRamp_Prefab;

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
                                    rampShape = prefabData.leftRamp_Prefab;
                                else if (downGreenCheck > 0)
                                    rampShape = prefabData.rightRamp_Prefab;
                                else
                                    rampShape = prefabData.straightRamp_Prefab;

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
                                    rampShape = prefabData.leftRamp_Prefab;
                                else if (rightGreenCheck > 0)
                                    rampShape = prefabData.rightRamp_Prefab;
                                else
                                    rampShape = prefabData.straightRamp_Prefab;

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

    public void CreateMapItemsFromImage(Texture2D image) {
        List<Color> valuesOnMap = new List<Color>();

        for (int xx = 0; xx < image.width; xx += 1) {
            for (int yy = 0; yy < image.height; yy += 1) {
                Color pixel = image.GetPixel(xx, yy);
                if (!valuesOnMap.Contains(pixel)) {
                    valuesOnMap.Add(pixel);
                }

                if (pixel == new Color(0, 0.8f, 0.8f)) {
                    //if (pixel.isEqual(new Color(0, 0.8f, 0.8f), 0.1f)) {
                    GameObject mineral = Instantiate(prefabData.mineral_Prefab, transform);

                    mineral.transform.localPosition = new Vector3(xx+0.5f, heightmap[xx, yy] * yScale, yy+0.5f);
                }

            }
        }

        string colours = "";

        foreach (Color col in valuesOnMap) {
            colours += "\n" +col.ToString();
        }

        Debug.Log(colours);
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


    public void GenerateResourceMasks(int resolution) {
        FindMeshRenderer();

        List<Vector2> organicsTiles = new List<Vector2>();
        List<Vector2> waterTiles = new List<Vector2>();
        List<Vector2> naturalGasTiles = new List<Vector2>();

        float scaleFactor =  (float)resolution / resourceTexture.width;


        for (int xx = 0; xx < resourceTexture.width; xx += 1) {
            for (int yy = 0; yy < resourceTexture.height; yy += 1) {
                Color pixel = resourceTexture.GetPixel(xx, yy);

                if (pixel.r > 0) {
                    // Organics
                    organicsTiles.Add(new Vector2(xx, yy));
                } else
                if (pixel.g > 0) {
                    // Natural Gas
                    naturalGasTiles.Add(new Vector2(xx, yy));
                } else
                if (pixel.b > 0) {
                    // Water / Ice
                    waterTiles.Add(new Vector2(xx, yy));
                }

            }
        }

        // Create empty masks
        float[,] organicsIntensity = new float[resolution, resolution];
        float[,] naturalGasIntensity = new float[resolution, resolution];
        float[,] waterIntensity = new float[resolution, resolution];

        // Clear the masks
        for (int xx = 0; xx < resolution; xx += 1) {
            for (int yy = 0; yy < resolution; yy += 1) {
                organicsIntensity[xx, yy] = 0;
                naturalGasIntensity[xx, yy] = 0;
                waterIntensity[xx, yy] = 0;
            }
        }
        /**********************
         * Organics
         * 
         */
        foreach ( Vector2 tile in organicsTiles) {
            //organicsIntensity[(int)(tile.x * scaleFactor), (int)(tile.y * scaleFactor)] = 1;

            int scaleFac = (int)scaleFactor;
            int tileX = (int)Mathf.Floor(tile.x * scaleFactor) - scaleFac/2;
            int tileY = (int)Mathf.Floor(tile.y * scaleFactor) - scaleFac/2;

            for (int xx = tileX-blurDistance; xx < tileX + scaleFac + blurDistance + 1; xx++) {
                for (int yy = tileY-blurDistance; yy < tileY + scaleFac + blurDistance+ 1; yy++) {
                    float intensity = 1;

                    float xOffset = xx - tileX;
                    float yOffset = yy - tileY;

                    
                    if (xOffset < 0)
                        intensity *= 1 - Mathf.Abs(xOffset / (float)blurDistance);
                    if (xOffset > scaleFac)
                        intensity *= 1 -Mathf.Abs((xOffset-scaleFac) / (float)blurDistance);
                    
                    if (yOffset < 0)
                        intensity *= 1 - Mathf.Abs(yOffset / (float)blurDistance);
                    if (yOffset > scaleFac)
                        intensity *= 1 - Mathf.Abs((yOffset - scaleFac) / (float)blurDistance);

                    organicsIntensity[xx, yy] = Mathf.Max(intensity, organicsIntensity[xx,yy]);
                }
            }
        }

        /**********************
         * Natural Gas
         * 
         */
        foreach (Vector2 tile in naturalGasTiles) {

            int scaleFac = (int)scaleFactor;
            int tileX = (int)Mathf.Floor(tile.x * scaleFactor) - scaleFac / 2;
            int tileY = (int)Mathf.Floor(tile.y * scaleFactor) - scaleFac / 2;

            for (int xx = tileX - blurDistance; xx < tileX + scaleFac + blurDistance + 1; xx++) {
                for (int yy = tileY - blurDistance; yy < tileY + scaleFac + blurDistance + 1; yy++) {
                    float intensity = 1;

                    float xOffset = xx - tileX;
                    float yOffset = yy - tileY;


                    if (xOffset < 0)
                        intensity *= 1 - Mathf.Abs(xOffset / (float)blurDistance);
                    if (xOffset > scaleFac)
                        intensity *= 1 - Mathf.Abs((xOffset - scaleFac) / (float)blurDistance);

                    if (yOffset < 0)
                        intensity *= 1 - Mathf.Abs(yOffset / (float)blurDistance);
                    if (yOffset > scaleFac)
                        intensity *= 1 - Mathf.Abs((yOffset - scaleFac) / (float)blurDistance);

                    naturalGasIntensity[xx, yy] = Mathf.Max(intensity, naturalGasIntensity[xx, yy]);
                }
            }
        }

        /**********************
         * Ice / Water
         * 
         */
        foreach (Vector2 tile in waterTiles) {
            //organicsIntensity[(int)(tile.x * scaleFactor), (int)(tile.y * scaleFactor)] = 1;

            int scaleFac = (int)scaleFactor;
            int tileX = (int)Mathf.Floor(tile.x * scaleFactor) - scaleFac / 2;
            int tileY = (int)Mathf.Floor(tile.y * scaleFactor) - scaleFac / 2;

            for (int xx = tileX - blurDistance; xx < tileX + scaleFac + blurDistance + 1; xx++) {
                for (int yy = tileY - blurDistance; yy < tileY + scaleFac + blurDistance + 1; yy++) {
                    float intensity = 1;

                    float xOffset = xx - tileX;
                    float yOffset = yy - tileY;


                    if (xOffset < 0)
                        intensity *= 1 - Mathf.Abs(xOffset / (float)blurDistance);
                    if (xOffset > scaleFac)
                        intensity *= 1 - Mathf.Abs((xOffset - scaleFac) / (float)blurDistance);

                    if (yOffset < 0)
                        intensity *= 1 - Mathf.Abs(yOffset / (float)blurDistance);
                    if (yOffset > scaleFac)
                        intensity *= 1 - Mathf.Abs((yOffset - scaleFac) / (float)blurDistance);

                    waterIntensity[xx, yy] = Mathf.Max(intensity, waterIntensity[xx, yy]);
                }
            }
        }

        //Create mask texture
        Texture2D organicsMask = new Texture2D(resolution, resolution);
        Texture2D naturalGasMask = new Texture2D(resolution, resolution);
        Texture2D waterMask = new Texture2D(resolution, resolution);

        for (int xx = 0; xx < resolution; xx += 1) {
            for (int yy = 0; yy < resolution; yy += 1) {

                float organics = organicsIntensity[xx, yy];
                Color organicsCol = new Color(organics, organics, organics);
                organicsMask.SetPixel(xx, yy, organicsCol);

                float water = waterIntensity[xx, yy];
                Color waterCol = new Color(water, water, water);
                waterMask.SetPixel(xx, yy, waterCol);

                float gas = naturalGasIntensity[xx, yy];
                Color gasCol = new Color(gas, gas, gas);  // IM GONNA STEP ON THE GAS
                naturalGasMask.SetPixel(xx, yy, gasCol);
            }
        }
        organicsMask.Apply();
        naturalGasMask.Apply();
        waterMask.Apply();
        

        renderer.sharedMaterial.SetTexture("OrganicResource_Mask", organicsMask);
        renderer.sharedMaterial.SetTexture("GasResource_Mask", naturalGasMask);
        renderer.sharedMaterial.SetTexture("WaterResource_Mask", waterMask);


        organicsMask.hideFlags = HideFlags.HideAndDontSave;
        naturalGasMask.hideFlags = HideFlags.HideAndDontSave;
        waterMask.hideFlags = HideFlags.HideAndDontSave;
    }
   

    public void RecalculateUVs() {
        FindMeshFilter();

        Mesh m = filter.sharedMesh;

        m.RecalculateBounds();
        float xScale = m.bounds.size.x;
        float yScale = m.bounds.size.z;

        Vector3[] verts = m.vertices;
        Vector2[] uvs = new Vector2[verts.Length];

        for (int i = 0; i < uvs.Length; i ++) {
            Vector3 pos = verts[i];
            uvs[i] = new Vector2(pos.x/xScale, pos.z/yScale);
        }
        m.uv = uvs;

        filter.sharedMesh = m;
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

    private void FindMeshRenderer() {
        if(renderer == null) {
            renderer = GetComponent<MeshRenderer>();
        }
    }


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
