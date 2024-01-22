using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;

public class Weapon : MonoBehaviour
{

    public Image ammoCircle;

    public int damage=5;

    public float Elementdamage = 0.2f;
    public int ElementType=0;

    public Camera camera;

    public float fireRate=10;

    [Header("VFX")]
    public GameObject hitVFX;

    private float nextFire;

    [Header("Ammo")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;

    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    [Header("Animation")]
    public Animator animator;
    public Animation animation;
    public AnimationClip reload;
    public TwoBoneIKConstraint constraint;

    [Header("Recoil Settings")]
    /*[Range(0,1)]
    public float recoilPercent = 0.3f;*/
    [Range(0, 2)]
    public float recoverPercent = 0.7f;

    [Space]
    public float recoilUp = .02f;

    public float recoilBack = .08f;


    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;

    private float recoilLength;
    private float recoverLength;


    private bool recoiling;
    public bool recovering;

    [Header("Score Calcular")]
    public PlayerSetup ps;
    public GameObject calcular;

    public void SetAmmo()
    {
        constraint.weight = 1f;
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        ammoCircle.fillAmount = (float)ammo / magAmmo;
    }

    private void Start()
    {
        ps = transform.root.GetComponent<PlayerSetup>();
        calcular = GameObject.Find("ScoreCalcular");

        animator = GetComponent<Animator>();

        SetAmmo();

        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
    }
    private void OnEnable()
    {
        
        SetAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        if (nextFire > 0)
            nextFire -= Time.deltaTime;


        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 &&
            animator.GetCurrentAnimatorStateInfo(1).IsName("Aimming.Draw")==false&&
            animator.GetCurrentAnimatorStateInfo(1).IsName("Aimming.Reloading") == false)
        {
            nextFire = 1 / fireRate;

            ammo--;


            
            SetAmmo();

            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && 
            animator.GetCurrentAnimatorStateInfo(1).IsName("Aimming.Draw") == false &&
            animator.GetCurrentAnimatorStateInfo(1).IsName("Aimming.Reloading") == false &&
            ammo < magAmmo && mag > 0)
        {
            Reload();
        }

        if (recoiling)
        {
            Recoil();
        }


        if (recovering)
        {
            Recovering();
        }
    }


    void Reload()
    {
        animator.SetTrigger("Reload");
        constraint.weight = 0;
        //animation.Play(reload.name);
        //Debug.Log(reload.name);
        /*mag--;

        ammo = magAmmo;


        SetAmmo();*/
    }
    void UpdateAmmo()
    {

        
        Debug.Log("ammo Updated");
        mag--;

        ammo = magAmmo;


        SetAmmo();
        constraint.weight = 0.9f;
    }
    void Fire()
    {
        recoiling = true;
        recovering = false;

        Debug.LogWarning("Fire");

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;

        if(Physics.Raycast(ray.origin,ray.direction,out hit, 100f))
        {
           // Debug.Log(hit.transform.position);
            // PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);//���Ի��ɶ���ز���
            if (hit.transform.gameObject.GetComponent<Health>())
            {
               // hit.transform.gameObject.GetComponent<Health>().shieldEffect.GetComponent<SpawnShieldRipples>().getHurt(hit.transform);
                

                if (damage >= hit.transform.gameObject.GetComponent<Health>().health)
                {
                    Debug.Log("Kill");
                    //kill

                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                    if (ps.battleSide == 0)
                    {
                        
                        calcular.GetComponent<PhotonView>().RPC("TscorePlus", RpcTarget.AllBuffered);
                        //ps.Tscore.text = calcular.GetComponent<ScoreCalcular>().Tscore.ToString() ;

                    }
                    else
                    {
                        
                        calcular.GetComponent<PhotonView>().RPC("CTscorePlus", RpcTarget.AllBuffered);
                        //ps.CTscore.text = calcular.GetComponent<ScoreCalcular>().CTscore.ToString();

                    }




                    PhotonNetwork.LocalPlayer.AddScore(100);
                }
                else
                    PhotonNetwork.LocalPlayer.AddScore(damage);

                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeElement", RpcTarget.All, Elementdamage, ElementType);
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, (float)damage);
            }
        }
    }


    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);

        //Debug.Log(finalPosition);
        transform.localPosition = 
            Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity,recoilLength);


        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    void Recovering()
    {
        Vector3 finalPosition = originalPosition;


        transform.localPosition = 
            Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);


        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
