using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedFX : MonoBehaviour
{
	private ParticleSystem partSys;
	private void Awake()
	{
		partSys = GetComponent<ParticleSystem>();
	}
	public void Update()
	{
		if (partSys.isStopped)
		{
			FinishedFXFactory.Instance.Destroy(this);
		}
	}
}
