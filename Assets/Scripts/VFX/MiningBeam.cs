using UnityEngine;

public class MiningBeam : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	//Serialized Fields----------------------------------------------------------------------------                                                    

	[SerializeField] private Transform beamOrigin;
	[SerializeField] private Transform impactGlowFX;

	//Non-Serialized Fields------------------------------------------------------------------------                                                    

	private LineRenderer lineRenderer;

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Enables the beam, spark fx and set their positions.
	/// </summary>
	/// <param name="endPoint">The position where the beam will stop and where the spark fx will be.</param>
	public void OnMineEnable(Vector3 endPoint)
	{
		lineRenderer.enabled = true;
		impactGlowFX.gameObject.SetActive(true);

		lineRenderer.SetPosition(0, beamOrigin.position);
		lineRenderer.SetPosition(1, endPoint);

		impactGlowFX.transform.position = endPoint;
		impactGlowFX.transform.LookAt(beamOrigin);
	}

	/// <summary>
	/// Disables the line renderer of the beam and the fx but keep the script running to re-enable these in the future.
	/// </summary>
	public void OnMineDisable()
	{
		if (lineRenderer.enabled)
			lineRenderer.enabled = false;

		if (impactGlowFX.gameObject.activeInHierarchy)
			impactGlowFX.gameObject.SetActive(false);
	}
}
