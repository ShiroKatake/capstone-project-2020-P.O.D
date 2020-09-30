using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A demo to show how the color flashing works. I'm imagining this to be in the game manager, or a script that has FunctionsThatHandleDamageDetection().
/// </summary>
public class ColorFlashDemo : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  
	private MaterialFlashColor materialFlashColor;

	//Serialized Fields----------------------------------------------------------------------------
	[SerializeField] private Color healFlash;
	[SerializeField] private Color damageFlash;
	
	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		materialFlashColor = GetComponent<MaterialFlashColor>();
	}

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
		{
			materialFlashColor.SetFlashColor(healFlash);
			Debug.Log("Heal!");
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			materialFlashColor.SetFlashColor(damageFlash);
			Debug.Log("Damage!");
		}
	}
}
