using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class OverLoad : MonoBehaviour
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
                PhotonNetwork.LocalPlayer.AddScore(10);

            other.transform.gameObject.GetComponent<Health>().TakeDamage(10);


        }
    }
}
