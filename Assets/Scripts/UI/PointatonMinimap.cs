using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PointatonMinimap : MonoBehaviour { 

    [SerializeField] private Vector3 targetposition;
    [SerializeField] private RectTransform pointerRectTransform;
    [SerializeField] private Camera minimapCamera;
    private bool isOffScreen;
    private float boardersize = 20f;
  

    private void Update()
    {
        Vector3 toPosition = targetposition;
        Vector3 fromPosition = minimapCamera.transform.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;

        Vector3 targetPositionScreenPoint = minimapCamera.WorldToScreenPoint(targetposition);
        if (targetPositionScreenPoint.x <= boardersize || targetPositionScreenPoint.x >= minimapCamera.pixelWidth - boardersize || targetPositionScreenPoint.y <= boardersize || targetPositionScreenPoint.y >= minimapCamera.pixelHeight - boardersize)
        {
            isOffScreen = true;
        }
        else isOffScreen = false;

        if (isOffScreen)
        {
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= boardersize) cappedTargetScreenPosition.x = boardersize;
            if (cappedTargetScreenPosition.x >= minimapCamera.pixelWidth - boardersize) cappedTargetScreenPosition.x = minimapCamera.pixelWidth - boardersize;
            if (cappedTargetScreenPosition.y <= boardersize) cappedTargetScreenPosition.y = boardersize;
            if (cappedTargetScreenPosition.y >= minimapCamera.pixelHeight - boardersize) cappedTargetScreenPosition.y = minimapCamera.pixelHeight - boardersize;

            Vector3 pointerWorldPosision = minimapCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointerRectTransform.position = pointerWorldPosision;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        }
        else
        {
            Vector3 pointerWorldPosision = minimapCamera.ScreenToWorldPoint(targetPositionScreenPoint);
            pointerRectTransform.position = pointerWorldPosision;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        }

        
    }
}
