using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RatioManager))]
public class RatioEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		RatioManager ratioManager = (RatioManager)target;

		GUILayout.BeginHorizontal();

			if (GUILayout.Button("Get Target Ratio"))
			{
				ratioManager.UpdateTargetRatio();
			}

			if (GUILayout.Button("Get Current Ratio"))
			{
				ratioManager.UpdateCurrentRatio();
			}

		GUILayout.EndHorizontal();
	}
}
