using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoveringDialogueManager : MonoBehaviour
{
	[SerializeField] private GameObject hoverDialogue;
	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = hoverDialogue.GetComponent<RectTransform>();
		HideDialogue();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			ShowDialogue();
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			HideDialogue();
		}
	}

	public void ShowDialogue()
	{
		hoverDialogue.SetActive(true);
	}

	public void HideDialogue()
	{
		hoverDialogue.SetActive(false);
	}
}
