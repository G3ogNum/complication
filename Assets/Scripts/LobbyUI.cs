using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


//��������
public class LobbyUI : MonoBehaviourPunCallbacks
{
    public TypedLobby lobby;//��������

    private Transform contentTf;
    private GameObject roomPrefab;

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("content/title/closeBtn").GetComponent<Button>().onClick.AddListener(onCloseBtn);
        transform.Find("content/createBtn").GetComponent<Button>().onClick.AddListener(onCreateRoom);
        transform.Find("content/updateBtn").GetComponent<Button>().onClick.AddListener(onUpdateRoomBtn);
        contentTf = transform.Find("content/Scroll View/Viewport/Content");
        roomPrefab= transform.Find("content/Scroll View/Viewport/item").gameObject;

        lobby = new TypedLobby("fpsLobby", LobbyType.SqlLobby);//1.�������� 2.�������ͣ���������
        //�������
        PhotonNetwork.JoinLobby(lobby);

        PhotonNetwork.GetCustomRoomList(lobby, "1");
        //Debug.Log(PhotonNetwork.InLobby);
        //Debug.Log("start");

        //����Ӧ����ĳ���߼��ϵ�����������ִ�У�����������ʱ���ϵ�0.1s
        //Invoke("updateRoomEveryTime", 1f);
        //updateRoomEveryTime();
        //InvokeRepeating("updateRoomEveryTime", 1, 5);
    }

    public void updateRoomEveryTime()
    {
        PhotonNetwork.GetCustomRoomList(lobby, "1");
    }


    //��������ص�
    public override void OnJoinedLobby()
    {
        onUpdateRoomBtn();
        Debug.Log("�������");
    }

    //�رմ�������
    public void onCloseBtn()
    {
        //�Ͽ�����
        PhotonNetwork.Disconnect();
        Game.uiManager.CloseUI(gameObject.name);
        //��ʾ��¼����
        Game.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    //��������
    public void onCreateRoom()
    {

        
        Debug.Log("ȷ�ϴ���");
        Game.uiManager.ShowUI<CreateRoomUI>("CreateRoomUI");
    }

    //ˢ�·����б�
    public void onUpdateRoomBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("ˢ����...");
        Debug.Log(PhotonNetwork.InLobby);
        PhotonNetwork.GetCustomRoomList(lobby,"1");//ִ�и÷�����ᴥ��OnRoomListUpdate�ص�
    }

    

    //����Ѿ����ڵķ�������
    private void ClearRoomList()
    {
        while (contentTf.childCount != 0)
        {
            DestroyImmediate(contentTf.GetChild(0).gameObject);
        }
    }

    //ˢ�·����Ļص�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Game.uiManager.CloseUI("MaskUI");
        Debug.Log("����ˢ��");

        ClearRoomList();
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject obj = Instantiate(roomPrefab, contentTf);
            obj.SetActive(true);
            string roomName = roomList[i].Name;
            obj.transform.Find("roomName").GetComponent<Text>().text = roomName;
            obj.transform.Find("joinBtn").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                Debug.Log(roomName);

                Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("������...");
                PhotonNetwork.JoinRoom(roomName);//���뷿��
            });
        }
    }

    public override void OnJoinedRoom()
    {
        //���뷿��ص�
        
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Game.uiManager.CloseUI("MaskUI");
    }
}
