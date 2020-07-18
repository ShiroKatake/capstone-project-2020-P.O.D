using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoveringDialogueInitializer : MonoBehaviour
{
	[SerializeField] private LayoutGroup layoutGroupComponent;

	private void OnEnable()
	{
		StartCoroutine("UpdateLayoutGroup");
	}

	IEnumerator UpdateLayoutGroup()
	{
		layoutGroupComponent.enabled = false;
		yield return new WaitForEndOfFrame();
		layoutGroupComponent.enabled = true;
	}
}
