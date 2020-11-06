using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreDisperseUI : PublicInstanceSerializableSingleton<StoreDisperseUI>
{
	[SerializeField] GameObject mainPanel;
	[SerializeField] GameObject greyPanel;
	[SerializeField] ButtonInteract storeButton;
	[SerializeField] TerraformingOrbDemo terraformingOrb;

    private bool canShowMenu;

    public bool CanShowMenu { get => canShowMenu; set => canShowMenu = value; }

    public bool IsVisible { get => mainPanel.activeSelf; }

    // Start is called before the first frame update
    void Start()
    {
		RatioManager.Instance.updateStoreDisperse += ToggleUI;
		mainPanel.SetActive(false);
    }

	public void Store()
	{
		RatioManager.Instance.StorePoints();
		ToggleUI(false);
		terraformingOrb.StoreOrb();
	}

	public void Disperse()
	{
		RatioManager.Instance.DispersePoints();
		ToggleUI(false);
		terraformingOrb.DisperseOrb();
	}

	private void ToggleUI(bool state)
	{
        if (canShowMenu)
        {
            greyPanel.SetActive(state);
            mainPanel.SetActive(state);
            storeButton.OnInteractableChanged(storeButton.GetComponent<Button>().interactable = AlienManager.Instance.WavesRemaining != 0);
        }
	}
}
