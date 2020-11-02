using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;


public class MouseOverGraphic : MonoBehaviour
{
    [SerializeField] GraphicRaycaster Raycaster;
    PointerEventData EventData;
    [SerializeField] EventSystem evntSys;

    public static MouseOverGraphic Instance = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != null) Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Raycaster == null){ Raycaster = GetComponent<GraphicRaycaster>(); }
        if (evntSys == null){ evntSys = GetComponent<EventSystem>(); }
        
    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/

    public bool IsOverGraphic(){
        EventData = new PointerEventData(evntSys);
        EventData.position = ReInput.controllers.Mouse.screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();

        Raycaster.Raycast(EventData, results);

        if (results.Count != 0) {
            foreach(RaycastResult r in results){
                //Debug.Log(r.gameObject.name);
                if (r.gameObject.GetComponent<MouseClickThrough>() != null)
                {
                    //Debug.Log("Over UI");
                    return true;
                }
            }
        }

        //Debug.Log("Not Over UI");
        return false;
    }
}
