using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using objectPoolController;

public class ReturnParticlesToPool : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
