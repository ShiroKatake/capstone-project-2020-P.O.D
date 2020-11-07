using UnityEngine;
using TMPro;

public class GOMessageManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI message;
	[TextArea]
	[SerializeField] private string winMessage;
	[TextArea]
	[SerializeField] private string loseMessage;

	[Header("Testing")]
	[SerializeField] private bool victory;
	[SerializeField] private bool defeat;

	private void Update()
	{
        if (!PauseMenuManager.Paused)
        {
            if (victory)
            {
                SetText(true);
            }
            else if (defeat)
            {
                SetText(false);
            }
        }
	}

	public void SetText(bool win){
        PauseMenuManager.Instance.CanPause = false;
        TerraformingUI.Instance.CanDisplay = false;

        if (win) {
			title.text = "Victory!";
            message.text = winMessage;
        } else {
			title.text = "Game Over!";
			message.text = loseMessage;
        }
    }
}
