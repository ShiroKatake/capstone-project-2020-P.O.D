using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    public Texture2D heightex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float[,] GenerateHeightmap(AnimationCurve curve, float noiseScale, float centreFlatRadius) {
        Terrain terrain = GetTerrain();
        TerrainData data = terrain.terrainData;

        int heightmapRes = data.heightmapResolution;
        Vector2 heightCentre = new Vector2(heightmapRes / 2, heightmapRes / 2);

        float[,] heights = new float[heightmapRes, heightmapRes];
        //heightex = new Texture2D(heightmapRes, heightmapRes);
        for (int xx = 0; xx < heightmapRes; xx++) {
            for (int yy = 0; yy < heightmapRes; yy++) {
                Vector2 pos = new Vector2(xx, yy);
                float height;



                if ((heightCentre - pos).magnitude < centreFlatRadius) {
                    height = curve.Evaluate(0.5f);
                }
                else if ((heightCentre - pos).magnitude < centreFlatRadius + 30) {
                    float delt = (heightCentre - pos).magnitude - centreFlatRadius;
                    float noiseVal = Mathf.PerlinNoise((float)xx * noiseScale + 0.1f, (float)yy * noiseScale + 0.1f);

                    height = Mathf.Max(curve.Evaluate(0.5f - delt/60f), curve.Evaluate(noiseVal));

                } else {
                    height = Mathf.PerlinNoise((float)xx * noiseScale + 0.1f, (float)yy * noiseScale + 0.1f);
                    height = curve.Evaluate(height);

                    //Random.Range(0,1);

                }
                heights[xx, yy] = height;
                //heightex.SetPixel(xx, yy, new Color(height, height, height));
            }
        }

        return heights;
    }

    public void SetHeightmap(float[,] heightmap) {
        Terrain terrain = GetTerrain();
        TerrainData data = terrain.terrainData;

        data.SetHeights(0, 0, heightmap);
    } 

    private Terrain GetTerrain() {
        return GetComponent<Terrain>();
    }


    public void GenerateTerrain(AnimationCurve curve, float noiseScale, float centreFlatRadius) {
        Terrain terrain = GetTerrain();
        TerrainData data = terrain.terrainData;

        int heightmapRes = data.heightmapResolution;

        float[,] heights = new float[heightmapRes, heightmapRes];

        List<int[,]> rockMaps = new List<int[,]>();
        for (int i = 0; i < 3; i ++) {
            rockMaps.Add(new int[data.detailWidth, data.detailHeight]);
        }

        Vector2 heightCentre = new Vector2(heightmapRes / 2, heightmapRes / 2);
        

        
        Random rand = new Random();

        for (int i = 0; i < 10; i ++) {
            Debug.Log(Mathf.RoundToInt(Random.value));
        }

        AnimationCurve rockCurve = new AnimationCurve();
        rockCurve.AddKey(0, 0);
        rockCurve.AddKey(0.97f, 0);
        rockCurve.AddKey(1, 1);

        Vector2 detailCenter = new Vector2(data.detailResolution / 2, data.detailResolution / 2);

        for (int xx = 0; xx < data.detailWidth; xx++) {
            for (int yy = 0; yy < data.detailHeight; yy++) {

                if ((detailCenter - new Vector2(xx, yy)).magnitude > centreFlatRadius/2) {
                    int val = Mathf.RoundToInt(rockCurve.Evaluate(Random.value));
                    rockMaps[Mathf.RoundToInt(Random.Range(0, 2))][xx, yy] = val;
                }
                
            }
        }

        data.SetHeights(0, 0, heights);
        for (int i = 0; i < 3; i++) {
            data.SetDetailLayer(0, 0, i, rockMaps[i]);
            //rockMaps.Add(new int[data.detailWidth, data.detailHeight]);
        }
        
    }

}
