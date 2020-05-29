using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] private int MainGame;

    public void OnTrigger(){
        //print(MainGame.ToString());
        SceneManager.LoadScene(MainGame);
    }
}
