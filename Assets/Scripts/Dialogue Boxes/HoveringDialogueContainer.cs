using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoveringDialogueContainer : MonoBehaviour
{
	private HoveringDialogueText[] hoveringDialogueTexts;

	public bool IsAllEmpty {
		get {
			foreach (HoveringDialogueText text in hoveringDialogueTexts)
			{
				if (!string.IsNullOrEmpty(text.GetComponent<TextMeshProUGUI>().text))
				{
					return false;
				}
			}
			return true;
		}
	}

	private void Awake()
	{
		hoveringDialogueTexts = GetComponentsInChildren<HoveringDialogueText>();
	}
}
