using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraformingOrbDemo : MonoBehaviour
{
	private enum TestPhase
	{
		Zero,
		One,
		Two,
		Three,
		Four,
		Five
	}
	
	[Header("Terraforming Orb")]
	[SerializeField] TerraformingOrbController terraformingOrbController;
	[SerializeField] private TestPhase setPhase = 0;
	[SerializeField] private bool beginSequenceDemo;

	private float timeElapsed;

    // Update is called once per frame
    void Update()
    {
		if (beginSequenceDemo)
		{
			timeElapsed += Time.deltaTime;
			if (timeElapsed > 5 && terraformingOrbController.IsScaleLerpingFinished && terraformingOrbController.CurrentPhase < 5)
			{
				timeElapsed = 0f;
				terraformingOrbController.CurrentPhase = terraformingOrbController.CurrentPhase + 1;
			}
		}

		else
		{
			if (terraformingOrbController.CurrentPhase != (int)setPhase)
			{
				terraformingOrbController.CurrentPhase = (int)setPhase;
			}
		}
	}
}
