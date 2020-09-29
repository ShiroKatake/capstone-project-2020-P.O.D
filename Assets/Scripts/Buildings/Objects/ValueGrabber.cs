using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueGrabber : MonoBehaviour
{
    //[SerializeField] Shader shader;
    private float dissolveAmount;

    public float DissolveAmount {get => dissolveAmount;}

    private void Awake() {
        dissolveAmount = gameObject.GetComponent<Material>().GetFloat("_DissolveAmount");
        //dissolveAmount = shader.GetPropertyDefaultFloatValue(11);
    }

    private void Update() {
        //dissolveAmount = gameObject.GetComponent<Material>().GetPropertyDefaultFloatValue(11);
        //dissolveAmount = shader.GetPropertyDefaultFloatValue(11);
    }
}
