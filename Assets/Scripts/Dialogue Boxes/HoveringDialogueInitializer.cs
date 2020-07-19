using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoveringDialogueInitializer : MonoBehaviour
{
	[SerializeField] private float padding = 10f;
	[SerializeField] private RectTransform contentRectTransform;
	private RectTransform rectTransform;
	
	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		rectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x + padding * 2, contentRectTransform.sizeDelta.y + padding * 2);
	}
}
