using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mineral ores that will spawn from mineral nodes for the player.
/// </summary>
public class Ore : MonoBehaviour
{
	//Fields-----------------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

	[SerializeField] private int value = 5;
	[SerializeField] private float speed;

	//Non-Serialized Fields------------------------------------------------------------------------

	private float interpolationPercent;
	private Transform p0;
	private Vector3 p1;
	private Transform p2;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Complex Public Properties----------------------------------------------------------------------
	
	/// <summary>
	/// Where the ore spawns.
	/// </summary>
	public Transform Start { get => p0; set => p0 = value; }
	
	/// <summary>
	/// The control point for creating a linear Bezier curve between the Start point and the End point.
	/// </summary>
	public Vector3 Mid { get => p1; set => p1 = value; }
	
	/// <summary>
	/// Where the ore arrives.
	/// </summary>
	public Transform End { get => p2; set => p2 = value; }

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	private void OnEnable()
	{
		interpolationPercent = 0;
	}

	//Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	void Update()
    {
		interpolationPercent += Time.deltaTime;
		transform.position = CurveTowards(p0, p1, p2, interpolationPercent, speed);
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Interpolate between point 0 and 1, then point 1 and 2 to create Bezier curve path for the ore to home towards the player.
	/// </summary>
	/// <param name="t">Progress of the interpolation.</param name="t">
	private Vector3 CurveTowards(Transform p0, Vector3 p1, Transform p2, float t, float speed)
	{
		return Vector3.Lerp(Vector3.Lerp(p0.position, p1, t * speed), Vector3.Lerp(p1, p2.position, t * speed), t * speed);
	}

	//OnTrigger/Collision Triggered Methods--------------------------------------------------------

	/// <summary>
	/// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
	/// When the ore collides with the player, add value points to the mineral resource and return to pool.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			//Debug.Log("Collected " + value + " ores.");
			ResourceController.Instance.Ore += value;
			OreFactory.Instance.ReturnToPool(this);
		}
	}
}
