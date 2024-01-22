using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnShieldRipples : MonoBehaviour
{
    public GameObject shieldRipples;  

    private VisualEffect shieldRipplesVFX;




    /*public void getHurt(Transform bullet)
    {
        //Debug.Log("GetHurt");
        var ripples = Instantiate(shieldRipples, transform) as GameObject;
        shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
        shieldRipplesVFX.SetVector3("SphereCenter", bullet.position);

        Destroy(ripples, 2);
    }*/

    private void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag == "Bullet")
        {
            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
            shieldRipplesVFX.SetVector3("SphereCenter", co.transform.position);

            Destroy(ripples, 2);
        }
    }

    
}
