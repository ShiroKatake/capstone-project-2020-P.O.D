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
	[SerializeField] private float cameraTransitionDuration = 3f;
	[SerializeField] private float stageSixPoints = 30f;
	[SerializeField] private float targetPhase;

	[SerializeField] private bool explode;
	[SerializeField] private bool isInDemo;

	private bool isExploding = false;
	private float timeElapsed;

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

		//else if (terraformingOrbController.CurrentPhase != Mathf.RoundToInt(6 * AlienManager.Instance.AlienKillProgress))
		//{
		//	terraformingOrbController.CurrentPhase = Mathf.RoundToInt(6 * AlienManager.Instance.AlienKillProgress);
		//}

		//else if (terraformingOrbController.CurrentPhase != Mathf.RoundToInt(currentPhase))
		//{
		//	terraformingOrbController.CurrentPhase = (int)currentPhase;
		//}
	}

	public void DisperseOrb()
	{
		explode = true;
	}

	public void StoreOrb()
	{
		StartCoroutine(Store());
	}

	public IEnumerator Store()
	{
        //Everything except the orb fx will be running (creates focus)
        PauseMenuManager.Instance.CanPause = false;
        TerraformingUI.Instance.CanDisplay = false;
        Time.timeScale = 0;
		float timeElapsed = 0f;

		//Lerp Camera to Tower's position
		while (timeElapsed < cameraTransitionDuration)
		{
			timeElapsed += Time.unscaledDeltaTime;
			float t = timeElapsed / cameraTransitionDuration;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			playerCamera.transform.position = Vector3.Lerp(playerCameraTransform.position, towerCameraTransform.position, t);
			yield return null;
		}

		//Store points
		RatioManager.Instance.StorePoints();

		//Trigger store fx, then pause for a bit
		targetPhase = Mathf.Round(RatioManager.Instance.PointsStored / stageSixPoints * 6);
		if (targetPhase > 6)
			targetPhase = 6;

		while (terraformingOrbController.CurrentPhase < targetPhase)
		{
			if (terraformingOrbController.IsScaleLerpingFinished)
			{
				terraformingOrbController.CurrentPhase = terraformingOrbController.CurrentPhase + 1;
			}
			yield return null;
		}
		yield return new WaitForSecondsRealtime(1f);

		//Then lerp the camera back to the player
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
        PauseMenuManager.Instance.CanPause = true;
        TerraformingUI.Instance.CanDisplay = true;
        yield return null;

	}

	private IEnumerator Explode()
	{
        //Everything except the orb fx will be running (creates focus)
        PauseMenuManager.Instance.CanPause = false;
        TerraformingUI.Instance.CanDisplay = false;
        Time.timeScale = 0;
		float timeElapsed = 0f;

		//Lerp Camera to Tower's position
		while (timeElapsed < cameraTransitionDuration)
		{
			timeElapsed += Time.unscaledDeltaTime;
			float t = timeElapsed / cameraTransitionDuration;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			playerCamera.transform.position = Vector3.Lerp(playerCameraTransform.position, towerCameraTransform.position, t);
			yield return null;
		}

		//Trigger store, then pause for a bit
		targetPhase = Mathf.Round((RatioManager.Instance.PointsStored + RatioManager.Instance.PointsGained) / stageSixPoints * 6);
		if (targetPhase > 6)
			targetPhase = 6;

		Debug.Log($"Target Phase: {targetPhase}");

		while (terraformingOrbController.CurrentPhase < targetPhase)
		{
			if (terraformingOrbController.IsScaleLerpingFinished)
			{
				terraformingOrbController.CurrentPhase = terraformingOrbController.CurrentPhase + 1;
				Debug.Log($"Target Phase: {terraformingOrbController.CurrentPhase}");
			}
			yield return null;
		}
		yield return new WaitForSecondsRealtime(1f);

		//Trigger explode, then pause for a bit
		terraformingOrbController.Explode();
		yield return new WaitForSecondsRealtime(1f);

		//Wait for the explosion effect to finish
		while (!terraformingOrbController.IsShockwaveFinished)
		{
			yield return null;
		}

		//Contribute points to progress bar
		RatioManager.Instance.DispersePoints();

		//Reset everything
		isExploding = false;
		explode = false;
		setPhase = 0;
		terraformingOrbController.CurrentPhase = 0;

		yield return new WaitForSecondsRealtime(0.5f);

		//Then lerp the camera back to the player
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
        PauseMenuManager.Instance.CanPause = true;
        TerraformingUI.Instance.CanDisplay = true;
        yield return null;
	}
}
