using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonInitialise : MonoBehaviour
{

    [SerializeField] ButtonInteract[] buttons;

    private void Awake() {}

    public void Initialise(){
        foreach (ButtonInteract btn in buttons){
            btn.SetDefault();
        }
    }
}
