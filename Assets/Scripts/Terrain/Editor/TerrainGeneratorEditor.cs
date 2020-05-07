using System.Collections;
using System.Collections.Generic;
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

    int values = 0;

    public override void OnInspectorGUI() {

        TerrainGenerator script = (TerrainGenerator)target;

        var curveField = EditorGUI.CurveField(new Rect(10,0,EditorGUIUtility.currentViewWidth-35,100), curve);

        EditorGUILayout.Space(100);

        layers = EditorGUILayout.IntField("Layers", layers);
        smoothness = EditorGUILayout.FloatField("Smoothness: ", smoothness);
        noiseScale = EditorGUILayout.FloatField("Noise Scale: ", noiseScale);
        centreFlatRadius = EditorGUILayout.FloatField("Centre Flat: ", centreFlatRadius);


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Scripted Heightmap")) {

            RedoCurve();
            script.GenerateTerrain(curve, noiseScale, centreFlatRadius);

        }
        EditorGUILayout.EndHorizontal();

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
}
