using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] private int sceneToSwitchTo;

    public void OnTrigger(){
        //print(MainGame.ToString());
        SceneManager.LoadScene(sceneToSwitchTo);
    }
}
