using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager instance;

    public GameObject player;
    [Space]
    public Transform[] TspwanPoints;
    public Transform[] CTspwanPoints;
    [Space]
    public GameObject roomCam;

    [Space]
    public GameObject nameUI;

    public GameObject connectingUI;

    public GameObject ChooseSideUI;

    private string nickname = "unnamed";

    public string roomNameToJoin="test";

    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;
    [HideInInspector]
    public int battleSide = 0;//T:0 , CT:1;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeNickName( string _name)
    {
        nickname = _name;
    }

    public void ChangeSide(int _side)
    {
        battleSide = _side;
        SetHashes();

        roomCam.SetActive(false);

        SpawnPlayer();

    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting...");

        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);

        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("We R Connected and in a room now");

        //this will be changed to choose your side


        //close connectingUI
        connectingUI.SetActive(false);

        //Open chosenUI
        ChooseSideUI.SetActive(true);
        
        //then change spawnPlayer SpawnPoints

        //and RespawnPlayer SpawnPoints
        
        
    }

    public void SpawnPlayer()
    {
        Transform spwanPoint;
        if (battleSide == 1)
        {
             spwanPoint = CTspwanPoints[UnityEngine.Random.Range(1, CTspwanPoints.Length)];

        }
        else
        {
             spwanPoint = TspwanPoints[UnityEngine.Random.Range(1, TspwanPoints.Length)];

        }

        GameObject _player = PhotonNetwork.Instantiate(player.name, spwanPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer = true;

        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        _player.GetComponent<PhotonView>().RPC("SetSide", RpcTarget.AllBuffered, battleSide);

        PhotonNetwork.LocalPlayer.NickName = nickname;
    }
    public Transform RespawnPlayer()
    {
        Transform spwanPoint;
        if (battleSide == 1)
        {
            spwanPoint = CTspwanPoints[UnityEngine.Random.Range(1, CTspwanPoints.Length)];

        }
        else
        {
            spwanPoint = TspwanPoints[UnityEngine.Random.Range(1, TspwanPoints.Length)];

        }
        return spwanPoint;

        /*GameObject _player = PhotonNetwork.Instantiate(player.name, spwanPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer=true;

        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);

        PhotonNetwork.LocalPlayer.NickName = nickname;*/
    }

    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            hash["kills"] = kills;
            hash["deaths"] = deaths;
            hash["side"] = battleSide;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
            //do nothing
        }
    }
}
