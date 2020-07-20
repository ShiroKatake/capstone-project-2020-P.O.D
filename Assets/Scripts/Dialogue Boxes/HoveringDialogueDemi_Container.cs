using UnityEngine;
using TMPro;

/// <summary>
/// A demi script placed in objects with multiple children that have editable text fields (this is neccessary since we need to check if all children are empty before disabling this object).
/// </summary>
public class HoveringDialogueDemi_Container : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------
	
	//Non-Serialized Fields------------------------------------------------------------------------

	private HoveringDialogueDemi_TextElement[] hoveringDialogueTexts;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Complex Public Properties--------------------------------------------------------------------
	
	/// <summary>
	/// Returns true if all editable text fields in its children are empty.
	/// </summary>
	public bool IsAllEmpty {
		get {
			foreach (HoveringDialogueDemi_TextElement text in hoveringDialogueTexts)
			{
				if (!string.IsNullOrEmpty(text.GetComponent<TextMeshProUGUI>().text))
				{
					return false;
				}
			}
			return true;
		}
	}

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		hoveringDialogueTexts = GetComponentsInChildren<HoveringDialogueDemi_TextElement>();
	}
}
