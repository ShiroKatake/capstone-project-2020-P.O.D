using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateToolTips : MonoBehaviour
{
    [SerializeField] private ToolTips.Etooltips selection;
    public void tooltipON()
    {
        ToolTips.Instance.ShowtoolTip(selection);
    }
    public void tooltipOff()
    {
        ToolTips.Instance.HideToolTip();
    }
}
