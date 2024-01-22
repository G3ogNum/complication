using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LossUI : MonoBehaviour
{
    public System.Action onClickCallBack;


    // Start is called before the first frame update
    void Start()
    {
        transform.Find("resetBtn").GetComponent<Button>().onClick.AddListener(OnClickBtn);
    }

    public void OnClickBtn()
    {
        if (onClickCallBack != null)
        {
            onClickCallBack();
        }

        Game.uiManager.CloseUI(gameObject.name);

    }

}
