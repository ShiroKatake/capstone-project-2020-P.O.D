using UnityEngine;

/// <summary>
/// A demo on how to control the script.
/// TL;DR:
/// - To change phase, you need to feed it an int value to terraformingOrbController.CurrentPhase.
/// - To end the FX in an exploooshion, just execute its Explode() method.
/// </summary>
public class TerraformingOrbDemo : MonoBehaviour
{
	//The fx's phase is changed by feeding it an int value. Using an enum might help with this
	private enum TestPhase
	{
		Zero,
		One,
		Two,
		Three,
		Four,
		Five,
		Six
	}
	
	[Header("Terraforming Orb")]
	[SerializeField] TerraformingOrbController terraformingOrbController;
	[SerializeField] private TestPhase setPhase = 0;
	[SerializeField] private bool beginSequenceDemo;

	// Explode() should only be called ONCE (if it happens in update, make sure to have a bool to prevent multiple calls)
	[SerializeField] private bool explode;
	private bool alreadyCalledExplode = false;

	private float timeElapsed;

	// Update is called once per frame
	void Update()
    {
		if (beginSequenceDemo)
		{
			if (terraformingOrbController.CurrentPhase < terraformingOrbController.PhaseCount - 2) // - 2 here because last phase is explode, and we wanna manually trigger that
			{
				timeElapsed += Time.deltaTime;
				if (timeElapsed > 5 && terraformingOrbController.IsScaleLerpingFinished)
				{
					timeElapsed = 0f;
					terraformingOrbController.CurrentPhase = terraformingOrbController.CurrentPhase + 1;
				}
			}
		}

		else
		{
			if (terraformingOrbController.CurrentPhase != (int)setPhase)
			{
				terraformingOrbController.CurrentPhase = (int)setPhase;
			}
		}

		if (explode)
		{
			if (!alreadyCalledExplode)
			{
				terraformingOrbController.Explode();
				alreadyCalledExplode = true;
			}
			else if (terraformingOrbController.IsShockwaveFinished)
			{
				alreadyCalledExplode = false;
				explode = false;
				setPhase = 0;
			}
		}
	}
}
