using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreDisperseUI : MonoBehaviour
{
	[SerializeField] GameObject mainPanel;
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
	}
}
