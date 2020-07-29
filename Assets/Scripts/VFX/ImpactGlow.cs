using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactGlow : MonoBehaviour
{
	[SerializeField] private LineRenderer lineRenderer;
	private Vector3[] positions;

	private void Update()
	{
		lineRenderer.GetPositions(positions);
		transform.LookAt(lineRenderer.transform);
	}

	public void OnMining()
	{

	}
}
