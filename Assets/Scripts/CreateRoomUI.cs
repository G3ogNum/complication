using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class CreateRoomUI : MonoBehaviourPunCallbacks
{

    InputField roomNameInput;


    // Start is called before the first frame update
    void Start()
    {
        transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(onCloseBtn);
        transform.Find("bg/okBtn").GetComponent<Button>().onClick.AddListener(onCreateBtn);
        
        
        roomNameInput = transform.Find("bg/InputField").GetComponent<InputField>();


        //随机一个房间名称
        roomNameInput.text = "room" + Random.Range(1, 9999);
    }


    //创建房间
    public void onCreateBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("创建中...");
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 8;//房间最大玩家数
        PhotonNetwork.CreateRoom(roomNameInput.text, room);//两参数意义为：1.房间名称 2.对象房间
    }


    public void onCloseBtn()
    {
        Game.uiManager.CloseUI(gameObject.name);
    }

    //创建成功的回调函数
    public override void OnCreatedRoom()
    {
        Debug.Log("创建成功");
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    //创建失败的回调函数
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Game.uiManager.CloseUI("MaskUI");
    }
}
