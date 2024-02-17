using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amount;


    public InventoryData_SO bag { get; set; }
    public int Index { get; set; } = -1;

    public void SetUpItemUI(ItemData_SO item,int itemAmount)
    {
        if (itemAmount == 0)
        {
            bag.items[Index].itemData = null;
            icon.gameObject.SetActive(false);
            return;
        }

        if (item != null)
        {
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);

        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }


    public ItemData_SO GetItem()
    {
        return bag.items[Index].itemData;
    }
}
