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
    /// Disables the fusion reactor beam object.
    /// </summary>
    public void Deactivate()
    {
        beam.SetActive(false);
    }
}
