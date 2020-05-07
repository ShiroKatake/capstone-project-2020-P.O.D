using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Terrain GetTerrain() {
        return GetComponent<Terrain>();
    }

    public void GenerateTerrain(AnimationCurve curve, float noiseScale, float centreFlatRadius) {
        Terrain terrain = GetTerrain();
        TerrainData data = terrain.terrainData;

        int heightmapRes = data.heightmapResolution;

        float[,] heights = new float[heightmapRes, heightmapRes];

        Vector2 centre = new Vector2(heightmapRes / 2, heightmapRes / 2);

        for (int xx = 0; xx < heightmapRes; xx++) {
            for (int yy = 0; yy < heightmapRes; yy++) {
                Vector2 pos = new Vector2(xx, yy);
                float height;

                if ((centre - pos).magnitude < centreFlatRadius) {
                    height = curve.Evaluate(0.5f);
                }
                else {
                    height = Mathf.PerlinNoise((float)xx * noiseScale + 0.1f, (float)yy * noiseScale + 0.1f);
                    height = curve.Evaluate(height);
                }
                heights[xx, yy] = height;
            }
        }

        data.SetHeights(0, 0, heights);

    }

}
