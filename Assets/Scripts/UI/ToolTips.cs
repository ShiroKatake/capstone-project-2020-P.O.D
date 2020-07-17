using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTips : MonoBehaviour
{
    //All variables being set up setting up an Enum to direct what tooltip should pop up
    private static ToolTips instance;
    private bool mousefollowingChecker;
    [SerializeField] private Etooltips StartingImage;
    [SerializeField] private Camera uiCamera;
    
    [SerializeField] private Transform Location;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 0f;

    [SerializeField] private bool printPosition;

    private bool test = false;
    public enum Etooltips
    {
        IceDrill,
        FusionReactor,
        Incinorator,
        Boilier,
        Greenhouse,
        Shotgun,
        MachineGun,
        Test
    }
    //ToolTip Class
    [System.Serializable]
    public class ToolTip
    {
        [SerializeField] private string name;
        [SerializeField] public Etooltips reference;
        [SerializeField] public Sprite Tip;
        [SerializeField] public bool followMouse;

        [SerializeField] public Transform tooltipLocation;
        [SerializeField] public float offsetX;
        [SerializeField] public float offsetY;
    }
    //Defines the dictionary that allows to call the image within the class
    private Dictionary<Etooltips, Sprite> tooltipDictionary;
    
    //Array of tool tips
    [SerializeField] private ToolTip[] tooltipImages;
    //What is being displayed on the screen
    [SerializeField] public Image tooltip;
        //[SerializeField] private RectTransform backgroundRectTransform;
        //private SpriteRenderer spriteRenderer;
    

    private void Awake()
    {
        instance = this;
       
        
            //spriteRenderer = tooltip.GetComponent<SpriteRenderer>();
        //Builds the dictionary
        tooltipDictionary = new Dictionary<Etooltips, Sprite>();
        
        foreach (ToolTip entry in tooltipImages)
        {
            tooltipDictionary.Add(entry.reference, entry.Tip);
        }

        //Hides the tooltip
        HideToolTip();
    }
    //Makes the object follow the mouse
    private void Update()
    {
        Vector2 localPoint;
        if (mousefollowingChecker == true)

        {
            Vector3 totalOffset = new Vector3(offsetX, offsetY, 0);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), (Input.mousePosition + totalOffset), uiCamera, out localPoint);

        }
        else
        {
            Vector2 newlocation = new Vector2(Location.position.x + offsetX, Location.position.y + offsetY);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), newlocation, uiCamera, out localPoint);

            if (printPosition)
            {
                Debug.Log(localPoint);
            }
        }

        transform.localPosition = localPoint;
        

    }

    private void mousefollow(Etooltips tooltip)
    {
        foreach (ToolTip entry in tooltipImages)
        {
            if (entry.reference == tooltip)
            {
                if (entry.followMouse == true)
                {
                    mousefollowingChecker = true;
                }
                else
                {
                    mousefollowingChecker = false;
                }
            }
        }
    }

    private void updateLocation(Etooltips tooltip)
    {
        foreach (ToolTip entry in tooltipImages)
        {
            if (entry.reference == tooltip)
            {
                Location = entry.tooltipLocation;
                offsetX = entry.offsetX;
                offsetY = entry.offsetY;

            }
        }
    }
    //Shows the tooltip and changes the sprite to the correct image
    public void ShowtoolTip(Etooltips toolImage)
    {
        gameObject.SetActive(true);

        tooltip.sprite = tooltipDictionary[toolImage];
        mousefollow(toolImage);
        updateLocation(toolImage);
        //spriteRenderer.sprite = tooltipDictionary[toolImage];
        //float imagePaddingSize = 5f;
        //Vector2 backgroundSize = new Vector2(tooltip.preferredWidth + imagePaddingSize * 2, tooltip.preferredHeight + imagePaddingSize * 2);
        //backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void ShowtoolTip()
    {
        gameObject.SetActive(true);

    }
    // removes the tooltip from view
    private void HideToolTip()
    {
        gameObject.SetActive(false);
    }

 
    // What is to be called from other classes when they want to show or hide the tool tip remotely
    public static void showTooltip_Static(Etooltips toolImage)
    {
        instance.ShowtoolTip(toolImage);
    }

    public static void hideToolTip_Static()
    {
        instance.HideToolTip();
    }
    
}
