using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreDisperseUI : MonoBehaviour
{
	[SerializeField] GameObject mainPanel;
	[SerializeField] Button storeButton;

    // Start is called before the first frame update
    void Start()
    {
		RatioManager.Instance.updateStoreDisperse += DisplayUI;
		mainPanel.SetActive(false);
    }

	public void Store()
	{
		RatioManager.Instance.StorePoints();
		DisplayUI(false);
	}

	public void Disperse()
	{
		RatioManager.Instance.DispersePoints();
		DisplayUI(false);
	}

	private void DisplayUI(bool state)
	{
		mainPanel.SetActive(state);
		storeButton.interactable = AlienManager.Instance.WavesRemaining == 0;
	}
}
