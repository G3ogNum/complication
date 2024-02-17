using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using objectPoolController;

public class Grenade : MonoBehaviour
{
    public GameObject Explosion;

    float timer = 2.0f;
    
    private void OnEnable()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        StartCoroutine(ExplosionTimer());
       
    }

    IEnumerator ExplosionTimer()
    {
        timer = 2.0f;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }


        ObjectPoolManager.SpawnObject(Explosion, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

        ObjectPoolManager.ReturnObjectToPool(gameObject);

        yield return null;
    }
}
