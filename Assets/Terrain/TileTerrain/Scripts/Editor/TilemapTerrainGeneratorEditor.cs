using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilemapTerrainGenerator))]
public class TilemapTerrainGeneratorEditor : Editor
{
    TilemapTerrainGenerator script;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        script = (TilemapTerrainGenerator)target;

        if (GUILayout.Button("Generate Heightmap")) {

            script.ConvertImageToHeightmap(script.heightmapTexture);

        }

        if (GUILayout.Button("Generate Mesh from Image")) {
            script.ConvertImageToHeightmap(script.heightmapTexture);
            script.GenerateMesh();
        }

        if (GUILayout.Button("Generate Cliffs from Image")) {
            ClearChildren();

            script.CreateCliffsFromImage(script.heightmapTexture);
            
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Optimise Mesh"))
            script.OptimiseMesh();
        if (GUILayout.Button("Set Collision Mesh"))
            script.SetCollisionMesh();

        GUILayout.Space(10);

        if (GUILayout.Button("Clear Children")) {
            ClearChildren();
        }


        //rampMesh = EditorGUILayout.ObjectField(rampMesh, typeof(Mesh), true);
    }
    
    private void ClearChildren() {
        Transform obj = script.transform;
        int childNum = obj.childCount;

        for (int i = obj.childCount - 1; i >= 0; i--) {
            Transform child = obj.GetChild(i);
            DestroyImmediate(child.gameObject, false);
        }
    }

}
