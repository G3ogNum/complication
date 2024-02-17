using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStatesController : MonoBehaviour
{
    
    private void OnEnable()
    {
        Invoke("SetActiveDisable", 1);
    }
    void SetActiveDisable()
    {
        gameObject.SetActive(false);
    }
}
