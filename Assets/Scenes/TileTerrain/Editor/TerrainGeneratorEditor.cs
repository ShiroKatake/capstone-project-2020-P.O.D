using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    AnimationCurve blank = AnimationCurve.EaseInOut(0, 0, 1, 1);
    AnimationCurve curve = new AnimationCurve();

    static int layers = 3;
    static float smoothness = 0.05f;

    static float noiseScale = 6f/1000f;
    static float centreFlatRadius = 50;

    Texture2D heightTex;

    int values = 0;

    //public override void 

    public override void OnInspectorGUI() {

        float rightEdge = EditorGUIUtility.currentViewWidth - 35;

        TerrainGenerator script = (TerrainGenerator)target;

        float[,] heightmap = new float[0,0];
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Scripted Heightmap")) {
            RedoCurve();
            heightmap = script.GenerateHeightmap(curve, noiseScale, centreFlatRadius);
            script.SetHeightmap(heightmap);
            //script.GenerateTerrain(curve, noiseScale, centreFlatRadius);
            this.RedoHeightTexture(heightmap);


        }
        if (GUILayout.Button("Generate Manual Heightmap")) {
            //RedoCurve();
            heightmap = script.GenerateHeightmap(curve, noiseScale, centreFlatRadius);
            script.SetHeightmap(heightmap);
            //script.GenerateTerrain(curve, noiseScale, centreFlatRadius);
            this.RedoHeightTexture(heightmap);


        }
        GUILayout.EndHorizontal();


        curve = EditorGUI.CurveField(new Rect(10,35,rightEdge,100), curve);
        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mat.hideFlags = HideFlags.HideAndDontSave;
        

        EditorGUILayout.Space(220);

        layers = EditorGUILayout.IntField("Layers", layers);
        smoothness = EditorGUILayout.FloatField("Smoothness: ", smoothness);
        noiseScale = EditorGUILayout.FloatField("Noise Scale: ", noiseScale);
        centreFlatRadius = EditorGUILayout.FloatField("Centre Flat: ", centreFlatRadius);

        if (GUILayout.Button("Redo curve")) {
            RedoCurve();
        }
        
        
    }

    private void RedoCurve() {
        curve = new AnimationCurve();

        float tSeperation = 1 / (float)layers;
        float oDelta = 1 / ((float)layers - 1);

        float smoothVal = tSeperation * smoothness;

        curve.AddKey(0, 0);

        for (int i = 0; i < layers - 1; i++) {

            Vector2 leadInV = new Vector2((i + 1) * tSeperation - smoothVal * 4, (i) * oDelta);
            Vector2 lowerSmoothV = new Vector2((i + 1) * tSeperation - smoothVal, (i) * oDelta + smoothVal * 4);
            float inTangent = (lowerSmoothV.y - leadInV.y) / (lowerSmoothV.x - leadInV.x);
            Vector2 upperSmoothV = new Vector2((i + 1) * tSeperation + smoothVal, (i + 1) * oDelta - smoothVal * 4);
            float rampTangent = (lowerSmoothV.y - upperSmoothV.y) / (lowerSmoothV.x - upperSmoothV.x);
            Vector2 leadOutV = new Vector2((i + 1) * tSeperation + smoothVal * 4, (i + 1) * oDelta);
            float outTangent = (upperSmoothV.y - leadOutV.y) / (upperSmoothV.x - leadOutV.x);

            Keyframe leadInF = new Keyframe(leadInV.x, leadInV.y, 0, inTangent/2);
            Keyframe lowerSmoothF = new Keyframe(lowerSmoothV.x, lowerSmoothV.y, inTangent * 2, rampTangent);
            Keyframe upperSmoothF = new Keyframe(upperSmoothV.x, upperSmoothV.y, rampTangent, outTangent * 2);
            Keyframe leadOutF = new Keyframe(leadOutV.x, leadOutV.y, outTangent/2, 0);

            curve.AddKey(leadInF);
            curve.AddKey(lowerSmoothF);
            curve.AddKey(upperSmoothF);
            curve.AddKey(leadOutF);

            /*Keyframe frame = new Keyframe((i + 1) * tSeperation - smoothVal * 4, (i) * oDelta, 0, 0);
            Keyframe f = new Keyframe((i + 1) * tSeperation - smoothVal * 2, (i) * oDelta + smoothVal);
            Keyframe frameUpper = new Keyframe((i + 1) * tSeperation + smoothVal * 2, (i + 1) * oDelta);
            */
            /*curve.AddKey(frame);
            curve.AddKey(f);
            curve.AddKey(frameUpper);*/
        }
        curve.AddKey(1, 1);
        AnimationUtility.SetKeyRightTangentMode(curve, curve.keys.Length - 2, AnimationUtility.TangentMode.Linear);
        

        /*for (int i = 0; i < curve.keys.Length; i++) {
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Free);
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Free);
        }*/
       
    }

    private void RedoHeightTexture(float[,] heightmap) {

        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);

        //if (heightTex != default(Texture2D))
        //    heightTex.hideFlags = HideFlags.HideAndDontSave;

        //heightTex = Texture2D.whiteTexture;// = new Texture2D(width, height);
        heightTex = new Texture2D(width, height);
        heightTex.hideFlags = HideFlags.HideAndDontSave;
        //heightTex.Resize(10, 10);
        //heightTex = new Texture2D(width, height);
        //heightTex.hideFlags = HideFlags.None;

        for (int xx = 0; xx < width; xx++) {
            for (int yy = 0; yy < height; yy++) {
                float val = heightmap[xx, yy];
                heightTex.SetPixel(xx, yy, new Color(val, val, val));
                
                //heightTex.SetPixel(xx, yy, Color.blue);
            }
        }

        //heightTex.EncodeToPNG();

        File.WriteAllBytes("IMAGE.png", heightTex.EncodeToPNG());
    }
}
