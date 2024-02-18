using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogueUI : MonoBehaviour
{
    [Header("Basic Element")]
    public Image icon;
    public TextMeshProUGUI mainText;
    public Button nextButton;

    public GameObject dialoguePanel;

    [Header("Options")]
    public RectTransform optionPanel;
    public Button optionPrefab;

    [Header("Datas")]
    public DialogueData_SO currentData;



    int currentIndex = 0;

    private void Update()
    {
        Debug.Log(currentIndex);
    }
    private void Awake()
    {
        nextButton.onClick.AddListener(ContinueDialogue);
    }
    void ContinueDialogue()
    {
        if (currentIndex < currentData.dialoguePieces.Count)
            UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);
        else
            dialoguePanel.SetActive(false);
    }
    public void UpdateDialogueData(DialogueData_SO data)
    {
        currentData = data;
        currentIndex = 0;
    }
    
    public void UpdateMainDialogue(DialoguePiece piece)
    {
        Debug.Log("dialogue");
        dialoguePanel.SetActive(true);
        currentIndex++;
        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
        {
            icon.enabled = false;
        }
        mainText.text = "";
        DOTween.To(() => string.Empty, value => mainText.text = value, piece.text, 1f).SetEase(Ease.Linear);

        if (piece.options.Count == 0 && currentData.dialoguePieces.Count > 0)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.GetComponent<KeyCodeE>().enabled = true;
            nextButton.transform.GetChild(0).gameObject.SetActive(true);
            
        }
        else
        {
            nextButton.GetComponent<KeyCodeE>().enabled = false;
            nextButton.transform.GetChild(0).gameObject.SetActive(false);
        }


        //create options
        CreateOptions(piece);
    }
    void CreateOptions(DialoguePiece piece)
    {
        if (optionPanel.childCount > 0)
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < piece.options.Count; i++)
        {
            var option = Instantiate(optionPrefab, optionPanel);
            option.GetComponent<OptionUI>().UpdateOption(piece, piece.options[i]);
            option.GetComponent<OptionButtonControl>().Index = i;
            if (i == 0)
            {
                option.GetComponent<Button>().enabled = true;
                option.GetComponent<Image>().color = new Color(0, 0, 0, .56f);
                option.GetComponent<OptionButtonControl>().enabled = true;
            }
            else
            {
                option.GetComponent<Button>().enabled = false;
                option.GetComponent<Image>().color = new Color(.77f, .77f, .77f, .77f);
                option.GetComponent<OptionButtonControl>().enabled = false;
            }
        }
    }

}
