using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mining_UI : MonoBehaviour
{
    [SerializeField] ToolTips.Etooltips reference;

  
    void OnMouseOver()
    {
        ToolTips.showTooltip_Static(reference);
        Debug.Log("Hovering over a mineral");
    }

    void OnMouseExit()
    {
        ToolTips.hideToolTip_Static();
    }
}
