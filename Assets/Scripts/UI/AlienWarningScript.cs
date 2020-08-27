using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienWarningScript : MonoBehaviour
{
    [SerializeField] private Transform targetposition;
    [SerializeField] private RectTransform pointerRectTransform;
    [SerializeField] private Camera minimapCamera;
    private bool isOffScreen;
    private float boardersize = 20f;
    private float clockShift = 40f;


    private void Update()
    {
        Vector3 toPosition = new Vector3 (targetposition.position.x, targetposition.position.y, targetposition.position.z);
        Vector3 fromPosition = minimapCamera.transform.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;

        Vector3 targetPositionScreenPoint = minimapCamera.WorldToScreenPoint(toPosition);
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
            if (cappedTargetScreenPosition.y >= minimapCamera.pixelHeight - boardersize - clockShift && cappedTargetScreenPosition.x >= minimapCamera.pixelWidth - boardersize - clockShift)
            {
                if (cappedTargetScreenPosition.x >= minimapCamera.pixelWidth - boardersize - clockShift) cappedTargetScreenPosition.x = minimapCamera.pixelWidth - boardersize - clockShift;
                if (cappedTargetScreenPosition.y >= minimapCamera.pixelHeight - boardersize - clockShift) cappedTargetScreenPosition.y = minimapCamera.pixelHeight - boardersize - clockShift;

            }

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

