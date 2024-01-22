using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
//登陆界面
public class LoginUI : MonoBehaviour,IConnectionCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("startBtn").GetComponent<Button>().onClick.AddListener(onStartBtn);
        transform.Find("quitBtn").GetComponent<Button>().onClick.AddListener(onQuitBtn);
    }


    public void onStartBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("连接服务器...");

        //连接pun2服务器
        PhotonNetwork.ConnectUsingSettings();//成功后会执行OnConnectedToMaster()
    }

    public void onQuitBtn()
    {
        Application.Quit();
    }


    private void OnEnable()
    {
        //注册pun2事件
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        //注销pun2事件
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public void OnConnected()
    {
        
    }

    //连接成功后执行的函数
    public void OnConnectedToMaster()
    {
        //关闭全部ui
        Game.uiManager.CloseAllUI();
        Debug.Log("连接成功");

        //显示大厅界面
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
    }

    //断开服务器执行的回调
    public void OnDisconnected(DisconnectCause cause)
    {
        //关闭遮罩
        Game.uiManager.CloseUI("MaskUI");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {

    }
}
