using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Terraformers to terraform the planet.
/// </summary>
public class Terraformer : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Terraformer Stats")]
    [SerializeField] private float terraformingSpeed;

    [Header("Colours")]
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;

    //Non-Serialized Fields------------------------------------------------------------------------

    private bool terraforming = false;
    private Health health;
    
    private Material terraformerMaterial;
    private Color activeColour;
    private Color inactiveColour;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// Is the terraformer active and terraforming the planet?
    /// </summary>
    public bool Terraforming
    {
        get
        {
            return terraforming;
        }

        set
        {
            terraforming = value;
            terraformerMaterial.color = terraforming ? activeColour : inactiveColour;
        } 
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        activeColour = activeMaterial.color;
        inactiveColour = inactiveMaterial.color;
        MeshRenderer meshRenderer = gameObject.GetComponent <MeshRenderer> () as MeshRenderer;
        meshRenderer.material = new Material(inactiveMaterial);
        terraformerMaterial = meshRenderer.material;
        health = GetComponent<Health>();
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        CheckHealth();
        Terraform();
    }

    //Recurring Methods(Update())--------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if the terraformer is dead or not, and destroys it if it is.
    /// </summary>
    private void CheckHealth()
    {
        if (health.IsDead())
        {
            Planet.Instance.Terraformers.Remove(this);
            health.Die();
        }
    }

    /// <summary>
    /// Checks if the terraformer is active, and if so, terraforms the planet.
    /// </summary>
    private void Terraform()
    {
        if (terraforming)
        {
            Planet.Instance.Terraform(terraformingSpeed * Time.deltaTime);
        }
    }
}
