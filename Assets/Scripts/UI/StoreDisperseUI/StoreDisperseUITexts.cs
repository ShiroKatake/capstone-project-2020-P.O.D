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

	private RectTransform rectTransform;

	private void Awake()
	{		
		rectTransform = GetComponent<RectTransform>();
	}

	private void OnEnable()
	{
		header.text = $"Wave {AlienManager.Instance.CurrentWave} complete!";
		pointsStored.text = $"Points stored: {RatioManager.Instance.PointsStored}";
		pointsGained.text = $"Points gained: <color=#75FF00>{RatioManager.Instance.PointsGained}</color>";
		disperseBonus.text = $"Disperse bonus: <color=#FFB500>+{RatioManager.Instance.DisperseBonus}%</color>";
		wavesRemaining.text = $"Waves remaining: {AlienManager.Instance.WavesRemaining}";

		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
	}
}
