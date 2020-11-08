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

	[SerializeField] private bool isInDemo;

	private float timeElapsed;

	void Update()
	{
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
        StartCoroutine(Store(true));
    }

	public void StoreOrb()
	{
		StartCoroutine(Store(false));
	}	

    private IEnumerator Store(bool explode)
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

        //Trigger store, then pause for a bit
        targetPhase = Mathf.Ceil((RatioManager.Instance.PointsStored) / stageSixPoints * 6);
        if (targetPhase > 6) targetPhase = 6;
        float lerpStartTime = Time.unscaledTime;
        Debug.Log($"TerraformingOrbDemo.Store(explode=={explode}), starting orb scale lerping, targetPhase: {targetPhase}, lerpStartTime: {lerpStartTime}");

        while (terraformingOrbController.CurrentPhase < targetPhase && Time.unscaledTime - lerpStartTime < 5)
        {
            Debug.Log($"TerraformingOrbDemo.Store(explode=={explode}), terraformingOrbController.IsScaleLerpingFinished: {terraformingOrbController.IsScaleLerpingFinished}");

            if (terraformingOrbController.IsScaleLerpingFinished)
            {
                terraformingOrbController.CurrentPhase++;
                Debug.Log($"TerraformingOrbDemo.Store(explode=={explode}), finished orb scale lerping for current phase, terraformingOrbController. CurrentPhase is now {terraformingOrbController.CurrentPhase}");
            }

            yield return null;
        }

        if (Time.unscaledTime - lerpStartTime >= 5) Debug.LogError($"TerraformingOrbDemo.Store(explode=={explode}), orb lerping timed out after 5 seconds. lerpStartTime: {lerpStartTime}, Time.unscaledTime: {Time.unscaledTime}, targetPhase: {targetPhase}, terraformingOrbController.CurrentPhase: {terraformingOrbController.CurrentPhase}, terraformingOrbController.IsScaleLerpingFinished: {terraformingOrbController.IsScaleLerpingFinished}");

        yield return new WaitForSecondsRealtime(1f);

        if (explode)
        {
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
		    setPhase = 0;
		    terraformingOrbController.CurrentPhase = 0;

		    yield return new WaitForSecondsRealtime(0.5f);
        }

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

        EStage currentStage = StageManager.Instance.CurrentStage.GetID();

        if (currentStage != EStage.Win && currentStage != EStage.Lose)
        {
            Time.timeScale = 1;
            PauseMenuManager.Instance.CanPause = true;
            TerraformingUI.Instance.CanDisplay = true;
        }

        yield return null;
	}
}
