using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGreeningFade : MonoBehaviour
{

    //EnvironmentManager envControl;
    Material terrainMat;

    // Start is called before the first frame update
    void Start()
    {
        //envControl = EnvironmentManager.Instance;
        terrainMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //float prop = envControl.ProgressProportion;

        //terrainMat.SetVector("FadeProportion", new Vector2(prop, 0));
        //terrainMat.SetFloat("FadeProportion", prop);
        
    }
}
