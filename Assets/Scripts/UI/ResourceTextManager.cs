using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResourceTextManager : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  
    //Serialized Fields----------------------------------------------------------------------------                                                    
    [SerializeField] private bool visibleOnAwake;
    [SerializeField] private float fadeInSpeed;
    [Header("Text Boxes")]
    [SerializeField] private TextMeshProUGUI ore;
    [SerializeField] private TextMeshProUGUI water;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private TextMeshProUGUI waste;
    [Header("Icons")]
    [SerializeField] private Image oreIcon;
    [SerializeField] private Image waterIcon;
    [SerializeField] private Image powerIcon;
    [SerializeField] private Image wasteIcon;
    //Non-Serialized Fields------------------------------------------------------------------------
    private List<Graphic> graphics;
    private float opacity;
    //Public Properties------------------------------------------------------------------------------------------------------------------------------
    //Singleton Public Property--------------------------------------------------------------------            
    public static ResourceTextManager Instance {get; protected set;}
    //think about using custom setters and getters here...
    /*
    int x {
        get {return x;}
        set {
            //custom setter code
        }
    }
    */
    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("There should never be 2 or more Resource Text Managers in the scene.");
        }
        Instance = this;
        if (!visibleOnAwake)
        {
            List<Graphic> initialisationGraphics = new List<Graphic>() { ore, water, power, waste, oreIcon, waterIcon, powerIcon, wasteIcon };
            graphics = new List<Graphic>();
            foreach (Graphic g in initialisationGraphics)
            {
                if (g != null && !graphics.Contains(g))
                {
                    graphics.Add(g);
                }
            }
            opacity = 0;
            foreach (Graphic g in graphics)
            {
                g.color = UpdateColorOpacity(g.color, opacity);
            }
        }
    }
    /// <summary>
    /// Update() is run every frame.
    /// </summary>
	private void Update()
	{
		ore.text = ResourceController.Instance.Ore.ToString();
		power.text = ResourceController.Instance.PowerConsumption.ToString() + " / " + ResourceController.Instance.PowerSupply.ToString();
		waste.text = ResourceController.Instance.WasteConsumption.ToString() + " / " + ResourceController.Instance.WasteSupply.ToString();
		water.text = ResourceController.Instance.WaterConsumption.ToString() + " / " + ResourceController.Instance.WaterSupply.ToString();
	}
    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Triggers the fading in of the resources text.
    /// </summary>
    public void FadeIn()
    {
        StartCoroutine(FadingIn());
    }
    /// <summary>
    /// Fades the resources text in.
    /// </summary>
    private IEnumerator FadingIn()
    {
        while (opacity < 1)
        {
            opacity += fadeInSpeed * Time.deltaTime;
            foreach (Graphic g in graphics)
            {
                g.color = UpdateColorOpacity(g.color, opacity);
            }
            yield return null;
        }
    }
    /// <summary>
    /// Updates the opacity of a colour.
    /// </summary>
    /// <param name="colour">The colour whose opacity is to be updated.</param>
    /// <param name="opacity">The value to set the colour's opacity to.</param>
    /// <returns>The updated colour</returns>
    private Color UpdateColorOpacity(Color colour, float opacity)
    {
        colour.a = opacity;
        return colour;
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceTextManager : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private bool visibleOnAwake;
    [SerializeField] private float fadeInSpeed;
    [SerializeField] private Color textColour;
    [SerializeField] private TextMeshProUGUI ore;
    [SerializeField] private TextMeshProUGUI water;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private TextMeshProUGUI waste;
    [SerializeField] private Image oreSP;
    [SerializeField] private Image waterSP;
    [SerializeField] private Image powerSP;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------            

    public static ResourceTextManager Instance {get; protected set;}

    //think about using custom setters and getters here...
    /*
    int x {
        get {return x;}
        set {
            //custom setter code
        }
    }
    *\/

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("There should never be 2 or more Resource Text Managers in the scene.");
        }

        Instance = this;

        if (!visibleOnAwake)
        { 
            textColour.a = 0;
            ore.color = textColour;
            water.color = textColour;
            power.color = textColour;
            waste.color = textColour;

            var tmpColor = oreSP.color;
            tmpColor.a = 0; 
            oreSP.color = tmpColor;

            tmpColor = waterSP.color;
            tmpColor.a = 0; 
            waterSP.color = tmpColor;

            tmpColor = powerSP.color;
            tmpColor.a = 0; 
            powerSP.color = tmpColor;
        }
    }

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
	private void Update()
	{
		ore.text = ResourceController.Instance.Ore.ToString();
		power.text = ResourceController.Instance.PowerConsumption.ToString() + " / " + ResourceController.Instance.PowerSupply.ToString();
		waste.text = ResourceController.Instance.WasteConsumption.ToString() + " / " + ResourceController.Instance.WasteSupply.ToString();
		water.text = ResourceController.Instance.WaterConsumption.ToString() + "/" + ResourceController.Instance.WaterSupply.ToString();
	}

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Triggers the fading in of the resources text.
    /// </summary>
    public void FadeIn()
    {
        StartCoroutine(FadingIn());
    }

    /// <summary>
    /// Fades the resources text in.
    /// </summary>
    private IEnumerator FadingIn()
    {
        while (textColour.a < 1)
        {
            textColour.a += fadeInSpeed * Time.deltaTime;
            ore.color = textColour;
            water.color = textColour;
            power.color = textColour;
            waste.color = textColour;

            var tmpColor = oreSP.color;
            tmpColor.a = fadeInSpeed * Time.deltaTime; 
            oreSP.color = tmpColor;

            tmpColor = waterSP.color;
            tmpColor.a = fadeInSpeed * Time.deltaTime; 
            waterSP.color = tmpColor;

            tmpColor = powerSP.color;
            tmpColor.a = fadeInSpeed * Time.deltaTime; 
            powerSP.color = tmpColor;

            yield return null;
        }
    }
    */