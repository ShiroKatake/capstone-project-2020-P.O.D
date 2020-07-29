using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningBeam : MonoBehaviour
{
	[SerializeField] private Transform beamOrigin;
	[SerializeField] private Transform beamEnd;
	private LineRenderer lineRenderer;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	void Start()
    {
		lineRenderer.SetPosition(0, beamOrigin.position);

	}

    // Update is called once per frame
    void Update()
	{
		lineRenderer.SetPosition(1, beamEnd.position);
	}
}
