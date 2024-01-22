using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;


public class LeaderBoard : MonoBehaviour
{
    public GameObject playersHolder;

    [Header("Options")] public float refreshRate = 1f;


    [Header("UI")] public GameObject[] slots;

    [Space] 
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI[] KDTexts;


    private void Start()
    {
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    public void Refresh()
    {
        foreach(var slot in slots)
        {
            slot.SetActive(false);
        }


        var sortefPlayerList = 
            (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        int i = 0;
        foreach(var player in sortefPlayerList)
        {
            slots[i].SetActive(true);

            if (player.NickName == "")
                player.NickName = "unnamed";


            nameTexts[i].text = player.NickName;
            scoreTexts[i].text = player.GetScore().ToString();

            if (player.CustomProperties["kills"] != null)
            {
                KDTexts[i].text = player.CustomProperties["kills"] + "/" + player.CustomProperties["deaths"];
            }
            else
            {
                KDTexts[i].text = "0/0";
            }


            i++;
        }


        
    }
    public void Update()
    {
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}
