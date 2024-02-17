using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { Bag,Weapon,Equipment,Buff}
public class SlotHolder : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{

    public SlotType slotType;

    public ItemUI itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if(itemUI.GetItem()!=null)
            if (itemUI.bag.items[itemUI.Index].amount > 0 && itemUI.GetItem().itemType == ItemType.UsableItem)
            {
                transform.root.GetComponent<PlayerRogue>().UseItem(itemUI.GetItem());
                itemUI.bag.items[itemUI.Index].amount-=1;
            }
        UpdateItem();
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.Equipment:
                itemUI.bag = transform.parent.parent.parent
                    .GetComponent<InventoryManager>().inventoryData;
                break;
            case SlotType.Buff:
                itemUI.bag = transform.parent.parent.parent
                    .GetComponent<InventoryManager>().buffData;
                break;
            case SlotType.Bag:
                itemUI.bag = transform.parent.parent
                    .GetComponent<InventoryManager>().actionData;
                break;
            case SlotType.Weapon:
                break;
        }

        var item = itemUI.bag.items[itemUI.Index];
        itemUI.SetUpItemUI(item.itemData, item.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())
        {
            transform.root.GetComponent<PlayerRogue>().InventoryManager.itemToolTip.SetUpToolTip(itemUI.GetItem());
            transform.root.GetComponent<PlayerRogue>().InventoryManager.itemToolTip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        transform.root.GetComponent<PlayerRogue>().InventoryManager.itemToolTip.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transform.root.GetComponent<PlayerRogue>().InventoryManager.itemToolTip.gameObject.SetActive(false);

    }
}
