using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionButtonControl : MonoBehaviour
{
    private Button thisButton;
    public int Index = -1;
    private void Awake()
    {
        thisButton = GetComponent<Button>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //enable last button
            transform.parent.GetChild((Index + transform.parent.childCount - 1) % transform.parent.childCount).GetComponent<Button>().enabled = true;
            transform.parent.GetChild((Index + transform.parent.childCount - 1) % transform.parent.childCount).GetComponent<Image>().color = new Color(0, 0, 0, .56f);
            transform.parent.GetChild((Index + transform.parent.childCount - 1) % transform.parent.childCount).GetComponent<OptionButtonControl>().enabled = true;
            //this button back to disable
            thisButton.enabled = false;
            GetComponent<Image>().color = new Color(.77f, .77f, .77f, .77f);
            GetComponent<OptionButtonControl>().enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //enable next button
            transform.parent.GetChild((Index + 1) % transform.parent.childCount).GetComponent<Button>().enabled = true;
            transform.parent.GetChild((Index + 1) % transform.parent.childCount).GetComponent<Image>().color = new Color(0, 0, 0, .56f);
            transform.parent.GetChild((Index + 1) % transform.parent.childCount).GetComponent<OptionButtonControl>().enabled = true;
            //this button back to disable
            thisButton.enabled = false;
            GetComponent<Image>().color = new Color(.77f, .77f, .77f, .77f);
            GetComponent<OptionButtonControl>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            thisButton.onClick.Invoke();
        }
    }
}
