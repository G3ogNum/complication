using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{

    public Transform cameraPosition;


    // Update is called once per frame

    private void Update()
    {
        transform.position = cameraPosition.position;
    }
    private void LateUpdate()
    {
        
    }
    private void FixedUpdate()
    {
       
    }
}
