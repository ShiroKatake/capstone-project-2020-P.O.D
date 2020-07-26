using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Rendering;

public class PointatonMinimap : MonoBehaviour {


    [SerializeField] private List<Pointer> pointerList;
    [SerializeField] private GameObject cryoegg;

    private void Start()
    {
        CreatePointer(cryoegg.transform, "CryoEgg Pointer");
        Debug.Log("I have been created from the PointatonMinimap");
    }

    private void Awake()
    {
        pointerList = new List<Pointer>();
    }
    private void Update()
    {
        foreach (Pointer pointer in pointerList)
        {
            pointer.Update();
        }
      
        
    }


    public Pointer CreatePointer(Transform targetPosition, string pointerObject)
    {
        GameObject pointerGameObject = Instantiate(transform.Find(pointerObject).gameObject);
        pointerGameObject.transform.SetParent(transform, false);
        pointerGameObject.SetActive(true);
        Camera minimapCamera = GameObject.Find("Minimap_Camera").GetComponent<Camera>();
        Pointer pointer = new Pointer(targetPosition, pointerGameObject,minimapCamera);
        pointerList.Add(pointer);
        return pointer;
    }
    public void DestroyPointers(Pointer pointer)
    {
        pointerList.Remove(pointer);
        pointer.destroySelf();
    }
    public class Pointer
    {
        private Transform targetPosition;
        private GameObject pointerGameObject;
        private RectTransform pointerRectTransform;
        private bool isOffScreen;
        private float boardersize = 25f;
        private float clockShift = 40f;
        private Camera minimapCamera;


        public Pointer (Transform targetPosition, GameObject pointerGameObject, Camera minimapCamera)
        {
            this.targetPosition = targetPosition;
            this.pointerGameObject = pointerGameObject;
            this.minimapCamera = minimapCamera;
            pointerRectTransform = pointerGameObject.GetComponent<RectTransform>();

        }

        public void Update()
        {
            Vector3 toPosition = new Vector3(targetPosition.position.x, targetPosition.position.y, targetPosition.position.z);


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

        public void destroySelf()
        {
            Destroy(pointerGameObject);
        }

        public void Show()
        {
            this.pointerGameObject.SetActive(true);
        }

        public void Hide()
        {
            this.pointerGameObject.SetActive(false);
        }

    }

}
