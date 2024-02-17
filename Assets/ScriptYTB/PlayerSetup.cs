using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
public class PlayerSetup : MonoBehaviour
{
    //public Movement movement;

    [Header("player movement")]
    public PlayerMovement pm;
    public Climbing climbing;
    public Sliding sliding;
    public WallRunning wallRunning;

    [Header("player information")]
    public GameObject camera;
    public GameObject StateUI;
    public GameObject Character;
    public string nickname;
    public int battleSide = 0;//T:0,CT:1;


    [Header("player score UI")]
    public TextMeshPro nicknameText;
    public TextMeshProUGUI Tscore;
    public TextMeshProUGUI CTscore;


    [Header("HUD")]
    public GameObject optionCanvas;
    public GameObject gameSettingCanvas;
    public GameObject InventoryBagCanvas;


    [Header("other")]
    public Transform TPweaponHolder;


    private void Start()
    {
        ScoreCalcular.instance.uts += UpdateScore;
        UpdateScore();
    }
    public void IsLocalPlayer()
    {
        //movement.enabled = true;
        pm.enabled = true;
        climbing.enabled = true;
        sliding.enabled = true;
        wallRunning.enabled = true;
        
        Character.layer = 10;
        foreach(Transform item in Character.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = 10;
        }
        camera.SetActive(true);
        StateUI.SetActive(false);


        optionCanvas.SetActive(true);
        gameSettingCanvas.SetActive(true);
        InventoryBagCanvas.SetActive(true);

        optionCanvas.SetActive(false);
        gameSettingCanvas.SetActive(false);
        InventoryBagCanvas.SetActive(false);
        //TPweaponHolder.gameObject.SetActive(false);
    }

    void UpdateScore()
    {
        Tscore.text = ScoreCalcular.instance.Tscore.ToString();
        CTscore.text = ScoreCalcular.instance.CTscore.ToString();
    }

    [PunRPC]
    public void SetTPWeapon(int _weaponIndex)
    {
        foreach (Transform _weapon in TPweaponHolder)
        {
            _weapon.gameObject.SetActive(false);
        }
        //rememberToChangeThis
        TPweaponHolder.GetChild(_weaponIndex).gameObject.SetActive(false);
        //TPweaponHolder.GetChild(_weaponIndex).gameObject.SetActive(true);
    }

    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;

        nicknameText.text = nickname; 
    }
    [PunRPC]
    public void SetSide(int _side)
    {
        battleSide = _side;
    }
    
}
