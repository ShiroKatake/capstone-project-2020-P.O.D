using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilemapTerrainGenerator))]
public class TilemapTerrainGeneratorEditor : Editor
{
    public Object rampMesh;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        TilemapTerrainGenerator script = (TilemapTerrainGenerator)target;

        if (GUILayout.Button("Generate Heightmap")) {

            script.ConvertImageToHeightmap(script.heightmapTexture);

        }

        if (GUILayout.Button("Generate Mesh from Image")) {
            script.ConvertImageToHeightmap(script.heightmapTexture);
            script.GenerateMesh();
            /*int resolution = 32;

            if (script.heightmap != null) {
                resolution = script.heightmap.width;
            }

            script.GenerateFlatMesh((resolution, resolution));*/
        }



        //rampMesh = EditorGUILayout.ObjectField(rampMesh, typeof(Mesh), true);
    }
    
}
