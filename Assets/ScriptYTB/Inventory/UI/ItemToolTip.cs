using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemInfo;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetUpToolTip(ItemData_SO item)
    {
        itemName.text = item.itemName;
        itemInfo.text = item.description;
    }

    private void Update()
    {
        UpdatePosition();
    }
    private void OnEnable()
    {
        UpdatePosition();
    }
    void UpdatePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;


        if (mousePosition.y < height)
        {
            rectTransform.position = mousePosition + Vector3.up * height * 0.6f;
        }
        else if (Screen.width-mousePosition.x<width)
        {
            rectTransform.position = mousePosition + Vector3.left * width * 0.6f;
        }
        else
        {
            rectTransform.position = mousePosition + Vector3.right * width * 0.6f;
        }
    }
}
