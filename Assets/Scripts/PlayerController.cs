using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//��ɫ������
public class PlayerController : MonoBehaviourPun,IPunObservable
{
    //���
    public Animator ani;
    public Rigidbody body;
    public Transform camTf;//����������

    //��ֵ
    public int CurHp = 10;
    public int MaxHp = 10;
    public float MoveSpeed = 3.5f;

    public float H;//ˮƽֵ
    public float V;//��ֱֵ
    public Vector3 dir;//�ƶ�����

    public Vector3 offset;//��������ɫ֮���ƫ��ֵ

    public float Mouse_X;//���ƫ��ֵ
    public float Mouse_Y;
    public float scroll;//������ֵ
    public float Angle_X;//X�����ת�Ƕ�
    public float Angle_Y;//Y�����ת�Ƕ�


    public Quaternion camRotation;//�������ת����Ԫ��

    public Gun gun;

    public AudioClip reloadClip;
    public AudioClip shootClip;

    public bool isDie = false;

    public Vector3 currentPos;
    public Quaternion currentRotation;


    Transform pointTf;//�����

    void Start()
    {
        pointTf = GameObject.Find("Point").transform;
        Angle_X = transform.eulerAngles.x;
        Angle_Y = transform.eulerAngles.y;
        ani = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        gun = GetComponentInChildren<Gun>();
        camTf = Camera.main.transform;

        currentPos = transform.position;
        currentRotation = transform.rotation;

        if (photonView.IsMine)
        {
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp, MaxHp);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //�ж��ǲ��Ǳ������ ֻ�ܲ���������ɫ
        if (photonView.IsMine)
        {
            if (isDie == true)
            {
                return;
            }
            UpdatePosition();
            UpdateRotation();
            InputCtrl();
        }
        else
        {
            UpdateLogic();
        }
    }


    //������ɫ���·��͹��������ݣ�λ�� ��ת��
    public void UpdateLogic()
    {
        transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * MoveSpeed*10);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, Time.deltaTime * 500);
    }

    private void LateUpdate()
    {
        ani.SetFloat("Horizontal", H);
        ani.SetFloat("Vertical", V);
        ani.SetBool("isDie", isDie);
    }

    //����λ��1
    public void UpdatePosition()
    {
        H = Input.GetAxisRaw("Horizontal");
        V = Input.GetAxisRaw("Vertical");
        dir = camTf.forward * V + camTf.right * H;
        body.MovePosition(transform.position + dir * Time.deltaTime * MoveSpeed);
    }

    //������ת��ͬʱ�����������λ�õ���ת��
    public void UpdateRotation()
    {
        Mouse_X = Input.GetAxisRaw("Mouse X");
        Mouse_Y = Input.GetAxisRaw("Mouse Y");
        scroll = Input.GetAxis("Mouse ScrollWheel");

        Angle_X = Angle_X - Mouse_Y;
        Angle_Y = Angle_Y + Mouse_X;

        Angle_X = ClampAngle(Angle_X,-80, 80);
        Angle_Y = ClampAngle(Angle_Y, -360, 360);
        //���ƽǶ���-360��360֮��

        camRotation = Quaternion.Euler(Angle_X, Angle_Y, 0);

        camTf.rotation = camRotation;

        offset.z += scroll;

        camTf.position = transform.position + camTf.rotation*offset;

        transform.eulerAngles = new Vector3(0, camTf.eulerAngles.y, 0);
    }

    public void InputCtrl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //�ж��ӵ�����
            if (gun.BulletCount > 0)
            {
                //������ڲ�������ӵ��Ķ������ܿ�ǹ
                if (ani.GetCurrentAnimatorStateInfo(1).IsName("Reload"))
                {
                    return;
                }


                gun.BulletCount--;
                Game.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);

                //���ſ��𶯻�
                ani.Play("Fire",1,0);

                StopAllCoroutines();
                StartCoroutine(AttackCo());
            }
            
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //����ӵ�
            AudioSource.PlayClipAtPoint(reloadClip, transform.position);//��������ӵ�������
            ani.Play("Reload");
            Invoke("UpdateBulletCount", 3);//�����ʱ��Ӧ�úͻ�������ʱ�䱣��һ��
            
        }

    }

    public void UpdateBulletCount()
    {
        gun.BulletCount = 10;
        Game.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);
    }

    IEnumerator AttackCo()
    {
        //�ӳ�0.1s�ٷ���
        yield return new WaitForSeconds(0.1f);

        //���������Ч
        AudioSource.PlayClipAtPoint(shootClip, transform.position);

        //���߼�⣬������ĵ㷢������
        Ray ray=Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, Input.mousePosition.z));
        //���߿��Ըĳ���ǹ��λ��Ϊ��ʼ�� ���ͣ����������䵽�����ɫ


        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, 1000, LayerMask.GetMask("Player")))
        {
            Debug.Log("�䵽��ɫ");
            hit.transform.GetComponent<PlayerController>().GetHit();
        }


        photonView.RPC("AttackRpc", RpcTarget.All);//�������ִ��AttackRpc����

        
    }

    [PunRPC]
    public void AttackRpc()
    {
        //�����ӵ�
        gun.Attack();
    }


    //����
    public void GetHit()
    {
        if (isDie == true)
        {
            return;
        }

        //ͬ�����н�ɫ����
        photonView.RPC("GetHitRPC", RpcTarget.All);

    }

    [PunRPC]
    public void GetHitRPC()
    {
        CurHp -= 1;//��Ѫ
        if (CurHp <= 0)
        {
            CurHp = 0;
            isDie = true; 
        }
        if (photonView.IsMine)
        {


            Game.uiManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp, MaxHp);
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateBlood();

            if (CurHp == 0)
            {
                Invoke("gameOver", 3);//3�����ʾʧ�ܽ���

            }
        }
    }

    private void gameOver()
    {
        //��ʾ���
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //��ʾʧ�ܽ���
        Game.uiManager.ShowUI<LossUI>("LossUI").onClickCallBack = OnReset ;
    }

    //����
    public void OnReset()
    {
        //�������
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        

        Vector3 pos = pointTf.GetChild(Random.Range(0, pointTf.childCount)).position;

        transform.position = pos;

        photonView.RPC("OnResetRPC", RpcTarget.All);
    }

    [PunRPC]
    public void OnResetRPC()
    {
        isDie = false;
        CurHp = MaxHp; 
        if (photonView.IsMine)
        {
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp, MaxHp);
        }
    }

    public float ClampAngle(float val,float min,float max)
    {
        if (val > 360)
        {
            val -= 360;
        }
        if (val < -360)
        {
            val += 360;
        }
        return Mathf.Clamp(val, min, max);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (ani != null)
        {
            Vector3 angle = ani.GetBoneTransform(HumanBodyBones.Chest).localEulerAngles;
            angle.x = Angle_X;
            ani.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Euler(angle));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //��������
            stream.SendNext(H);
            stream.SendNext(V);
            stream.SendNext(Angle_X);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            //��������
            H = (float)stream.ReceiveNext();
            V = (float)stream.ReceiveNext();
            Angle_X = (float)stream.ReceiveNext();
            currentPos = (Vector3)stream.ReceiveNext();
            currentRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
