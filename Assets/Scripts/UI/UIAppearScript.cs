using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAppearScript : MonoBehaviour
{

    [Header("Element 1")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private bool isShowingUICanvas;

    [Header("Element 2 <--- This one is inactive at start")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private bool isShowingGameOverCanvas;

    public GameObject Canvas {get => gameOverCanvas;}

    // Start is called before the first frame update
    void Start()
    {
        uiCanvas.SetActive(isShowingUICanvas);
        gameOverCanvas.SetActive(isShowingGameOverCanvas);
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
        isShowingGameOverCanvas = !isShowingGameOverCanvas;
            gameOverCanvas.SetActive(isShowingGameOverCanvas);
            if (isShowingGameOverCanvas){
                gameOverCanvas.GetComponent<UIButtonInitialise>().Initialise();
            }
    }

    public void DeactivateGameOverCanvas()
    {
        gameOverCanvas.SetActive(false);
    }
}
