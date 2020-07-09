using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class for Day & Night cycle. 
/// </summary>
public class LightingManager : MonoBehaviour {
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------
	[SerializeField] private Light directionalLight;
	[SerializeField] private LightingPreset preset;

	//Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	private void Update()
	{
		float cycleDuration = ClockController.Instance.CycleDuration;
		if (preset == null)
			return;
		UpdateLighting(ClockController.Instance.Time24hr / cycleDuration);
	}

	//Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Change the colours in the lighting preset according to the time of day
	/// </summary>
	private void UpdateLighting(float timePercent)
	{
		RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
		RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

		if (directionalLight != null)
		{
			directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
			directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f), 245f, 0));
		}
	}

	//Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// OnValidate() is run every time the script is reloaded or something is changed in the inspector.
	/// Check for a directional light and set as the sun to use. If there isn't any, find the first available directional light, 
	/// </summary>
	private void OnValidate()
	{
		if (directionalLight != null)
			return;
		if (RenderSettings.sun != null)
		{
			directionalLight = RenderSettings.sun;
		}
		else
		{
			Light[] lights = FindObjectsOfType<Light>();
			foreach (Light light in lights)
			{
				if (light.type == LightType.Directional)
				{
					directionalLight = light;
					return;
				}
			}
		}
	}
}
