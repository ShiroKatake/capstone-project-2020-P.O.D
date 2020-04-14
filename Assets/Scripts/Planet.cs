using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A demo class for the planet the player is terraforming.
/// </summary>
public class Planet : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Materials")]
    [SerializeField] private Material grassMaterial;
    [SerializeField] private Material dirtMaterial;

    [Header("The Cryo Egg")]
    [SerializeField] private Building cryoEgg;

    //Non-Serialized Fields------------------------------------------------------------------------
    
    //Colours
    private Color grassColour;
    private Color dirtColour;
    private Material planetMaterial;

    //Terraforming
    private List<Terraformer> terraformers;
    private float terraformingProgress;
    private bool disabledTerraformers = false;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// Planet's singleton public property.
    /// </summary>
    public static Planet Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The cryo egg.
    /// </summary>
    public Building CryoEgg { get => cryoEgg; }

    /// <summary>
    /// The list of terraformers terraforming the planet.
    /// </summary>
    public List<Terraformer> Terraformers { get => terraformers; }

    /// <summary>
    /// How far the planet's terraforming has progressed.
    /// </summary>
    public float TerraformingProgress { get => terraformingProgress; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
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

    ///// <summary>
    ///// Called by terraformers to progress how much the planet has been terraformed.
    ///// </summary>
    ///// <param name="additionalProgress"></param>
    //public void Terraform(float additionalProgress)
    //{
    //    if (terraformingProgress < 1)
    //    {
    //        terraformingProgress = Mathf.Min(1, terraformingProgress + additionalProgress);
    //        planetMaterial.color = Color.Lerp(dirtColour, grassColour, terraformingProgress);
    //    }
    //    else if (!disabledTerraformers)
    //    {
    //        disabledTerraformers = true;

    //        foreach (Terraformer t in terraformers)
    //        {
    //            t.Terraforming = false;
    //        }
    //    }
    //}
}
