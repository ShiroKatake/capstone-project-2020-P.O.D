using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTextManager : MonoBehaviour
{
    //think about using custom setters and getters here...
    /*
    int x {
        get {return x;}
        set {
            //custom setter code
        }
    }
    */
    [SerializeField] private Text metal;
    [SerializeField] private Text water;
    [SerializeField] private Text energyUsed;
    [SerializeField] private Text energyMax;

    public static ResourceTextManager Instance {get; protected set;}

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("There should never be 2 or more Resource Text Managers in the scene.");
        }

        Instance = this;
    }

    public void SetMetalText(string newText){
        metal.text = newText;
    }

    public void SetWaterText(string newText){
        water.text = newText;
    }

    public void SetEnergyUsedText(string newText){
        energyUsed.text = newText;
    }

    public void SetEnergyMaxText(string newText){
        energyMax.text = newText;
    }
}
