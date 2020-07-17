using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Camera uiCamera;
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;
    
    void Update()
    {
        Vector2 localpoint;
        Vector3 shift = new Vector3(Input.mousePosition.x + offsetX, Input.mousePosition.y + offsetY, 0);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), shift, uiCamera, out localpoint);
        transform.localPosition = localpoint;
    }
}
