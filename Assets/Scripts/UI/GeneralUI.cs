using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralUI : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    private void Awake() {
        //hotSpot = new Vector2(cursorTexture.width/2, cursorTexture.height/2);
    }

    public void OnMouseEnter()
    {
        //Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void OnMouseExit()
    {
        //Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    /*// Start is called before the first frame update
    void Start()
    {
        
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
