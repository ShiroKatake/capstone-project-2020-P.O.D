using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GOMessageController : MonoBehaviour
{
    [SerializeField] Text message;

    public void SetText(bool win){
        if (win) {
            message.text = "Congratulations!!\nYou have WON!!!";
        } else {
            message.text = "Bummer!!\nYou have Lost...\nTry again next time!!";
        }
    }
}
