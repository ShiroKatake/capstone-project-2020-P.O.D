using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public class DamagePointer : MonoBehaviour
{
    private static DamagePointer Instance;
    [SerializeField] private GameObject pointer;
    private Transform location;
    [SerializeField]private Camera minimapCamera;
    private RectTransform pointerRectTransform;
    private bool isOffScreen;
    private bool isActive;
    private float boardersize = 10f;
    private float clockShift = 20f;


    private float count = 0;
    private void Awake()
    {
        Instance = this;
        pointerRectTransform = pointer.GetComponent<RectTransform>();
        pointer.SetActive(false);
        location = this.transform;
        isActive = false;

    }
    private void Update()
    {
        if (isActive)
        {
            
            Vector3 toPosition = new Vector3(location.position.x, location.position.y, location.position.z);
            Vector3 targetPositionScreenPoint = minimapCamera.WorldToScreenPoint(toPosition);
            if (targetPositionScreenPoint.x <= boardersize || targetPositionScreenPoint.x >= minimapCamera.pixelWidth - boardersize || targetPositionScreenPoint.y <= boardersize || targetPositionScreenPoint.y >= minimapCamera.pixelHeight - boardersize)
            {
                isOffScreen = true;
            }
            else isOffScreen = false;

            if (isOffScreen)
            {
                if (count <= 0)
                {
                    AudioManager.Instance.PlaySound(AudioManager.ESound.Attacked, this.gameObject);
                    count = 2;
                }
                else
                {
                    count -= Time.deltaTime;
                }

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
                this.gameObject.SetActive(false);
                isActive = false;
            }

    

        }
        
        

    }

    public void Jump(Transform newLocation)
    {
        if (!isActive)
        {
            isActive = true;
            pointer.SetActive(true);
            location = newLocation;
            
        }
       
       
    }

    public static void Jump_Static(Transform newlocation)
    {
        Instance.Jump(newlocation);
    }

}
