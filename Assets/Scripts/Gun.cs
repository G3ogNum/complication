using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���������ű�
public class Gun : MonoBehaviour
{
    public int BulletCount = 10;

    public GameObject bulletPrefab;
    public GameObject casingPrefab;

    public Transform bulletTf;
    public Transform casingTf;

    void Start()
    {
        
    }

    public void Attack()
    {
        GameObject bulletObj = Instantiate(bulletPrefab);
        bulletObj.transform.position = bulletTf.transform.position;
        bulletObj.GetComponent<Rigidbody>().AddForce(transform.forward * 500, ForceMode.Impulse);//�ӵ��ɵĿ�Щ����������Ӧǹ�ں������ƫ��ֵ��λ��

        GameObject casingObj = Instantiate(casingPrefab);
        casingObj.transform.position = casingTf.transform.position;
    }

}
