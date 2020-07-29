using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to get the world anchor position and convert it to on screen position for the hover dialogue box.
/// </summary>
public class GetBoundsToScreenSpace : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

	[SerializeField] private Renderer objectRenderer;

	//Non-Serialized Fields------------------------------------------------------------------------

	private Bounds bigBounds;

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		bigBounds = objectRenderer.bounds;
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Consider the bounds of an object to be a 3D box,
	/// whatever orientation it is currently on the screen, flat it out,
	/// then find the highest, and most right point (top right) and use it as the anchor.
	/// </summary>
	public Vector2 GetAnchorPosition()
	{
		Vector3[] screenSpaceCorners = new Vector3[8];

		screenSpaceCorners[0] = Camera.main.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		screenSpaceCorners[1] = Camera.main.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		screenSpaceCorners[2] = Camera.main.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		screenSpaceCorners[3] = Camera.main.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));

		screenSpaceCorners[4] = Camera.main.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		screenSpaceCorners[5] = Camera.main.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		screenSpaceCorners[6] = Camera.main.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		screenSpaceCorners[7] = Camera.main.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));

		float max_x = screenSpaceCorners[0].x;
		float max_y = screenSpaceCorners[0].y;

		for (int i = 0; i < 8; i++)
		{
			if (screenSpaceCorners[i].x > max_x)
			{
				max_x = screenSpaceCorners[i].x;
			}
			if (screenSpaceCorners[i].y > max_y)
			{
				max_y = screenSpaceCorners[i].y;
			}
		}

		return new Vector2(max_x, max_y);
	}
}
