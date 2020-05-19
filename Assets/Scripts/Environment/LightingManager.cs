using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour {
	//References
	[SerializeField] private Light directionalLight;
	[SerializeField] private LightingPreset preset;
	//Variables

	// Update is called once per frame
	private void Update() {
		float cycleDuration = ClockController.Instance.CycleDuration;
		if (preset == null)
			return;
		UpdateLighting(ClockController.Instance.Time24hr / cycleDuration);
	}

	private void UpdateLighting(float timePercent) {
		RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
		RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

		if (directionalLight != null) {
			directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
			directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
		}
	}

	//Try to find a directional light to use if we haven't set one
	private void OnValidate() {
		if (directionalLight != null)
			return;
		//Search for directional light then set as sun
		if (RenderSettings.sun != null) {
			directionalLight = RenderSettings.sun;
		} 
		//Search scene for light that fits criteria (directional)
		else {
			Light[] lights = FindObjectsOfType<Light>();
			foreach (Light light in lights) {
				if (light.type == LightType.Directional) {
					directionalLight = light;
					return;
				}
			}
		}
	}
}
