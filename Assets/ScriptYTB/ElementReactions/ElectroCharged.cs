using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using objectPoolController;

public class ElectroCharged : MonoBehaviour
{
    public GameObject Electro;
    //Infect others around you
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.GetComponent<Health>())
        {
            if(other.transform.gameObject.GetComponent<Health>().
                Elements[1].gameObject.activeSelf == true && 
                other.transform.gameObject.GetComponent<Health>()
                .Elements[1].transform.GetChild(0).GetComponent<Image>().fillAmount >=0)
            {
                ObjectPoolManager.SpawnObject(Electro, other.transform.position, other.transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
                other.transform.gameObject.GetComponent<Health>().TakeElement(0.1f,3);
                other.transform.gameObject.GetComponent<Health>().TakeDamage(2.5f);
            }
        }
    }
}
