using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactGlow : MonoBehaviour
{
	[SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private Transform beamOrigin;

	private void Update()
	{
		transform.position = lineRenderer.GetPosition(1);
		transform.LookAt(beamOrigin);
	}
}
