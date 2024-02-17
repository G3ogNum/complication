using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using objectPoolController;
using System;

public class Health : MonoBehaviour
{
    public float health;
    public float ElementShield;

    public float healthMax = 100f;
    public float shieldMax = 100f;

    [Header("Shield Components")]
    public GameObject shieldEffect;
    public GameObject shieldBrokenEffect;

    public bool isLocalPlayer;

    public Image[] Elements;
    public Image[] ElementSelfs;


    public Weapon[] weapons;
    public WeaponSwitcher weaponSwitcher;

    public float[] ElementsColdTime;
    public float ElementShieldColdTime = 5f;

    delegate void RespawnDelegate();
    event RespawnDelegate Respawn;

    [Header("VFX")]
    //elementType:0-Pyro,1-Hydro,2-Cryo,3-Electro,4-Anemo,5-Geo,6-Dendro
    public GameObject[] PyroActivatedVFXs;
    //ElementalReactions:0-Vaporize(Water&Fire),1-Melt(FireIce),2-Overload(FireElectro),
    //      3-Superconduct(ElectroIce),4-ElectroCharge(WaterElectro),5-Frozen(WaterIce),
    //      6-Swirl(Wind+anotherEspectGeo&Dendro
    public GameObject[] ElementalReactions;

    public GameObject[] SwirlElements;

    [Header("UI")]
    //health
    public TextMeshProUGUI healthText;
    public RectTransform healthBar;
    public float originalHealthBarSize;
    //shield
    public TextMeshProUGUI shieldText;
    public RectTransform shieldBar;
    public float originalShieldBarsize;
    //screen damage
    public Image FlashImage;
    public Image HurtImage;

    [Header("Timers")]
    public float hurtTimer = 0.1f;
    public float hurtRecoverTimer = 3f;
    public float LowSpeedTimer = 3f;

    [Header("Frozen")]
    public float frozenTimer = 2f;
    public int shakeCount = 0;
    public CameraShakeYTB camShake;
    public GameObject FrozenImage;
    public RectTransform FrozenBar;
    public float originalFrozenBarSize;
    bool isFrozen;

    private void Start()
    {
        ElementsColdTime = new float[] { 0, 0, 0, 0, 0, 0, 0 };
        originalHealthBarSize = healthBar.sizeDelta.x;
        originalShieldBarsize = shieldBar.sizeDelta.x;
        originalFrozenBarSize = FrozenBar.sizeDelta.x;
        weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();

        Respawn += UpdatePosition;
        Respawn += RemakeStates;
        Respawn += RemakeWeaponSwitcher;
        Respawn += UpdateScore;
        Respawn += ClearAllElements;
    }

    private void Update()
    {
        ElementShieldColdDown();

        ElementColdDown();

        ScreenHurtColdDown();

        //UpdateFrozen();
        //Debug.Log(ElementCount);
        //healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / 100f, healthBar.sizeDelta.y);
    }

    void ElementColdDown()
    {
        for (int i = 0; i < 7; i++)
        {
            if (ElementsColdTime[i] > 0)
            {
                ElementsColdTime[i] -= Time.deltaTime;

            }
            else
            {
                if (Elements[i].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                {


                    Elements[i].transform.GetChild(0).gameObject.SetActive(true);
                    Elements[i].transform.GetChild(3).gameObject.SetActive(false);

                    Elements[i].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
                    Elements[i].gameObject.SetActive(false);

                    #region localself
                    ElementSelfs[i].transform.GetChild(0).gameObject.SetActive(true);
                    ElementSelfs[i].transform.GetChild(3).gameObject.SetActive(false);

                    ElementSelfs[i].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
                    ElementSelfs[i].gameObject.SetActive(false);
                    #endregion
                }
                else if (Elements[i].transform.GetChild(0).GetComponent<Image>().fillAmount == 0)
                {
                    if (Elements[i].gameObject.activeSelf == true)
                    {
                        Elements[i].gameObject.SetActive(false);


                        #region localself
                        ElementSelfs[i].gameObject.SetActive(false);
                        #endregion
                    }
                }
                else
                {
                    Elements[i].transform.GetChild(0).GetComponent<Image>().fillAmount -= Time.deltaTime;//������Գ���Ԫ��˥����صĲ���

                    #region localself
                    ElementSelfs[i].transform.GetChild(0).GetComponent<Image>().fillAmount -= Time.deltaTime;//������Գ���Ԫ��˥����صĲ���
                    #endregion
                }
            }
        }
    }

    //elementType:0-Pyro,1-Hydro,2-Cryo,3-Electro,4-Anemo,5-Geo,6-Dendro
    [PunRPC]
    public void TakeElement(float _eDamage, int _eType)
    {
        ElementsColdTime[_eType] = 3f;

        //Debug.Log(Elements[_eType].gameObject.activeSelf);
        if (Elements[_eType].gameObject.activeSelf == false)
        {

            //the buff add at last will appear at the last of the ui panel
            Elements[_eType].transform.SetAsLastSibling();


            Elements[_eType].gameObject.SetActive(true);
            Elements[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
            Elements[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount += _eDamage;


            #region localself
            ElementSelfs[_eType].transform.SetAsLastSibling();

            ElementSelfs[_eType].gameObject.SetActive(true);
            ElementSelfs[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
            ElementSelfs[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount += _eDamage;
            #endregion
        }
        else
        {
            if (Elements[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount < 1 &&
                Elements[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount + _eDamage >= 1)
            {
                Elements[_eType].transform.GetChild(3).gameObject.SetActive(true);
                Elements[_eType].transform.GetChild(0).gameObject.SetActive(false);


                Elements[_eType].transform.GetChild(2).gameObject.SetActive(true);
                ObjectPoolManager.SpawnObject(PyroActivatedVFXs[_eType], transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
                //PhotonNetwork.Instantiate(PyroActivatedVFXs[_eType].name, transform.position, transform.rotation);
                TakeDamage(10);

                #region localself
                ElementSelfs[_eType].transform.GetChild(3).gameObject.SetActive(true);
                ElementSelfs[_eType].transform.GetChild(0).gameObject.SetActive(false);
                ElementSelfs[_eType].transform.GetChild(2).gameObject.SetActive(true);
                #endregion
            }
            Elements[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount += _eDamage;
            #region localself
            ElementSelfs[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount += _eDamage;
            #endregion
        }

        if (Elements[_eType].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
            switch (_eType)
            {
                case 0:
                    if (Elements[1].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Vaporize();
                    }
                    else if (Elements[2].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Melt();
                    }
                    else if (Elements[3].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Overload();
                    }
                    else if (Elements[4].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Swirl(0);
                    }
                    break;
                case 1:
                    if (Elements[0].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Vaporize();
                    }
                    else if (Elements[2].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Frozen();
                    }
                    else if (Elements[3].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_ElectroCharged();
                    }
                    else if (Elements[4].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Swirl(1);
                    }
                    break;
                case 2:
                    if (Elements[0].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Melt();
                    }
                    else if (Elements[1].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Frozen();
                    }
                    else if (Elements[3].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Superconduct();
                    }
                    else if (Elements[4].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Swirl(2);
                    }
                    break;
                case 3:
                    if (Elements[0].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Overload();
                    }
                    else if (Elements[1].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_ElectroCharged();
                    }
                    else if (Elements[2].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Superconduct();
                    }
                    else if (Elements[4].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Swirl(3);
                    }
                    break;
                case 4:
                    if (Elements[0].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Swirl(0);
                    }
                    else if (Elements[1].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Swirl(1);
                    }
                    else if (Elements[2].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Swirl(2);
                    }
                    else if (Elements[3].transform.GetChild(0).GetComponent<Image>().fillAmount == 1)
                    {
                        EReaction_Swirl(3);
                    }
                    break;
                default:
                    break;
            }
    }

    [PunRPC]
    public void TakeDamage(float _damage)
    {

        Debug.LogWarning("Damage : " + _damage);

        ElementShieldColdTime = 5f;

        if (ElementShield - _damage > 0)
        {
            ElementShield -= _damage;

            shieldText.text = ((int)ElementShield).ToString();

            shieldBar.sizeDelta = new Vector2(originalShieldBarsize * ElementShield / shieldMax, shieldBar.sizeDelta.y);
        }
        else
        {
            if (ElementShield > 0)
            {
                _damage -= ElementShield;

                ElementShield = 0;

                shieldText.text = ((int)ElementShield).ToString();

                shieldBar.sizeDelta = new Vector2(originalShieldBarsize * ElementShield / shieldMax, shieldBar.sizeDelta.y);


                shieldEffect.gameObject.SetActive(false);

                shieldBrokenEffect.gameObject.SetActive(true);
            }

            hurtRecoverTimer = 3f;

            health -= _damage;

            healthText.text = ((int)health).ToString();

            healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / healthMax, healthBar.sizeDelta.y);

            StartCoroutine(HurtFlash());

            UpdateHealthUI();
        }


        //Respawn need to be changed by delegate
        if (health <= 0)
        {
            if (isLocalPlayer)
            {
                Respawn();
            }


            //Destroy(gameObject);//use object pool operation instead(has been done)
        }
    }
    #region respawn
    void UpdatePosition()
    {
        transform.position = RoomManager.instance.RespawnPlayer().position;
    }
    
    void RemakeStates()
    {
        /*shieldEffect.gameObject.SetActive(true);
        shieldEffect.GetComponent<Animation>().Play("ShieldGrowing01");*/

        //cant be see but control by myself
        ElementShield = shieldMax;
        shieldText.text = ((int)ElementShield).ToString();
        shieldBar.sizeDelta = new Vector2(originalShieldBarsize * ElementShield / shieldMax, shieldBar.sizeDelta.y);

        //shield timer remake
        GetComponent<PhotonView>().RPC("RemakeShield", RpcTarget.AllBuffered);
        GetComponent<PhotonView>().RPC("ClearAllElements", RpcTarget.AllBuffered);
        ElementShieldColdTime = 0f;
        shieldEffect.gameObject.SetActive(true);
        shieldEffect.GetComponent<Animation>().Play("ShieldGrowing01");

        isFrozen = false;

        //cant be see but control by myself
        health = healthMax;
        healthText.text = ((int)health).ToString();
        healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / healthMax, healthBar.sizeDelta.y);

        //cant be see but control by myself
        UpdateHealthUI();
    }
    [PunRPC]
    void RemakeShield()
    {
        ElementShield = shieldMax;
        health = healthMax;
        ElementShieldColdTime = 0f;
        shieldEffect.gameObject.SetActive(true);
        shieldEffect.GetComponent<Animation>().Play("ShieldGrowing01");
    }
    //cant be see but control by myself
    void RemakeWeaponSwitcher()
    {
        weaponSwitcher.selectedWeapon = 0;
        weaponSwitcher.SelectWeapon();

        foreach (Weapon weapon in weapons)
        {
            weapon.ammo = 30;
            weapon.mag = 5;
            weapon.SetAmmo();
        }
    }
    void UpdateScore()
    {
        RoomManager.instance.deaths++;
        RoomManager.instance.SetHashes();
    }
    [PunRPC]
    void ClearAllElements()
    {
        for(int i=0;i< Elements.Length; i++)
        {
            Elements[i].transform.GetChild(0).gameObject.SetActive(true);
            Elements[i].transform.GetChild(3).gameObject.SetActive(false);
            Elements[i].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
            Elements[i].gameObject.SetActive(false);

            ElementSelfs[i].transform.GetChild(0).gameObject.SetActive(true);
            ElementSelfs[i].transform.GetChild(3).gameObject.SetActive(false);
            ElementSelfs[i].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
            ElementSelfs[i].gameObject.SetActive(false);
        }
    }
    #endregion

    void ClearElements(int ele_1, int ele_2)
    {
        Elements[ele_1].transform.GetChild(0).gameObject.SetActive(true);
        Elements[ele_1].transform.GetChild(3).gameObject.SetActive(false);
        Elements[ele_2].transform.GetChild(0).gameObject.SetActive(true);
        Elements[ele_2].transform.GetChild(3).gameObject.SetActive(false);

        Elements[ele_1].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
        Elements[ele_2].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
        Elements[ele_1].gameObject.SetActive(false);
        Elements[ele_2].gameObject.SetActive(false);

        #region localself
        ElementSelfs[ele_1].transform.GetChild(0).gameObject.SetActive(true);
        ElementSelfs[ele_1].transform.GetChild(3).gameObject.SetActive(false);
        ElementSelfs[ele_2].transform.GetChild(0).gameObject.SetActive(true);
        ElementSelfs[ele_2].transform.GetChild(3).gameObject.SetActive(false);

        ElementSelfs[ele_1].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
        ElementSelfs[ele_2].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
        ElementSelfs[ele_1].gameObject.SetActive(false);
        ElementSelfs[ele_2].gameObject.SetActive(false);
        #endregion
    }

    #region ElementReactions
    void EReaction_Vaporize()
    {
        Debug.Log("���");
        ObjectPoolManager.SpawnObject(ElementalReactions[0], transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        //PhotonNetwork.Instantiate(ElementalReactions[0].name, transform.position, transform.rotation);
        TakeDamage(10);
        ClearElements(0, 1);
    }
    void EReaction_Melt()
    {
        Debug.Log("�ڻ�");
        ObjectPoolManager.SpawnObject(ElementalReactions[1], transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        //PhotonNetwork.Instantiate(ElementalReactions[1].name, transform.position, transform.rotation);
        TakeDamage(10);
        ClearElements(0, 2);
    }
    void EReaction_Frozen()
    {
        Debug.Log("����");
       // ObjectPoolManager.SpawnObject(ElementalReactions[2], transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        //PhotonNetwork.Instantiate(ElementalReactions[2].name, transform.position, transform.rotation);
        TakeDamage(10);
        ClearElements(1, 2);
        StartFrozen();
        //StartCoroutine(Frozen());
    }
    #region Frozen
    void StartFrozen()
    {
        FrozenImage.SetActive(true);
        FrozenBar.sizeDelta = new Vector2(originalFrozenBarSize, FrozenBar.sizeDelta.y);
        isFrozen = true;
        GetComponent<PlayerMovement>().speedBuff = 0;
        shakeCount = 0;
        frozenTimer = 2f;
        StartCoroutine(UpdateFrozen());
    }
    void EndFrozen()
    {
        isFrozen = false;
        GetComponent<PlayerMovement>().speedBuff = 1;
        FrozenImage.SetActive(false);
    }
    IEnumerator UpdateFrozen()
    {
        while(isFrozen && frozenTimer > 0)
        {
            frozenTimer -= Time.deltaTime;
            FrozenBar.sizeDelta = new Vector2(originalFrozenBarSize * frozenTimer / 2, FrozenBar.sizeDelta.y);
            if (Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.Space))
            {
                shakeCount++;
                //shakeScreen Here
                StartCoroutine(camShake.Shake(0.15f, 0.2f));

            }
            if (shakeCount >= 4)
            {
                shakeCount = 0;
                frozenTimer -= 0.4f;
                FrozenBar.sizeDelta = new Vector2(originalFrozenBarSize * frozenTimer / 2, FrozenBar.sizeDelta.y);
            }
            yield return null;
        }

        EndFrozen();
    }
    /*void UpdateFrozen()
    {
        if (isFrozen && frozenTimer > 0)
        {
            frozenTimer -= Time.deltaTime;
            FrozenBar.sizeDelta = new Vector2(originalFrozenBarSize * frozenTimer/2, FrozenBar.sizeDelta.y);
            if (Input.GetKeyDown(KeyCode.W)||
                Input.GetKeyDown(KeyCode.S) || 
                Input.GetKeyDown(KeyCode.A) || 
                Input.GetKeyDown(KeyCode.D) || 
                Input.GetKeyDown(KeyCode.Space))
            {
                shakeCount++;
                //shakeScreen Here
                StartCoroutine(camShake.Shake(0.15f, 0.2f));

            }
            if (shakeCount >= 4)
            {
                shakeCount = 0;
                frozenTimer -= 0.4f;
                FrozenBar.sizeDelta = new Vector2(originalFrozenBarSize * frozenTimer / 2, FrozenBar.sizeDelta.y);
            }
        }
        else
        {
            EndFrozen();
        }
    }*/
    #endregion
    void EReaction_Overload()
    {
        Debug.Log("����");
        ObjectPoolManager.SpawnObject(ElementalReactions[3], transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        //PhotonNetwork.Instantiate(ElementalReactions[3].name, transform.position, transform.rotation);
        TakeDamage(10);
        ClearElements(0, 3);
    }
    void EReaction_Superconduct()
    {
        Debug.Log("����");
        ClearElements(2, 3);
        ObjectPoolManager.SpawnObject(ElementalReactions[4], transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        //PhotonNetwork.Instantiate(ElementalReactions[4].name, transform.position, transform.rotation);
        TakeDamage(10);
        
    }
    #region LowSpeed
    public void StartLowSpeed()
    {
        //�����õ����ز�
        FrozenImage.SetActive(true);
        FrozenBar.sizeDelta = new Vector2(originalFrozenBarSize, FrozenBar.sizeDelta.y);
        LowSpeedTimer = 3f;
        GetComponent<PlayerMovement>().speedBuff = 0.3f;
        StartCoroutine(UpdateLowSpeed());
    }
    IEnumerator UpdateLowSpeed()
    {
        while (LowSpeedTimer > 0)
        {
            LowSpeedTimer -= Time.deltaTime;
            //�����õ����ز�
            FrozenBar.sizeDelta = new Vector2(originalFrozenBarSize * LowSpeedTimer / 2, FrozenBar.sizeDelta.y);
            
            yield return null;
        }

        
        EndLowSpeed();
    }
    private void EndLowSpeed()
    {
        GetComponent<PlayerMovement>().speedBuff = 1;
        FrozenImage.SetActive(false);
    }
    #endregion
    void EReaction_ElectroCharged()
    {
        ClearElements(1, 3);
        
        Debug.Log("�е�");
        //PhotonNetwork.Instantiate(ElementalReactions[5].name, transform.position, transform.rotation);
        StartElectroCharged();
    }
    #region ElectroCharged
    void StartElectroCharged()
    {
        StartCoroutine(UpdateElectroCharged());
    }

    IEnumerator UpdateElectroCharged()
    {

        for(int i = 0; i < 3; i++)
        {
            ObjectPoolManager.SpawnObject(ElementalReactions[5], transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
            TakeDamage(5);
            yield return new WaitForSeconds(1f);
        }
       
    }
    #endregion
    void EReaction_Swirl(int ele)
    {
        Debug.Log("��ɢ");
        ObjectPoolManager.SpawnObject(SwirlElements[ele], transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem)
            .GetComponent<Swirl>().ElementToBeSwirled=ele;
        //PhotonNetwork.Instantiate(ElementalReactions[6].name, transform.position, transform.rotation);
        TakeDamage(10);
        ClearElements(ele, 4);
    }
    #endregion

    IEnumerator HurtFlash()
    {
        FlashImage.enabled = true;
        yield return new WaitForSeconds(hurtTimer);
        FlashImage.enabled = false;
    }

    
    public void UpdateHealthUI()
    {
        Color splatterAlpha = HurtImage.color;
        splatterAlpha.a = 0.5f - (health / (2 * healthMax));
        HurtImage.color = splatterAlpha;
    }

    public void UpdateShield()
    {
        shieldText.text = ((int)ElementShield).ToString();

        shieldBar.sizeDelta = new Vector2(originalShieldBarsize * ElementShield / shieldMax, shieldBar.sizeDelta.y);

    }
    public void UpdateHealth()
    {
        healthText.text = ((int)health).ToString();

        healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / healthMax, healthBar.sizeDelta.y);

        UpdateHealthUI();
    }

    void ScreenHurtColdDown()
    {
        if (hurtRecoverTimer > 0)
        {
            hurtRecoverTimer -= Time.deltaTime;
        }
        else
        {
            if (HurtImage.color.a > 0.3f)
            {
                Color splatterAlpha = HurtImage.color;
                splatterAlpha.a -= Time.deltaTime;
                HurtImage.color = splatterAlpha;
            }
        }
    }
    public void ElementShieldColdDown()
    {
        if (ElementShieldColdTime > 0)
        {
            ElementShieldColdTime -= Time.deltaTime;
        }
        else if(ElementShield<shieldMax)
        {
            if(ElementShield <= 0)
            {
                shieldEffect.gameObject.SetActive(true);
                shieldEffect.GetComponent<Animation>().Play("ShieldGrowing01");
            }
            ElementShield += (Time.deltaTime * 10);//this could mutiply other facts to get the result num

            shieldText.text = ((int)ElementShield).ToString();

            shieldBar.sizeDelta = new Vector2(originalShieldBarsize * ElementShield / shieldMax, shieldBar.sizeDelta.y);

        }
        
    }
}
