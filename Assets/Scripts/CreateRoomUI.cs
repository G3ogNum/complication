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


        //���һ����������
        roomNameInput.text = "room" + Random.Range(1, 9999);
    }


    //��������
    public void onCreateBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("������...");
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 8;//������������
        PhotonNetwork.CreateRoom(roomNameInput.text, room);//����������Ϊ��1.�������� 2.���󷿼�
    }


    public void onCloseBtn()
    {
        Game.uiManager.CloseUI(gameObject.name);
    }

    //�����ɹ��Ļص�����
    public override void OnCreatedRoom()
    {
        Debug.Log("�����ɹ�");
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    //����ʧ�ܵĻص�����
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Game.uiManager.CloseUI("MaskUI");
    }
}
