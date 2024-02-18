using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OptionUI : MonoBehaviour
{
    public TextMeshProUGUI optionText;
    private Button thisButton;
    private DialoguePiece currentPiece;
    private string nextPieceID;

    

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);

        /*if(transform.parent.GetChild(0)==gameObject)
        {
            thisButton.enabled = true;
            GetComponent<Image>().color = new Color(0, 0, 0);
            GetComponent<OptionButtonControl>().enabled = true;
        }
        else
        {
            thisButton.enabled = false;
            GetComponent<Image>().color = new Color(196, 196, 196);
            GetComponent<OptionButtonControl>().enabled = false;
        }*/
    }
    
    public void UpdateOption(DialoguePiece piece,DialogueOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetId;
    }

    public void OnOptionClicked()
    {
        if(nextPieceID=="")
        {
            transform.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            transform.root.GetComponent<PlayerRogue>().
                dialogueUI.UpdateMainDialogue(transform.root.GetComponent<PlayerRogue>().
                dialogueUI.currentData.dialogueIndex[nextPieceID]);
        }
    }

    
}
