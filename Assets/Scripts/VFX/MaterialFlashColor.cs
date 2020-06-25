using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Colour flashing when taking damage/getting healed.
/// </summary>
public class MaterialFlashColor : MonoBehaviour
{
	//Private Fields--------------------------------------------------------------------------------------------------------------------------------- 
	private Material material;
	private Color materialFlashColor;
	private Animator animator;
	
	//Serialized Fields----------------------------------------------------------------------------
	[SerializeField] private float flashSpeed;
	[SerializeField] private Renderer objectRenderer;
	
	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	/// <summary>
	/// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
	/// Start() runs after Awake().
	/// </summary>
	private void Start()
    {
		materialFlashColor = new Color(1, 0, 0, 0);
		material = objectRenderer.material;
    }

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	private void Update()
    {
        if (materialFlashColor.a > 0)
		{
			Debug.Log("change");
			materialFlashColor.a = Mathf.Clamp01(materialFlashColor.a - flashSpeed * Time.deltaTime);
			material.SetColor("_Flash", materialFlashColor);
		}
    }

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------
	
	//Start the flashing (Requires colour input, contemplating on setting colours in here instead of via input)
	public void SetFlashColor(Color color)
	{
		animator.enabled = false;
		objectRenderer.material = material;
		materialFlashColor = color;
		material.SetColor("_Flash", materialFlashColor);
		Debug.Log("Material set");
	}
}
