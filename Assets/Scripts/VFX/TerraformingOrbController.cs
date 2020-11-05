using UnityEngine;

/// <summary>
/// Controller script for terraforming orb FX.
/// </summary>
public class TerraformingOrbController : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------
	
	#region Serialized Fields
	#region Main Components
	[Header("Terraforming Orb's Main Components")]
	[Tooltip("The Spark Particle System.")]
	[SerializeField] private ParticleSystem spark;

	[Space]
	[Tooltip("The Pulsating Orb Particle System.")]
	[SerializeField] private ParticleSystem pulsatingOrb;

	[Tooltip("The size of this orb when at climax.")]
	[SerializeField] private float pulsatingOrbEndSize = 1.8f;

	[Tooltip("How long it takes for the orb to grow from one phase to another.")]
	[SerializeField] private float scaleChangeDuration = 1f;

	[Tooltip("How long it takes for the orb to shrink from current phase to 0.")]
	[SerializeField] private float shrinkDuration = 0.5f;

	[Tooltip("How long the orb should wait before shrinking.")]
	[SerializeField] private float shrinkDelay = 0.6f;
	
	[Space]
	[Tooltip("The Accumulating Lines Particle System.")]
	[SerializeField] private ParticleSystem lines;

	[Space]
	[Tooltip("The Outer Sphere FX.")]
	[SerializeField] private GameObject outerSphere;

	[Tooltip("How long it takes for the sphere to fade in.")]
	[SerializeField] private float alphaChangeDuration = 10f;

	[Space]
	[Tooltip("The Wind Blades Particle System.")]
	[SerializeField] private ParticleSystem windBlades;

	[Space]
	[Tooltip("The Pulses Particle System.")]
	[SerializeField] private ParticleSystem pulses;
	#endregion

	#region Explosion Components
	[Header("Terraforming Orb's Explosion Components")]
	
	[Tooltip("The Shockwave FX.")]
	[SerializeField] private GameObject shockwave;
	[SerializeField] private float shockwaveDelay = 0.8f;
	[SerializeField] private float showaveDuration = 5f;

	[Space]
	[Tooltip("The Expanding Wind Blades Particle System.")]
	[SerializeField] private ParticleSystem expandingWindBlades;
	#endregion
	#endregion
	
	//Non-Serialized Fields------------------------------------------------------------------------
	
	#region Private Fields
	private bool explode;
	private int currentPhase = 0;
	private GameObject[] phaseEffects;
	private bool[,] fxControl;

	//Pulsating Orb variables
	private bool isScaleLerping;
	private bool isScaleLerpingFinished = true;
	private float scaleTimeElapsed = 0f;
	private float shrinkDelayElapsed;
	private float orbGrowFactor;

	//Outer Sphere variables
	private Material outerSphereMaterial;
	private bool isAlphaLerping;
	private bool isAlphaLerpingFinished = true;
	private float alphaTimeElapsed = 0f;
	private float rotationTimeElapsed = 0f;
	private Color outerSphereStartColor;
	private Color outerSphereEndColor;

	//Shockwave variables
	private Material shockwaveMaterial;
	private bool isShockwaveLerping;
	private bool isShockwaveFinished = true;
	private float shockwaveTimeElapsed = 0f;
	private float shockwaveDelayElapsed;
	#endregion
	
	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Basic Public Properties----------------------------------------------------------------------
	
	#region Public Properties
	public int PhaseCount
	{
		get
		{
			return fxControl.GetLength(1);
		}
	}

	public int CurrentPhase
	{
		get
		{
			return currentPhase;
		}

		set
		{
			currentPhase = value;
			shockwave.GetComponent<ShockwaveFX>().ShockwaveLevel = Mathf.CeilToInt(currentPhase / 2f);
			DoPhase(currentPhase);
		}
	}
	
	public bool IsScaleLerpingFinished { get => isScaleLerpingFinished; }
	public bool IsShockwaveFinished { get => isShockwaveFinished; }
	#endregion

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		//Initialize the outer sphere
		outerSphereMaterial = outerSphere.GetComponent<Renderer>().material;
		outerSphereEndColor = outerSphereMaterial.GetColor("_MainColor");
		outerSphereStartColor = outerSphereEndColor;
		outerSphereStartColor.a = 0;
		outerSphereMaterial.SetColor("_MainColor", outerSphereStartColor);

		shockwaveMaterial = shockwave.GetComponent<Renderer>().material;

		//Set grow factor
		orbGrowFactor = 0.36f;

		phaseEffects = new GameObject[8] { spark.gameObject, pulsatingOrb.gameObject, lines.gameObject, outerSphere, windBlades.gameObject, pulses.gameObject, expandingWindBlades.gameObject, shockwave };
		fxControl = new bool[8, 8]
		{ //Phase:  0      1      2      3      4      5      6   EXPLODE
				{ false,  true,  true,  true,  true,  true, false, false },	//Spark
				{ false,  true,  true,  true,  true,  true,  true,  true },	//PulsatingOrb
				{ false, false, false,  true,  true,  true, false, false },	//Lines
				{ false, false, false, false,  true,  true,  true,  true },	//OuterSphere
				{ false, false, false, false, false,  true,  true, false },	//WindBlades
				{ false, false, false, false, false, false,  true, false },	//Pulses
				{ false, false, false, false, false, false, false,  true },	//ExpandingWindBlades
				{ false, false, false, false, false, false, false, false }	//Shockwave
		};

		//Set current phase to 0
		CurrentPhase = 0;
	}

	//Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	private void Update()
	{
		#region OuterSphere Rotation
		if (rotationTimeElapsed < 1f)
		{
			rotationTimeElapsed += Time.unscaledDeltaTime;
			outerSphereMaterial.SetFloat("Time", rotationTimeElapsed);
		}
		else
		{
			outerSphereMaterial.SetFloat("Time", 1f);
			rotationTimeElapsed = 0f;
		}
		#endregion

		#region OuterSphere Alpha Lerping
		if (isAlphaLerping)
		{
			isAlphaLerpingFinished = LerpOuterSphereAlpha(outerSphereStartColor, outerSphereEndColor, alphaChangeDuration);
		}
		#endregion

		#region Pulsating Orb Scale Lerping
		//If the FX is exploding, shrink the orb
		if (explode)
		{
			//Delay the shrinkage
			if (shrinkDelayElapsed < shrinkDelay)
			{
				shrinkDelayElapsed += Time.unscaledDeltaTime;
			}
			else
			{
				isScaleLerpingFinished = LerpPulsatingOrbSize(orbGrowFactor * (currentPhase - 1), 0, shrinkDuration);
			}
		}

		else if (isScaleLerping && currentPhase != 0)
		{
			isScaleLerpingFinished = LerpPulsatingOrbSize(orbGrowFactor * (currentPhase - 1), orbGrowFactor * currentPhase, scaleChangeDuration);
		}
		#endregion

		#region Shockwave Texture Size Lerping
		if (isShockwaveLerping)
		{
			//Delay the shockwave
			isShockwaveFinished = false;
			if (shockwaveDelayElapsed < shockwaveDelay)
			{
				shockwaveDelayElapsed += Time.unscaledDeltaTime;
			}
			else
			{
				//Manually enable the shockwave (this should be enabled later since there's a delay)
				if (!shockwave.activeSelf)
				{
					shockwaveMaterial.SetFloat("_FXScale", -45f);
					shockwave.SetActive(true);
				}

				isShockwaveFinished = LerpShockWave(-45f, 0f, showaveDuration);
				if (isShockwaveFinished)
				{
					//Once this is done, reset entire FX
					explode = false;
					CurrentPhase = 0;
				}
			}
		}
		#endregion
	}

	//Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Lerping the orb's size over time.
	/// </summary>
	/// <param name="startSize">The size the orb starts with.</param>
	/// <param name="endSize">The size the orb should be after lerping.</param>
	/// <param name="scaleDuration">How long the lerp should takes.</param>
	private bool LerpPulsatingOrbSize(float startSize, float endSize, float scaleDuration)
	{
		if (scaleTimeElapsed < scaleDuration)
		{
			scaleTimeElapsed += Time.unscaledDeltaTime;
			float t = scaleTimeElapsed / scaleDuration;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			pulsatingOrb.transform.localScale = Vector3.Lerp(new Vector3(startSize, startSize, startSize), new Vector3(endSize, endSize, endSize), t);
		}

		else
		{
			shrinkDelayElapsed = 0f;
			scaleTimeElapsed = 0f;
			isScaleLerping = false;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Lerping the sphere's fade in over time.
	/// </summary>
	/// <param name="startValue">The alpha value the sphere starts with.</param>
	/// <param name="endValue">The alpha value the sphere should be after lerping.</param>
	/// <param name="alphaDuration">How long the lerp should takes.</param>
	private bool LerpOuterSphereAlpha(Color startValue, Color endValue, float alphaDuration)
	{
		if (alphaTimeElapsed < alphaDuration)
		{
			alphaTimeElapsed += Time.unscaledDeltaTime;
			outerSphereMaterial.SetColor("_MainColor", Color.Lerp(startValue, endValue, alphaTimeElapsed / alphaDuration));
		}
		else
		{
			alphaTimeElapsed = 0f;
			isAlphaLerping = false;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Lerping the shockwave's size over time.
	/// </summary>
	/// <param name="startSize">The size the shockwave starts with.</param>
	/// <param name="endSize">The size the shockwave should be after lerping.</param>
	/// <param name="shockwaveDuration">How long the lerp should takes.</param>
	private bool LerpShockWave(float startSize, float endSize, float shockwaveDuration)
	{
		if (shockwaveTimeElapsed < shockwaveDuration)
		{
			float t = shockwaveTimeElapsed / shockwaveDuration;
			t = 1 - Mathf.Pow(2, -10 * t);
			shockwaveTimeElapsed += Time.unscaledDeltaTime;
			shockwaveMaterial.SetFloat("_FXScale", Mathf.Lerp(startSize, endSize, t));
		}
		else
		{
			shockwaveTimeElapsed = 0f;
			shockwaveDelayElapsed = 0f;
			isShockwaveLerping = false;
			return true;
		}

		return false;
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Enables/Disables FXs based on the phase number.
	/// </summary>
	/// <param name="phaseNumber">The phase number to change to.</param>
	private void ChangeFXtoPhase(int phaseNumber)
	{
		for (int i = 0; i < fxControl.GetLength(0); i++)
		{
			ParticleSystem partSys = phaseEffects[i].GetComponent<ParticleSystem>();
			if (partSys != null)
			{
				if (partSys.isPlaying != fxControl[i, phaseNumber])
				{
					if (fxControl[i, phaseNumber])
					{
						phaseEffects[i].SetActive(true);
						partSys.Play();
					}
					else
					{
						partSys.Stop();
					}
				}
			}
			else
			{
				if (phaseEffects[i].activeSelf != fxControl[i, phaseNumber])
				{
					phaseEffects[i].SetActive(fxControl[i, phaseNumber]);
				}
			}
		}
	}

	/// <summary>
	/// Update the FX according to a specified phase.
	/// </summary>
	/// <param name="phaseNumber">The phase number to change to.</param>
	public void DoPhase(int phaseNumber)
	{
		ChangeFXtoPhase(phaseNumber);

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

	/// <summary>
	/// EXPLOOOOOOSHION.
	/// </summary>
	public void Explode()
	{
		explode = true;
		isShockwaveLerping = true;
		ChangeFXtoPhase(7);
	}
}
