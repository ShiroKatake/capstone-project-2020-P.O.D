using UnityEngine;
using kTools.Decals;
using System.Collections.Generic;

/// <summary>
/// A Factory class for TurretRangeFXs.
/// </summary>
public class TurretRangeFXFactory : Factory<TurretRangeFXFactory, TurretRangeFX, ENone>
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	//Non-Serialized Fields------------------------------------------------------------------------

	private const float BASE_SPRITE_RADIUS = 2.1f;
	private const float DECAL_HEIGHT = 4.5f;

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves a TurretRangeFX from the pool if there's any available, or instantiates a new one if none are available.
    /// </summary>
    /// <returns>A new instance of TurretRangeFX.</returns>
    public TurretRangeFX Get()
    {
        return base.Get(ENone.None);
    }

    /// <summary>
    /// Handles the destruction of TurretRangeFXs.
    /// </summary>
    /// <param name="fx">The TurretRangeFX to be destroyed.</param>
    public void Destroy(TurretRangeFX fx)
    {
        Destroy(ENone.None, fx);
    }

	/// <summary>
	/// If the player is placing a turret, display the turret range.
	/// </summary
	public void OnGetTurret(Transform turretTransform, TurretRangeFX currentFX)
	{
		float radius = turretTransform.GetComponent<TurretShooting>().DetectionRadius;
		currentFX.transform.position = new Vector3(0f, DECAL_HEIGHT, 0f);
		currentFX.transform.localScale = turretTransform.localScale * radius * BASE_SPRITE_RADIUS;
		currentFX.transform.SetParent(turretTransform, false);
	}
}
