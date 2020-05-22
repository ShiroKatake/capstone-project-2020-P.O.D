using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Determines the colour for the ambient colour, directional light, and fog throughout the day.
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "LightingPreset", menuName = "Scriptables/Lighting Preset", order = 1)]
public class LightingPreset : ScriptableObject
{
	public Gradient AmbientColor;
	public Gradient DirectionalColor;
	public Gradient FogColor;
}
