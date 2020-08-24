using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTips : SerializableSingleton<ToolTips>
{
    //All variables being set up setting up an Enum to direct what tooltip should pop up
    [SerializeField] private Camera uiCamera;

    private GameObject current_tooltip;
    private Etooltips currentImage;

    private bool test = false;
    public enum Etooltips
    {
        IceDrill,
        FusionReactor,
        Incinerator,
        Boiler,
        Greenhouse,
        Shotgun,
        MachineGun,
        Ratiobars,
        Mining_Nodes
    }
    //ToolTip Class
    [System.Serializable]
    public class ToolTip
    {
        [SerializeField] private string name;
        [SerializeField] public Etooltips reference;
        [SerializeField] public GameObject Tip;
        [SerializeField] public Transform tooltipLocation;
    }
    //Defines the dictionary that allows to call the image within the class
    private Dictionary<Etooltips, GameObject> tooltipDictionary;
    private Dictionary<Etooltips, Transform> tooltipLocationDictionary;
    
    //Array of tool tips
    [SerializeField] private ToolTip[] tooltipImages;

    protected override void Awake()
    {
        base.Awake();
        
            //spriteRenderer = tooltip.GetComponent<SpriteRenderer>();
        //Builds the dictionary
        tooltipDictionary = new Dictionary<Etooltips, GameObject>();
        tooltipLocationDictionary = new Dictionary<Etooltips, Transform>();
        
        foreach (ToolTip entry in tooltipImages)
        {
            tooltipDictionary.Add(entry.reference, entry.Tip);
            tooltipLocationDictionary.Add(entry.reference, entry.tooltipLocation);
        }

        //Hides the tooltip
        //HideToolTip();
    }


    // private void mousefollow(Etooltips tooltip)
    // {
    //     foreach (ToolTip entry in tooltipImages)
    //     {
    //         if (entry.reference == tooltip)
    //         {
    //             if (entry.followMouse == true)
    //             {
    //                 mousefollowingChecker = true;
    //             }
    //             else
    //             {
    //                 mousefollowingChecker = false;
    //             }
    //         }
    //     }
    // }

    //private void updateLocation(Etooltips tooltip)
    //{
    //    foreach (ToolTip entry in tooltipImages)
    //    {
    //        if (entry.reference == tooltip)
    //        {
    //            Location = entry.tooltipLocation;
    //        }
    //    }
    //}
    //Shows the tooltip and changes the sprite to the correct image

    public void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            if (tooltipLocationDictionary[currentImage] != null)
            {
                if (current_tooltip != null)
                {
                    current_tooltip.transform.position = tooltipLocationDictionary[currentImage].position;
                }
            }
        }
    }
    public void ShowToolTip(Etooltips toolImage)
    {
        //mousefollow(toolImage);
        //updateLocation(toolImage);
        //gameObject.SetActive(true);
      
       current_tooltip = Instantiate(tooltipDictionary[toolImage]);
       current_tooltip.transform.parent = gameObject.transform;
       currentImage = toolImage;
     
        //GameObject current_tooltip = Instantiate(tooltipDictionary[toolImage],newpos, Quaternion.identity);
       
        
        //spriteRenderer.sprite = tooltipDictionary[toolImage];
        //float imagePaddingSize = 5f;
        //Vector2 backgroundSize = new Vector2(tooltip.preferredWidth + imagePaddingSize * 2, tooltip.preferredHeight + imagePaddingSize * 2);
        //backgroundRectTransform.sizeDelta = backgroundSize;
    }

    //private void ShowtoolTip()
    //{
    //    gameObject.SetActive(true);

    //}
    // removes the tooltip from view
    public void HideToolTip()
    {
        GameObject tobeKilled = this.transform.GetChild(transform.childCount - 1).gameObject;
        Destroy(tobeKilled);
        //gameObject.SetActive(false);
    }   
}
