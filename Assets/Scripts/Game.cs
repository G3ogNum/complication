using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game : MonoBehaviour
{

    public static UIManager uiManager;//TODO:ʹ��static��ԭ��

    public static bool isLoaded = false;

    private void Awake()
    {
        if (isLoaded == true)
        {
            Destroy(gameObject);
        }
        else
        {
            isLoaded = true;
            DontDestroyOnLoad(gameObject);//��ת������ǰ��Ϸ���岻ɾ��
            uiManager = new UIManager();
            uiManager.Init();

            //���÷��� ������ϢƵ�� �����ӳ�
            PhotonNetwork.SendRate = 50;
            PhotonNetwork.SerializationRate = 50;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //��ʾ��½����
        uiManager.ShowUI<LoginUI>("LoginUI");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
