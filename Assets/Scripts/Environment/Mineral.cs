using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Nodes of minerals for the player to collect for building.
/// </summary>
public class Mineral : MonoBehaviour
{
	//Fields-----------------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private bool placed;
	[SerializeField] private float oreSpawnRate = 1f;
	[SerializeField] private float oreCurveRadius = 2;
	[SerializeField] private Transform miningPoint;

	//Non-Serialized Fields------------------------------------------------------------------------

	private int oreCount;
	private int initialCount;
    private int id;
    private List<Collider> colliders;
    private List<MeshRenderer> renderers;
    private bool despawning;
	private float timer = 0f;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Basic Public Properties----------------------------------------------------------------------

	/// <summary>
	/// Whether the mineral node is in the process of despawning or not.
	/// </summary>
	public bool Despawning { get => despawning; }

    /// <summary>
    /// How much ore remains in this mineral node.
    /// </summary>
    public int OreCount { get => oreCount; }

	public Vector3 MiningPoint { get => miningPoint.position; }
    //Complex Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The Mineral node's unique ID number. Id should only be set by MineralFactory.GetMineral().
    /// </summary>
    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
            gameObject.name = $"Mineral {id}";
        }
    }

    /// <summary>
    /// Whether or not the mineral has been placed in the scene, or is pooled in the object pool.
    /// </summary>
    public bool Placed
    {
        get
        {
            return placed;
        }

        set
        {
            placed = value;

            if (placed)
            {
                MapController.Instance.RegisterMineral(this);
            }
            else
            {
                MapController.Instance.DeRegisterMineral(this);
            }
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        renderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
		initialCount = oreCount;
        timer = oreSpawnRate;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
	{
		oreCount = MineralFactory.Instance.OreCount;

		if (placed)
        {
            MapController.Instance.RegisterMineral(this);
        }
    }

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Allows the player to mine a mineral node for minerals.
	/// </summary>
	/// <returns>The number of minerals that the player has mined during this frame.</returns>
	public void Mine()
    {
		timer -= Time.deltaTime;

		if (timer <= 0f)
		{
			ReleaseOre();
			oreCount--;

			if (oreCount <= 0)
			{
				MineralFactory.Instance.Destroy(this);
			}

			timer = oreSpawnRate;
		}
	}

	/// <summary>
	/// Spawns ores and choose a path for them to home towards the player.
	/// </summary>
	private void ReleaseOre()
	{
		Ore ore = OreFactory.Instance.Get();
		ore.transform.position = transform.position;
		ore.transform.rotation = transform.rotation;
		ore.Start = transform;
		ore.Mid = GetPointOnUnitSphereCap(Quaternion.LookRotation(Vector3.up), 90f) * oreCurveRadius + transform.position; //Choose a random point in a hemisphere, facing up, with a radius of 1, as a "curving point".
		ore.End = FindObjectOfType<PlayerID>().transform;
		ore.gameObject.SetActive(true);
	}

    /// <summary>
    /// Enables/disables the mineral node's colliders.
    /// </summary>
    /// <param name="enabled">Whether the colliders will be enabled or disabled.</param>
    public void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider c in colliders)
        {
            c.enabled = enabled;
        }
    }

    /// <summary>
    /// Enables/disables the mineral node's mesh renderers.
    /// </summary>
    /// <param name="enabled">Whether the mesh renderers will be enabled or disabled.</param>
    public void SetMeshRenderersEnabled(bool enabled)
    {
        foreach (MeshRenderer r in renderers)
        {
            r.enabled = enabled;
        }
    }

    /// <summary>
    /// Resets the mineral node to its default state.
    /// </summary>
    public void Reset()
    {
        StartCoroutine(DespawnMineral());
	}

    /// <summary>
    /// Handles the visual dissapation of the mineral
    /// </summary>
    /// <returns></returns>
    private IEnumerator DespawnMineral()
    {
        despawning = true;

        while (transform.position.y > -1.1)
        {
            transform.position += new Vector3(0, -1 * Time.deltaTime, 0);//TODO: swap for de-spawn shader/animation
            yield return null;
		}

		SetCollidersEnabled(false);
		SetMeshRenderersEnabled(false);
		oreCount = initialCount;
		transform.position = ObjectPool.Instance.transform.position;
        transform.parent = ObjectPool.Instance.transform;
        despawning = false;
    }

	//Utility Methods--------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// This will generate a random 3D point within a sphere with an angle modifier (hemisphere, quarter of a sphere, etc) 
	/// to create varied paths for the ores homing towards the player (keep things interesting).
	/// </summary>
	/// <returns>A random 3D point that will be the control point for the ore's Bezier curve.</returns>
	private Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
	{
		float angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
		Vector2 PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
		Vector3 v = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
		return targetDirection * v;
	}

	/// <summary>
	/// Draws a sphere to help visuallising the radius of the GetPointOnUnitSphereCap() function (can't visuallize the angle aspect unfortunately).
	/// </summary>
	void OnDrawGizmosSelected()
	{
		// Draw a yellow sphere at the transform's position
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, oreCurveRadius);
	}
}
