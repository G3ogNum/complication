using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceObjectToCamera : MonoBehaviour
{
    RectTransform RT;
    float distance = 16f;
    float scale = 1f;

    public GameObject nameSpace;
    public GameObject stateCanvas;

    float originalY;

    private void Start()
    {
        RT = GetComponent<RectTransform>();
        originalY = transform.position.y;
    }
    // Update is called once per frame
    void Update()
    {

        UpdateUI();
    }

    void UpdateUI()
    {
        if (Camera.main != null)
        {
            // transform.rotation = Camera.main.transform.root.transform.rotation;
            /*Debug.LogWarning(RT.rotation);
            transform.LookAt(new Vector3(0, Camera.main.transform.root.eulerAngles.y, 0));
            new Vector3(0, Camera.main.transform.root.eulerAngles.y, 0);*/
            //RT.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, RT.eulerAngles.y, Camera.main.transform.eulerAngles.z);

            RT.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.root.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

            //Debug.LogWarning((transform.position - Camera.main.transform.position).magnitude);

            distance = (transform.position - Camera.main.transform.position).magnitude;

            if (distance > 30)
            {
                nameSpace.SetActive(false);
                stateCanvas.SetActive(false);
            }
            else
            {
                nameSpace.SetActive(true);
                stateCanvas.SetActive(true);
            }

            scale = distance / 16;

            RT.localScale = new Vector3(scale, scale, scale);
            
            RT.transform.position = new Vector3(RT.transform.position.x, originalY + (distance - 16) / 20, RT.transform.position.z);
        }
    }
}
