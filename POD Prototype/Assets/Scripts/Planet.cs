using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [Header("Materials")]
    [SerializeField] private Material grassMaterial;
    [SerializeField] private Material dirtMaterial;

    //Non-Serialized Fields
    
    //Colours
    private Color grassColour;
    private Color dirtColour;
    private Material planetMaterial;

    //Terraforming
    private List<Terraformer> terraformers;
    private float terraformingProgress;
    private bool disabledTerraformers = false;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property

    public static Planet Instance { get; protected set; }

    //Basic Public Properties

    public List<Terraformer> Terraformers { get => terraformers; }
    public float TerraformingProgress { get => terraformingProgress; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more Planets.");
        }

        Instance = this;

        grassColour = grassMaterial.color;
        dirtColour = dirtMaterial.color;
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>() as MeshRenderer;
        meshRenderer.material = new Material(dirtMaterial);
        planetMaterial = meshRenderer.material;

        terraformers = new List<Terraformer>();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    public void Terraform(float additionalProgress)
    {
        if (terraformingProgress < 1)
        {
            terraformingProgress = Mathf.Min(1, terraformingProgress + additionalProgress);
            planetMaterial.color = Color.Lerp(dirtColour, grassColour, terraformingProgress);
        }
        else if (!disabledTerraformers)
        {
            disabledTerraformers = true;

            foreach (Terraformer t in terraformers)
            {
                t.Terraforming = false;
            }
        }
    }
}
