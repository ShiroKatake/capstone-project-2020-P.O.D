using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows building to switch the fusion reactor's beam off.
/// </summary>
public class FusionReactorBeam : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private GameObject beam;

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Enables/disables the fusion reactor beam object.
    /// </summary>
    /// <param name="active">Should the beam be active or not?</param>
    public void SetBeamActive(bool active)
    {
        beam.SetActive(active);
    }
}
