using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridRendering : MonoBehaviour
{

    [SerializeField] private (int, int) gridSize = (20, 20);
    [SerializeField] public float scale = 0.5f;
    


    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            child.localScale = Vector3.one * 0.5f;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    


    

    private void OnDrawGizmos() {
        Gizmos.color = Color.black;

        float width = gridSize.Item1;
        float height = gridSize.Item2;

        for (int xx = 0; xx < gridSize.Item1+1; xx++) {

            Vector3 startH = new Vector3(-width / 2 + xx, 0.01f, -height / 2) * scale;
            Vector3 endH = new Vector3(-width / 2 + xx, 0.01f, height / 2) * scale;
            Gizmos.DrawLine(startH, endH);
            for (int yy = 0; yy < gridSize.Item2+1; yy++) {

                Vector3 startV = new Vector3(-width / 2, 0.01f, -height / 2 + yy) * scale;
                Vector3 endV = new Vector3(width / 2, 0.01f, -height / 2 + yy) * scale;

                Gizmos.DrawLine(startV, endV);
            }
        }
    }

}
