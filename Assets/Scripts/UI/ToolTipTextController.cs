using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipTextController : MonoBehaviour
{
    public enum ETextReferece
    {
        IceDrill,
        FusionReactor
    }
    [System.Serializable]
    public class Textbox_Control
    {
        [SerializeField] private string name;
        [SerializeField] public ETextReferece TextReferece;
        [SerializeField] public string[] strings;
        [SerializeField] private bool livingtext;
    }

  //  [System.Serializable]
   // public class Textbox_Location_Control
   //{
   //   [SerializeField] private string name;
   //  [SerializeField] public ETextReferece TextReferece;
   // [SerializeField] private float[] xloactions;
   // [SerializeField] private float[] ylocations;
   // }

    [System.Serializable]
    public class Textbox_PrefabManager
    {
        [SerializeField] private string name;
        [SerializeField] private ETextReferece TextReferece;
        [SerializeField] private GameObject tooltipPrefab;
        [SerializeField] private Text[] textobjects;
    }

    [SerializeField] private Font font;


    [SerializeField] private Textbox_Control[] textbox_Controls;
    [SerializeField] private Textbox_PrefabManager[] textbox_PrefabManagers;

    public void populateTexts(GameObject tooltip)
    {
        
    }
}
