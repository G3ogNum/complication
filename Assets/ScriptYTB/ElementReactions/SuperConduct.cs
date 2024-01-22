using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class SuperConduct : MonoBehaviour
{
    //will be called twice
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.GetComponent<Health>())
        {
            if (10 >= other.transform.gameObject.GetComponent<Health>().health)
            {
                Debug.Log("Kill");
                //kill

                RoomManager.instance.kills++;
                RoomManager.instance.SetHashes();

                PhotonNetwork.LocalPlayer.AddScore(100);
            }
            else
                PhotonNetwork.LocalPlayer.AddScore(5);

            other.transform.gameObject.GetComponent<Health>().TakeElement(0.2f, 2);
            
            other.transform.gameObject.GetComponent<Health>().TakeDamage(5);

            other.transform.gameObject.GetComponent<Health>().StartLowSpeed();
           
        }
    }
    
}
