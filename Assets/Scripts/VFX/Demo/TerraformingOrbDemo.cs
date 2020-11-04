using UnityEngine;
using Unity.Collections;
using System.Collections;

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
	[SerializeField] Camera playerCamera;
	[SerializeField] Transform towerCameraTransform;
	[SerializeField] Transform playerCameraTransform;
	[SerializeField] private TestPhase setPhase = 0;
	[SerializeField] private bool beginSequenceDemo;
	[SerializeField] float cameraTransitionDuration = 3f;

	// Explode() should only be called ONCE (if it happens in update, make sure to have a bool to prevent multiple calls)
	[SerializeField] private bool explode;
	[SerializeField] private bool isInDemo;

	private bool isExploding = false;
	private float timeElapsed;

	public bool IxExploding { get => isExploding; }

	// Update is called once per frame
	void Update()
	{
		if (explode)
		{
			if (!isExploding)
			{
				StartCoroutine(Explode());
				isExploding = true;
			}
		}

		if (isInDemo)
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

			else if (terraformingOrbController.CurrentPhase != (int)setPhase)
			{
				terraformingOrbController.CurrentPhase = (int)setPhase;
			}
		}

		else if (terraformingOrbController.CurrentPhase != Mathf.RoundToInt(6 * AlienManager.Instance.AlienKillProgress))
		{
			terraformingOrbController.CurrentPhase = Mathf.RoundToInt(6 * AlienManager.Instance.AlienKillProgress);
		}
	}

	public void TriggerExplode()
	{
		explode = true;
	}

	private IEnumerator Explode()
	{
		Time.timeScale = 0;
		float timeElapsed = 0f;
		while (timeElapsed < cameraTransitionDuration)
		{
			timeElapsed += Time.unscaledDeltaTime;
			float t = timeElapsed / cameraTransitionDuration;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			playerCamera.transform.position = Vector3.Lerp(playerCameraTransform.position, towerCameraTransform.position, t);
			yield return null;
		}
		Debug.Log("explode");
		terraformingOrbController.Explode();
		yield return new WaitForSecondsRealtime(1f);

		while (!terraformingOrbController.IsShockwaveFinished)
		{
			yield return null;
		}

		isExploding = false;
		explode = false;
		setPhase = 0;

		yield return new WaitForSecondsRealtime(0.5f);

		timeElapsed = 0f;
		while (timeElapsed < cameraTransitionDuration)
		{
			timeElapsed += Time.unscaledDeltaTime;
			float t = timeElapsed / cameraTransitionDuration;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			playerCamera.transform.position = Vector3.Lerp(towerCameraTransform.position, playerCameraTransform.position, t);
			yield return null;
		}
		Time.timeScale = 1;
		yield return null;
	}
}
