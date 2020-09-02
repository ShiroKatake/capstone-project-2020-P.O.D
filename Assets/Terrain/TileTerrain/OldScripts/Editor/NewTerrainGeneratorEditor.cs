using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NewTerrainGenerator))]
public class NewTerrainGeneratorEditor : Editor
{
    AnimationCurve blank = AnimationCurve.EaseInOut(0, 0, 1, 1);
    AnimationCurve curve = new AnimationCurve();

    Texture2D tex = default;

    static int layers = 3;
    static float smoothness = 0.00001f;

    static float noiseScale = 2f/100f;
    static float centreFlatRadius = 50;
    static float meshSmoothness = 0.2f;

    public override void OnInspectorGUI() {

        NewTerrainGenerator script = (NewTerrainGenerator)target;


        
        tex = (Texture2D)EditorGUILayout.ObjectField("Texture", tex, typeof(Texture2D), false);

        if (GUILayout.Button("Generate Texture Heightmap")) {

            script.SetHeightRes(tex.width);
            script.RegenerateHeightmap(tex);
            script.GenerateMesh(0);


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
