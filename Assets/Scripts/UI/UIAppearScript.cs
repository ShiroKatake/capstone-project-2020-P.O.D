using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAppearScript : MonoBehaviour
{

    [Header("Element 1")]
    [SerializeField] private GameObject canvas1;
    [SerializeField] private bool isShowingC1;

    [Header("Element 2 <--- This one is inactive at start")]
    [SerializeField] private GameObject canvas2;
    [SerializeField] private bool isShowingC2;

    public GameObject Canvas {get => canvas2;}

    // Start is called before the first frame update
    void Start()
    {
        canvas1.SetActive(isShowingC1);
        canvas2.SetActive(isShowingC2);
    }

    // Update is called once per frame
    /*void Update()
    {
        if (PlayerMovementController.Instance.PlayerInputManager.GetButton("GeneralAction")){
            isShowing = !isShowing;
            canvas.SetActive(isShowing);
            canvas.GetComponent<UIButtonInitialise>().Initialise();
        }
    }*/

    public void ToggleVisibility(){
        isShowingC2 = !isShowingC2;
            canvas2.SetActive(isShowingC2);
            if (isShowingC2){
                canvas2.GetComponent<UIButtonInitialise>().Initialise();
            }
    }
}
