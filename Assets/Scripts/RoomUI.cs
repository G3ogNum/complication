using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour,IInRoomCallbacks
{
    Transform startTf;
    Transform contentTf;
    GameObject roomPrefab;//��ָ���
    public List<RoomItem> roomList;
    private void Awake()
    {
        roomList = new List<RoomItem>();
        contentTf = transform.Find("bg/Content");
        roomPrefab = transform.Find("bg/roomItem").gameObject;
        transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(onCloseBtn);

        startTf = transform.Find("bg/startBtn");
        startTf.GetComponent<Button>().onClick.AddListener(onStartBtn);

        PhotonNetwork.AutomaticallySyncScene = true;//�÷��������Ҷ�ִ��PhotonNetwork.LoadLevel����ת��ͬ�ĳ���
    }
    void Start()
    {
        //���ɷ�����������
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player p = PhotonNetwork.PlayerList[i];
            CreateRoomItem(p);
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    //�������
    public void CreateRoomItem(Player p)
    {
        GameObject obj = Instantiate(roomPrefab, contentTf);
        obj.SetActive(true);
        RoomItem item = obj.AddComponent<RoomItem>();
        item.owerId = p.ActorNumber;
        roomList.Add(item);


        object val;
        if(p.CustomProperties.TryGetValue("isReady",out val))
        {
            item.isReady = (bool)val;
        }

        //������������ �ж�������ҵ�׼��״̬
        if (PhotonNetwork.IsMasterClient)
        {
            bool isAllReady = true;
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].isReady == false)
                {
                    isAllReady = false;
                    break;
                }
            }
            startTf.gameObject.SetActive(isAllReady);
        }
    }

    //ɾ���뿪��������
    public void DeleteRoomItem(Player p)
    {
        RoomItem item = roomList.Find((RoomItem _item) => { return p.ActorNumber == _item.owerId; });
        if (item != null)
        {
            Destroy(item.gameObject);
            roomList.Remove(item);
        }
    }

    void onCloseBtn()
    {
       /* //�Ͽ�����
        PhotonNetwork.Disconnect();
        Game.uiManager.CloseUI(gameObject.name);
        Game.uiManager.ShowUI<LoginUI>("LoginUI");*/



        //�뿪����
        PhotonNetwork.LeaveRoom();
        Game.uiManager.CloseUI(gameObject.name);
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
        
        //lobby.onUpdateRoomBtn();

        
    }

    //��ʼ��Ϸ
    void onStartBtn()
    {
        //���س��� �÷���������Ҳ���س���
        PhotonNetwork.LoadLevel("game");
    }


    //����ҽ��뷿��
    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreateRoomItem(newPlayer);
    }


    //���������������뿪����
    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        DeleteRoomItem(otherPlayer);
    }




    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        
    }



    //����Զ���������»ص�
    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        RoomItem item = roomList.Find((_item) => {return _item.owerId == targetPlayer.ActorNumber; });
        if (item != null)
        {
            item.isReady = (bool)changedProps["isReady"];
            item.ChangeReady(item.isReady);
        }

        //������������ �ж�������ҵ�׼��״̬
        if (PhotonNetwork.IsMasterClient)
        {
            bool isAllReady = true;
            for(int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].isReady == false)
                {
                    isAllReady = false;
                    break;
                }
            }
            startTf.gameObject.SetActive(isAllReady);
        }
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        
    }
}
