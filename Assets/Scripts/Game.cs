using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game : MonoBehaviour
{

    public static UIManager uiManager;//TODO:使用static的原因

    public static bool isLoaded = false;

    private void Awake()
    {
        if (isLoaded == true)
        {
            Destroy(gameObject);
        }
        else
        {
            isLoaded = true;
            DontDestroyOnLoad(gameObject);//跳转场景当前游戏物体不删除
            uiManager = new UIManager();
            uiManager.Init();

            //设置发送 接收消息频率 降低延迟
            PhotonNetwork.SendRate = 50;
            PhotonNetwork.SerializationRate = 50;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //显示登陆界面
        uiManager.ShowUI<LoginUI>("LoginUI");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
