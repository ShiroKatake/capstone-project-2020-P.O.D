using UnityEngine;

/// <summary>
/// A controller class for TurretRangeFXs.
/// </summary>
public class TurretRangeFXController : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	[SerializeField] private TurretRangeFX turretRangeFX;
    [SerializeField] private bool fxEnabled;

	//Non-Serialized Fields------------------------------------------------------------------------

	private const float BASE_SPRITE_RADIUS = 2.1f;
	private const float FX_HEIGHT = 4.5f;

	private void Start()
    {
        Debug.Log($"TurretRangeFXFactory.Start(), turret range FXs are{(fxEnabled ? " " : " not ")}enabled.");
        BuildingFactory.Instance.onBuildingHasRange += EnableFX;
		BuildingFactory.Instance.onPlacementFinished += DisableFX;
		DisableFX();
	}

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// If the player is placing a turret, display the turret range.
    /// </summary>
    private void EnableFX(Building building)
    {
        if (fxEnabled)
        {
            float radius = building.GetComponent<TurretShooting>().DetectionRadius;
            turretRangeFX.transform.localScale = building.transform.localScale * radius * BASE_SPRITE_RADIUS;
            turretRangeFX.transform.SetParent(building.transform, false);
            turretRangeFX.transform.localPosition = new Vector3(0f, FX_HEIGHT, 0f);

            turretRangeFX.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides TurretRangeFXs.
    /// </summary>
    public void DisableFX()
	{
		turretRangeFX.transform.SetParent(null);
		turretRangeFX.gameObject.SetActive(false);
	}
}