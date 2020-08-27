using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceZoneGenerator : MonoBehaviour
{

    private void Awake() {
        Material terrainMat = GetComponent<Renderer>().material;

        Texture2D mask = new Texture2D(1024, 1024);

        ClearTexture(mask, Color.black);
        //mask.SetPixels(0, 0, 1024, 1024, new Color[] { Color.black });

        GenerateCircle(mask, new Vector2(300, 300), 100, 0);

        terrainMat.SetTexture("IceCap_Mask", mask);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ClearTexture(Texture2D texture, Color col) {
        for (int xx = 0; xx < texture.width; xx++) {
            for (int yy = 0; yy < texture.height; yy++) {
                texture.SetPixel(xx, yy, col);
            }
        }
    }

    private void GenerateCircle(Texture2D image, Vector2 position, float scale, float fuzz) {

        for (int xx = 0; xx < image.width; xx ++) {
            for (int yy = 0; yy < image.height; yy++) {

                Vector2 thisPos = new Vector2(xx, yy);
                if ((thisPos - position).magnitude < scale) {
                    image.SetPixel(xx, yy, Color.white);
                }
            }
        }

        image.Apply();
    }
}
