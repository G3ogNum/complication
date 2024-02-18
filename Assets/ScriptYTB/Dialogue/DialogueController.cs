using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;

    bool canTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    private void Update()
    {
        if(Camera.main!=null&&Camera.main.transform.root.GetComponent<PlayerRogue>().dialogueUI.dialoguePanel.activeSelf==false)
            if (canTalk && Input.GetKeyDown(KeyCode.E))
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

                RaycastHit hit;

                if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
                {
                    if (hit.transform.gameObject == gameObject)
                    {
                        //begin the dialogue
                        OpenDialogue(Camera.main.transform.root.GetComponent<PlayerRogue>().dialogueUI);
                    }
                }
            }   
    }

    void OpenDialogue(DialogueUI dialogueUI)
    {
        Debug.Log("open");
        //Open UI panel
        dialogueUI.UpdateDialogueData(currentData);
        //transmit information to player
        dialogueUI.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }
}
