using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponSwitcher : MonoBehaviour
{
    public PhotonView playerSetupView;

    public int selectedWeapon = 0;

    public Animation _animation;

    public Animator _animator;

    public Transform TPweaponHolder;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedWeapon = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedWeapon = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedWeapon = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectedWeapon = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            selectedWeapon = 8;
        }

        
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
            selectedWeapon+=1;
            selectedWeapon %= transform.childCount;
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
            selectedWeapon-=1;
            selectedWeapon += transform.childCount ;
            selectedWeapon %= transform.childCount ;
        }

        if (previousSelectedWeapon != selectedWeapon&& selectedWeapon < transform.childCount)
        {
            SelectWeapon();
        }
    }

    public void SelectWeapon()
    {

        playerSetupView.RPC("SetTPWeapon", RpcTarget.AllBuffered, selectedWeapon);


        foreach (Transform _weapon in TPweaponHolder)
        {
            _weapon.gameObject.SetActive(false);
        }

        int i = 0;
        foreach(Transform _weapon in transform)
        {
            if (i == selectedWeapon)
            {
                _weapon.gameObject.SetActive(true);
                //_animation = _weapon.GetComponent<Animation>();
                //_animation.Stop();
                //_animation.Play("Draw");

                _animator= _weapon.GetComponent<Animator>();
                
                _animator.SetTrigger("Draw");
            }
            else
            {
                //_animation = _weapon.GetComponent<Animation>();
                //_animation.Stop();
                _weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void ChangeWeaponControllerActive()
    {
        transform.GetChild(selectedWeapon).GetComponent<Sway>().enabled
        = !transform.GetChild(selectedWeapon).GetComponent<Sway>().enabled;

        transform.GetChild(selectedWeapon).GetComponent<Weapon>().enabled
        = !transform.GetChild(selectedWeapon).GetComponent<Weapon>().enabled;

        /* foreach (Transform item in GetComponentInChildren<Transform>())
         {
             item.GetComponent<Sway>().enabled = !item.GetComponent<Sway>().enabled;
         }*/
    }

}
