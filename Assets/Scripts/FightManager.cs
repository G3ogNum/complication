using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class FightManager : MonoBehaviour
{
    private void Awake()
    {
        //�������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //�ر����н���
        Game.uiManager.CloseAllUI();
        //��ʾս������
        Game.uiManager.ShowUI<FightUI>("FightUI");

        Transform pointTf = GameObject.Find("Point").transform;

        Vector3 pos = pointTf.GetChild(Random.Range(0, pointTf.childCount)).position;

        //ʵ������ɫ
        PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);//ʵ������ɫ����ԴҪ����Resource�ļ���
        
    }
}
