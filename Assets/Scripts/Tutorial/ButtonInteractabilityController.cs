using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractabilityController : MonoBehaviour
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Interactability Requirements - Resources")]
    [SerializeField] private bool requiresResources;
    [SerializeField] private int requiredOre;
    [SerializeField] private int requiredPower;
    [SerializeField] private int requiredWater;
    [SerializeField] private int requiredWaste;

    //Non-Serialized Fields------------------------------------------------------------------------

    private bool active;
    private Button button;

    //Public Properties------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Is this button currently active;
    /// </summary>
    public bool Active { get => active; set => active = value; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        button.interactable = CheckInteractable();
    }

    /// <summary>
    /// Checks if the button should be interactable.
    /// </summary>
    /// <returns>Whether the button should be interactable.</returns>
    private bool CheckInteractable()
    {
        return active && (!requiresResources ||
                (
                       requiredOre <= ResourceController.Instance.Ore
                    && requiredPower <= ResourceController.Instance.SurplusPower
                    && requiredWaste <= ResourceController.Instance.SurplusWaste
                    && requiredWater <= ResourceController.Instance.SurplusWater
                )
            );
    }
}
