using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class FightManager : MonoBehaviour
{
    private void Awake()
    {
        //隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //关闭所有界面
        Game.uiManager.CloseAllUI();
        //显示战斗界面
        Game.uiManager.ShowUI<FightUI>("FightUI");

        Transform pointTf = GameObject.Find("Point").transform;

        Vector3 pos = pointTf.GetChild(Random.Range(0, pointTf.childCount)).position;

        //实例化角色
        PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);//实例化角色的资源要放在Resource文件夹
        
    }
}
