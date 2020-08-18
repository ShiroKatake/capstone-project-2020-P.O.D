using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for FX when buildings finished constructing.
/// </summary>
public class FinishedFXFactory : Factory<FinishedFXFactory, FinishedFX, ENone>
{
    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves a FinishedFX from the pool if there's any available, instantiates a new one if none are available, then sets its position.
    /// </summary>
    /// <returns>A new instance of FinishedFX.</returns>
    public FinishedFX Get()
    {
        return base.Get(ENone.None);
    }

    /// <summary>
    /// Creates a new FinishedFX.
    /// </summary>
    /// <returns>A building of the specified type.</returns>
    protected override FinishedFX Create(ENone type)
    {
        FinishedFX fx = base.Create(type);
        fx.gameObject.SetActive(false);
        return fx;
    }

    /// <summary>
    /// Handles the destruction of FinishedFXs.
    /// </summary>
    /// <param name="fx">The FinishedFX to be destroyed.</param>
    public void Destroy(FinishedFX fx)
    {
        Destroy(ENone.None, fx);
    }

    /// <summary>
    /// Handles the destruction of FinishedFXs.
    /// </summary>
    /// <param name="type">The type of the FinishedFX to be destroyed.</param>
    /// <param name="fx">The FinishedFX to be destroyed.</param>
    public override void Destroy(ENone type, FinishedFX fx)
    {
        fx.gameObject.SetActive(false);
        base.Destroy(type, fx);
    }
}
