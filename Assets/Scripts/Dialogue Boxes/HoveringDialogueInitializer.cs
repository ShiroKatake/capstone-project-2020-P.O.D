using UnityEngine;

/// <summary>
/// Keeps the dialogue box's size consistent.
/// </summary>
public class HoveringDialogueInitializer : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

	[SerializeField] private float padding = 10f;
	[SerializeField] private RectTransform contentRectTransform;

	//Non-Serialized Fields------------------------------------------------------------------------

	private RectTransform rectTransform;

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	//Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	void Update()
	{
		rectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x + padding * 2, contentRectTransform.sizeDelta.y + padding * 2);
	}
}
