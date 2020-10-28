using UnityEngine;

/// <summary>
/// A controller class for the placement grid.
/// </summary>
public class PlacementGridFXManager : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

	[Tooltip("Reference to the Building Area object.")]
	[SerializeField] private GameObject buildingArea;

	[Tooltip("Reference to the Placement Grid object.")]
	[SerializeField] private GameObject placementGrid;

	[Tooltip("The color the building area should be when the placement is valid.")]
	[SerializeField] private Color validColor;

	[Tooltip("The color the building area should be when the placement is invalid.")]
	[SerializeField] private Color invalidColor;

	[Tooltip("The maximum length or width a building can currently achieve.")]
	[SerializeField] private float maxPossibleSize;

	//Non-Serialized Fields------------------------------------------------------------------------
	
	private const float GRID_BASE_SIZE = 0.1f;
	private const float GRID_PADDING = 0.001f;
	private const float FX_HEIGHT = 0f - 0.5f + 0.015f;
	
	private float offsetX;
	private float offsetY;
	private float lastSizeX;
	private float lastSizeY;
	private Material placementGridMaterial;
	private SpriteRenderer buildingAreaRenderer;

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	void Awake()
	{
		placementGridMaterial = placementGrid.GetComponent<SpriteRenderer>().material;
		buildingAreaRenderer = buildingArea.GetComponent<SpriteRenderer>();
		maxPossibleSize++;
	}

	/// <summary>
	/// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
	/// Start() runs after Awake().
	/// </summary>
	void Start()
	{
		BuildingFactory.Instance.onPlacementStarted += OnPlacementStarted;
		BuildingFactory.Instance.onPlacementFinished += OnPlacementFinished;
		BuildingFactory.Instance.onPlacementInvalid += OnPlacementInvalid;
		BuildingFactory.Instance.onPlacementValid += OnPlacementValid;

		OnPlacementFinished();
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Initializes the grid and building area relative to the size of the building being held.
	/// </summary>
	/// <param name="x">The size of the building's length.</param>
	/// <param name="y">The size of the building's width.</param>
	private void InitializeGrid(float x, float y)
	{
		//Length
		if (MathUtility.Instance.IsOdd(x))
		{
			offsetX = 0.5f;
		}
		else if (MathUtility.Instance.IsEven(y))
		{
			offsetX = 0f;
		}

		//Width
		if (MathUtility.Instance.IsOdd(y))
		{
			offsetY = 0.5f;
		}
		else if (MathUtility.Instance.IsEven(y))
		{
			offsetY = 0f;
		}

		float largestSide = x >= y ? x : y;
		string vignetteSize = $"1.{maxPossibleSize - largestSide}";
		//Debug.Log(vignetteSize);
		placementGridMaterial.SetVector("_GridOffset", new Vector4(offsetX, offsetY, 0f, 0f));
		placementGridMaterial.SetFloat("_VignetteSize", float.Parse(vignetteSize));
		buildingArea.transform.localScale = new Vector3(GRID_BASE_SIZE * x + GRID_PADDING * (x - 1), GRID_BASE_SIZE * y + GRID_PADDING * (y - 1), 1);
	}

	/// <summary>
	/// A UnityEvent that triggers when OnPlacement() is called.
	/// Compares the building's current size to the last building that was held.
	/// If the size is different then update the grid and building area accordingly.
	/// </summary>
	/// <param name="building">The building currently being held.</param>
	private void OnPlacementStarted(Building building)
	{
		if (lastSizeX != building.BuildingFoundationOffsetsOnXAxis + 2 || lastSizeY != building.BuildingFoundationOffsetsOnZAxis + 2)
		{
			lastSizeX = building.BuildingFoundationOffsetsOnXAxis + 2;
			lastSizeY = building.BuildingFoundationOffsetsOnZAxis + 2;
			InitializeGrid(lastSizeX, lastSizeY);
		}

		placementGrid.transform.SetParent(building.transform, false);
		placementGrid.transform.localPosition = new Vector3(0f, FX_HEIGHT, 0f);
		placementGrid.SetActive(true);
	}

	private void OnPlacementFinished()
	{
		placementGrid.transform.SetParent(null);
		placementGrid.SetActive(false);
	}

	/// <summary>
	/// A UnityEvent that triggers when OnPlacementValid() is called.
	/// Change building area's color to a color that represents a valid placement.
	/// </summary>
	public void OnPlacementValid()
	{
		buildingAreaRenderer.color = validColor;
	}

	/// <summary>
	/// A UnityEvent that triggers when OnPlacementInvalid() is called.
	/// Change building area's color to a color that represents an invalid placement.
	/// </summary>
	public void OnPlacementInvalid()
	{
		buildingAreaRenderer.color = invalidColor;
	}
}
