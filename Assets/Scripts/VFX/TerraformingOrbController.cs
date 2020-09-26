using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraformingOrbController : MonoBehaviour
{
	[Header("Terraforming Orb's Components")]
	[SerializeField] private ParticleSystem spark;

	[Space]
	[SerializeField] private ParticleSystem pulsatingOrb;
	[SerializeField] private float pulsatingOrbEndSize = 1.8f;

	[Space]
	[SerializeField] private ParticleSystem lines;

	[Space]
	[SerializeField] private GameObject outerSphere;
	[SerializeField] private Color outerSphereStartColor;
	[SerializeField] private Color outerSphereEndColor;

	[Space]
	[SerializeField] private ParticleSystem windBlades;

	private int currentPhase = 0;
	private float timeElapsedForScale = 0f;
	private float timeElapsedForAlpha = 0f;
	private bool isScaleLerping;
	private bool isAlphaLerping;
	private GameObject[] fxArray;
	private float orbGrowFactor;
	private Material outerSphereMaterial;
	private bool isAlphaLerpingFinished = true;
	private bool isScaleLerpingFinished = true;

	public int CurrentPhase
	{
		get
		{
			return currentPhase;
		}

		set
		{
			currentPhase = value;
			DoPhase(currentPhase);
		}
	}

	public bool IsAlphaLerpingFinished { get => isAlphaLerpingFinished; }
	public bool IsScaleLerpingFinished { get => isScaleLerpingFinished; }

	private void Awake()
	{
		outerSphereMaterial = outerSphere.GetComponent<Renderer>().material;
		fxArray = new GameObject[5] { spark.gameObject, spark.gameObject, lines.gameObject, outerSphere, windBlades.gameObject };
		orbGrowFactor = pulsatingOrbEndSize / 5;
		CurrentPhase = 0;
	}

	// Update is called once per frame
	private void Update()
	{
		if (isAlphaLerping)
		{
			isAlphaLerpingFinished = LerpOuterSphereAlpha(outerSphereStartColor, outerSphereEndColor, 10f);
		}

		if (isScaleLerping)
		{
			isScaleLerpingFinished = LerpPulsatingOrbSize(orbGrowFactor * (currentPhase - 1), orbGrowFactor * currentPhase, 1f);
		}
	}

	private bool LerpOuterSphereAlpha(Color startValue, Color endValue, float alphaDuration)
	{
		if (timeElapsedForAlpha < alphaDuration)
		{
			timeElapsedForAlpha += Time.deltaTime;
			outerSphereMaterial.SetColor("_MainColor", Color.Lerp(startValue, endValue, timeElapsedForAlpha / alphaDuration));
		}
		else
		{
			timeElapsedForAlpha = 0f;
			isAlphaLerping = false;
			return true;
		}

		return false;
	}

	private bool LerpPulsatingOrbSize(float startSize, float endSize, float scaleDuration)
	{
		if (timeElapsedForScale < scaleDuration)
		{
			timeElapsedForScale += Time.deltaTime;
			float t = timeElapsedForScale / scaleDuration;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			pulsatingOrb.transform.localScale = Vector3.Lerp(new Vector3(startSize, startSize, startSize), new Vector3(endSize, endSize, endSize), t);
		}

		else
		{
			timeElapsedForScale = 0f;
			isScaleLerping = false;
			return true;
		}

		return false;
	}

	private void DoPhase(int phaseNumber)
	{

		for (int i = fxArray.Length; i --> 0;)
		{
			if (i < phaseNumber && !fxArray[i].activeSelf)
			{
				fxArray[i].SetActive(true);
			}

			else if (i >= phaseNumber && fxArray[i].activeSelf)
			{
				fxArray[i].SetActive(false);
			}
		}

		if (phaseNumber > 0)
		{
			if (!pulsatingOrb.gameObject.activeSelf)
			{
				pulsatingOrb.gameObject.SetActive(true);
			}

			if (phaseNumber == 4)
			{
				isAlphaLerping = true;
			}

			else if (phaseNumber < 4)
			{
				outerSphereMaterial.SetColor("_MainColor", outerSphereStartColor);
			}

			isScaleLerping = true;
		}

		else
		{
			if (pulsatingOrb.gameObject.activeSelf)
			{
				pulsatingOrb.gameObject.SetActive(false);
			}
		}
	}
}
