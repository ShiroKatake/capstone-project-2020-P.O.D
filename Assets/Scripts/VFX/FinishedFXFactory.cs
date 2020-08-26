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
    /// Retrieves a FinishedFX from the pool if there's any available, or instantiates a new one if none are available.
    /// </summary>
    /// <param name="type">The type of FinishedFX to instantiate. Should be left as default value of ENone.None.</param>
    /// <returns>A new instance of FinishedFX.</returns>
    public override FinishedFX Get(ENone type = ENone.None)
    {
        return base.Get(type);
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

    ///// <summary>
    ///// Handles the destruction of FinishedFXs.
    ///// </summary>
    ///// <param name="fx">The FinishedFX to be destroyed.</param>
    //public void Destroy(FinishedFX fx)
    //{
    //    Destroy(ENone.None, fx);
    //}

    /// <summary>
    /// Handles the destruction of FinishedFXs.
    /// </summary>
    /// <param name="fx">The FinishedFX to be destroyed.</param>
    /// <param name="type">The type of the FinishedFX to be destroyed. Should be left as default value of ENone.None.</param>
    public override void Destroy(FinishedFX fx, ENone type = ENone.None)
    {
        fx.gameObject.SetActive(false);
        base.Destroy(fx, type);
    }
}
