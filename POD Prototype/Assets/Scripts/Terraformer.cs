using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraformer : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [Header("Terraformer Stats")]
    [SerializeField] private float terraformingSpeed;
    [SerializeField] private float health;

    [Header("Colours")]
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;

    //Non-Serialized Fields
    private bool terraforming = false;
    
    private Material terraformerMaterial;
    private Color activeColour;
    private Color inactiveColour;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Complex Public Properties

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

    private void Awake()
    {
        activeColour = activeMaterial.color;
        inactiveColour = inactiveMaterial.color;
        MeshRenderer meshRenderer = gameObject.GetComponent <MeshRenderer> () as MeshRenderer;
        meshRenderer.material = new Material(inactiveMaterial);
        terraformerMaterial = meshRenderer.material;
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

    private void Update()
    {
        CheckHealth();
        Terraform();
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            Planet.Instance.Terraformers.Remove(this);
            Destroy(this.gameObject);
        }
    }

    private void Terraform()
    {
        if (terraforming)
        {
            Planet.Instance.Terraform(terraformingSpeed * Time.deltaTime);
        }
    }
}
