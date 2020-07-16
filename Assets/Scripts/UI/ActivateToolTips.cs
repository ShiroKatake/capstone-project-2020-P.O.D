using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateToolTips : MonoBehaviour
{
    public void tooltipON()
    {
        ToolTips.showTooltip_Static(ToolTips.Etooltips.Boilier);
    }
    public void tooltipOff()
    {
        ToolTips.hideToolTip_Static();
    }
}
