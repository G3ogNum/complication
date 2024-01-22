using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ExitGameButton : MonoBehaviour
{
    
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ExitGame);
    }

    void ExitGame()
    {
        Debug.LogWarning("quit");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
