using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTips : MonoBehaviour
{
    //All variables being set up setting up an Enum to direct what tooltip should pop up
    private static ToolTips instance;
    [SerializeField]private bool testbuttonChange;
    [SerializeField] private bool testbuttonHide;
    [SerializeField] private Camera uiCamera;
    public enum Etooltips
    {
        Testimage_1,
        Testimage_2
    }
    //ToolTip Class
    [System.Serializable]
    public class ToolTip
    {
        [SerializeField] private string name;
        [SerializeField] public Etooltips reference;
        [SerializeField] public Sprite Tip; 
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
        ShowtoolTip(Etooltips.Testimage_1);
    }
    //Makes the object follow the mouse
    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);
        transform.localPosition = localPoint;
        testChange();
    }
    //Shows the tooltip and changes the sprite to the correct image
    private void ShowtoolTip(Etooltips toolImage)
    {
        gameObject.SetActive(true);

        tooltip.sprite = tooltipDictionary[toolImage];
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

    public void testChange()
    {
        if (testbuttonChange == true)
        {
            ShowtoolTip(Etooltips.Testimage_1);
        }
        else if (testbuttonChange == false)
        {
            ShowtoolTip(Etooltips.Testimage_2);
        }

        


    }
    // What is to be called from other classes when they want to show or hide the tool tip remotely
    public static void showTooltip_Static(Etooltips toolImage)
    {
        instance.ShowtoolTip(toolImage);
    }

    public static void hideToolTip_Static(Etooltips toolImage)
    {
        instance.HideToolTip();
    }
    
}
