using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mining_UI : MonoBehaviour
{
    [SerializeField] ToolTips.Etooltips reference;

  
    void OnMouseOver()
    {
        ToolTips.Instance.ShowtoolTip(reference);
        Debug.Log("Hovering over a mineral");
    }

    void OnMouseExit()
    {
        ToolTips.Instance.HideToolTip();
    }
}
