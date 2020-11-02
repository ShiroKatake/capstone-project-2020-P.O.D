using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreDisperseUITexts : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI header;
	[SerializeField] TextMeshProUGUI pointsStored;
	[SerializeField] TextMeshProUGUI pointsGained;
	[SerializeField] TextMeshProUGUI disperseBonus;
	[SerializeField] TextMeshProUGUI wavesRemaining;

	private List<RectTransform> rectsToRefresh = new List<RectTransform>();
	private RectTransform rectTransform;

	private void Awake()
	{
		HoveringDialogueDemi_RefreshElement[] refreshElements = GetComponentsInChildren<HoveringDialogueDemi_RefreshElement>();
		foreach (HoveringDialogueDemi_RefreshElement refreshElement in refreshElements)
		{
			rectsToRefresh.Add(refreshElement.GetComponent<RectTransform>());
		}
		rectsToRefresh.Reverse();
		rectTransform = GetComponent<RectTransform>();
	}

	private void OnEnable()
	{
		header.text = $"Wave {AlienManager.Instance.CurrentWave} complete!";
		pointsStored.text = $"Points stored: {RatioManager.Instance.PointsStored}";
		pointsGained.text = $"Points gained: <color=#75FF00>{RatioManager.Instance.PointsGained}</color>";
		disperseBonus.text = $"Disperse bonus: <color=#FFB500>+{RatioManager.Instance.DisperseBonus}%</color>";
		wavesRemaining.text = $"Waves remaining: {AlienManager.Instance.WavesRemaining}";

		StartCoroutine("Enable");
	}

	private void OnDisable()
	{		
		Time.timeScale = 1;
	}

	private void Rebuild()
	{
		foreach (var rect in rectsToRefresh)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
		}
		//Refresh the biggest container last
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
	}

	private IEnumerator Enable()
	{
		Rebuild();
		yield return new WaitForEndOfFrame();
		Time.timeScale = 0;
	}
}
