using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateToolTips : MonoBehaviour
{
    [SerializeField] private ToolTips.Etooltips selection;
    public void tooltipON()
    {
        ToolTips.showTooltip_Static(selection);
    }
    public void tooltipOff()
    {
        ToolTips.hideToolTip_Static();
    }
}
