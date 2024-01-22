using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


//大厅界面
public class LobbyUI : MonoBehaviourPunCallbacks
{
    public TypedLobby lobby;//大厅对象

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

        lobby = new TypedLobby("fpsLobby", LobbyType.SqlLobby);//1.大厅名字 2.大厅类型（可搜索）
        //进入大厅
        PhotonNetwork.JoinLobby(lobby);

        PhotonNetwork.GetCustomRoomList(lobby, "1");
        //Debug.Log(PhotonNetwork.InLobby);
        //Debug.Log("start");

        //这里应该在某个逻辑上的生命函数后执行，而不是物理时间上的0.1s
        //Invoke("updateRoomEveryTime", 1f);
        //updateRoomEveryTime();
        //InvokeRepeating("updateRoomEveryTime", 1, 5);
    }

    public void updateRoomEveryTime()
    {
        PhotonNetwork.GetCustomRoomList(lobby, "1");
    }


    //进入大厅回调
    public override void OnJoinedLobby()
    {
        onUpdateRoomBtn();
        Debug.Log("进入大厅");
    }

    //关闭大厅界面
    public void onCloseBtn()
    {
        //断开链接
        PhotonNetwork.Disconnect();
        Game.uiManager.CloseUI(gameObject.name);
        //显示登录界面
        Game.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    //创建房间
    public void onCreateRoom()
    {

        
        Debug.Log("确认创建");
        Game.uiManager.ShowUI<CreateRoomUI>("CreateRoomUI");
    }

    //刷新房间列表
    public void onUpdateRoomBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("刷新中...");
        Debug.Log(PhotonNetwork.InLobby);
        PhotonNetwork.GetCustomRoomList(lobby,"1");//执行该方法后会触发OnRoomListUpdate回调
    }

    

    //清除已经存在的房间物体
    private void ClearRoomList()
    {
        while (contentTf.childCount != 0)
        {
            DestroyImmediate(contentTf.GetChild(0).gameObject);
        }
    }

    //刷新房间后的回调
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Game.uiManager.CloseUI("MaskUI");
        Debug.Log("房间刷新");

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

                Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("加入中...");
                PhotonNetwork.JoinRoom(roomName);//加入房间
            });
        }
    }

    public override void OnJoinedRoom()
    {
        //加入房间回调
        
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Game.uiManager.CloseUI("MaskUI");
    }
}
