using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using objectPoolController;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (itemData.itemType)
            {
                case ItemType.Equipment:
                    //TODO: put this item into player's bag and refresh the bagUI
                    other.GetComponent<PlayerRogue>().InventoryManager.
                        inventoryData.AddItem(itemData, itemData.itemAmount);
                    other.GetComponent<PlayerRogue>().InventoryManager.inventoryUI.RefreshUI();
                    //equip the item
                    other.GetComponent<PlayerRogue>().EquipItem(itemData);
                    ObjectPoolManager.ReturnObjectToPool(gameObject);
                    break;


                case ItemType.Buff:
                    //TODO: put this item into player's bag and refresh the bagUI
                    other.GetComponent<PlayerRogue>().InventoryManager.
                        buffData.AddItem(itemData, itemData.itemAmount);
                    other.GetComponent<PlayerRogue>().InventoryManager.BuffUI.RefreshUI();
                    //equip the item
                    //other.GetComponent<PlayerRogue>().EquipItem(itemData);
                    ObjectPoolManager.ReturnObjectToPool(gameObject);
                    break;


                case ItemType.UsableItem:
                    //TODO: put this item into player's bag and refresh the bagUI
                    other.GetComponent<PlayerRogue>().InventoryManager.
                        actionData.AddItem(itemData, itemData.itemAmount);
                    other.GetComponent<PlayerRogue>().InventoryManager.actionBarUI.RefreshUI();
                    //equip the item
                    //other.GetComponent<PlayerRogue>().EquipItem(itemData);
                    ObjectPoolManager.ReturnObjectToPool(gameObject);
                    break;

            }
            
        }
    }
}
