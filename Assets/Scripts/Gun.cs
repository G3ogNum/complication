using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//武器基础脚本
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
        bulletObj.GetComponent<Rigidbody>().AddForce(transform.forward * 500, ForceMode.Impulse);//子弹飞的快些，让其自适应枪口和摄像机偏移值的位置

        GameObject casingObj = Instantiate(casingPrefab);
        casingObj.transform.position = casingTf.transform.position;
    }

}
