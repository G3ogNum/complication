using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameSettingsUI : MonoBehaviour
{
    public Dropdown ScreenSettingDropdown;
    public Button ScreenConfirmButton;

    private int ScreenResolutionValue=0;

   

    public void ChangeScreenResolutionValue(int _value)
    {
        ScreenResolutionValue = _value;
    }

    public void ConfirmChangeScreenResolution()
    {
        if(ScreenResolutionValue==0)
            Screen.SetResolution(1920, 1080, true);
        else if(ScreenResolutionValue==1)
            Screen.SetResolution(2560, 1440, true);
        else if (ScreenResolutionValue == 2)
            Screen.SetResolution(800, 600, false);
    }
}
