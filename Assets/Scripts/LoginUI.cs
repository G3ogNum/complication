using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
//��½����
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
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("���ӷ�����...");

        //����pun2������
        PhotonNetwork.ConnectUsingSettings();//�ɹ����ִ��OnConnectedToMaster()
    }

    public void onQuitBtn()
    {
        Application.Quit();
    }


    private void OnEnable()
    {
        //ע��pun2�¼�
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        //ע��pun2�¼�
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public void OnConnected()
    {
        
    }

    //���ӳɹ���ִ�еĺ���
    public void OnConnectedToMaster()
    {
        //�ر�ȫ��ui
        Game.uiManager.CloseAllUI();
        Debug.Log("���ӳɹ�");

        //��ʾ��������
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
    }

    //�Ͽ�������ִ�еĻص�
    public void OnDisconnected(DisconnectCause cause)
    {
        //�ر�����
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
