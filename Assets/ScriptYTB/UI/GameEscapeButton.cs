using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEscapeButton : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<Button>().onClick.Invoke();
            ChangeLockCursor();
        }
    }
    public void ChangeLockCursor()
    {
        // make the cursor hidden and locked
        Cursor.lockState = Cursor.lockState == CursorLockMode.None? CursorLockMode.Locked: CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
    }
}
