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
    static float smoothness = 0.01f;

    static float noiseScale = 2f/100f;
    static float centreFlatRadius = 50;

    Texture2D heightTex;

    int values = 0;

    //public override void 

    public override void OnInspectorGUI() {

        float rightEdge = EditorGUIUtility.currentViewWidth - 35;

        TerrainGenerator script = (TerrainGenerator)target;

        float[,] heightmap = new float[0,0];

        if (GUILayout.Button("Generate Scripted Heightmap")) {
            RedoCurve();
            heightmap = script.GenerateHeightmap(curve, noiseScale, centreFlatRadius);
            script.SetHeightmap(heightmap);
            //script.GenerateTerrain(curve, noiseScale, centreFlatRadius);
            this.RedoHeightTexture(heightmap);
            /*tex = new Texture2D(2, 2);
            tex.name = "newtex";
            tex.hideFlags = HideFlags.HideAndDontSave;
            //tex.
            tex.SetPixel(0, 0, new Color(1,1,1,1));*/

            //tex.SetPixel(0, 0, Color.black);
            //tex.SetPixel(0, 1, Color.red);

            //tex.SetPixel(1, 1, Color.blue);

        }
        //if (heightmap.GetLength(0) > 0 )
        //RedoHeightTexture(heightmap);

        var curveField = EditorGUI.CurveField(new Rect(10,35,rightEdge,100), curve);
        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mat.hideFlags = HideFlags.HideAndDontSave;
        //script.hei
        //if (heightTex)
            //EditorGUI.DrawPreviewTexture(new Rect(10, 140, rightEdge, 100), heightTex, mat, ScaleMode.ScaleToFit, 0, -1, UnityEngine.Rendering.ColorWriteMask.All);
          //  EditorGUI.DrawPreviewTexture(new Rect(10, 140, rightEdge, 100), heightTex);




        //Texture2D texw = EditorGUIUtility.whiteTexture;
        //texw.SetPixel(1, 0, Color.green);

        /*if (tex) {
            Rect outline = new Rect(10, 140, rightEdge, 100);
            //EditorGUI.DrawPreviewTexture(outline, tex);
            GUI.DrawTexture(outline, tex);
        }*/


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
            Keyframe frame = new Keyframe((i + 1) * tSeperation - smoothVal, (i) * oDelta);
            Keyframe frameUpper = new Keyframe((i + 1) * tSeperation + smoothVal, (i + 1) * oDelta);

            curve.AddKey(frame);
            curve.AddKey(frameUpper);
        }

        curve.AddKey(1, 1);

        for (int i = 0; i < curve.keys.Length; i++) {
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
        }
       
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
