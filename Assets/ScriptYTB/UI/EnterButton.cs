using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnterButton : MonoBehaviour,IBackButton
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            GetComponent<Button>().onClick.Invoke();
    }
    public void onButtonClicked()
    {
        throw new System.NotImplementedException();
    }
}
