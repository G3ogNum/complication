using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    //public KeyCode triggerKey=KeyCode.Tab; 
    private void Update()
    {
        //this model could be made inside one with gameEscapeButton
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GetComponent<Button>().onClick.Invoke();
            ChangeLockCursor();
        }
    }
    public void ChangeLockCursor()
    {
        // make the cursor hidden and locked
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
    }
}
