using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using objectPoolController;

public class PlayerRogue : MonoBehaviour
{
    public Transform weaponSlot;
    public InventoryManager InventoryManager;
    public DialogueUI dialogueUI;

    private AttackData_SO characterData;
    private void Start()
    {
        characterData = new AttackData_SO();
    }
    public void PutItemIntoInventory(ItemData_SO item)
    {
        //this could be changed to preuse item on character and disable it,
        //make it active when call this func
        /*if(item.itemType==ItemType.Weapon)
        {
            if(item.weaponPrefab!=null)
            ObjectPoolManager.SpawnObject(item.weaponPrefab, weaponSlot);
        }*/
    }


    //this will be changed to local param to suit network game
    public void EquipItem(ItemData_SO item)
    {
        if (item.attackData != null)
        characterData.ApplyWeaponData(item.attackData);

        //this could use BuffData if time is enough
        Health health = GetComponent<Health>();
        health.healthMax = characterData.health;
        health.health += item.attackData.health;
        health.shieldMax = characterData.elementShield;
        if (health.ElementShieldColdTime > 0)
            health.ElementShieldColdTime = 0;
        health.ElementShield += item.attackData.elementShield;
        health.UpdateShield();
        health.UpdateHealth();
    }

    public void UnequipItem(ItemData_SO item)
    {
        if (item.attackData != null)
            characterData.DisapplyWeaponData(item.attackData);

        //this could use BuffData if time is enough
        Health health = GetComponent<Health>();
        health.healthMax = characterData.health;
        health.health -= item.attackData.health;
        health.shieldMax = characterData.elementShield;
        if (health.ElementShieldColdTime > 0)
            health.ElementShieldColdTime = 0;
        health.ElementShield -= item.attackData.elementShield;
        health.UpdateShield();
        health.UpdateHealth();
    }

   


    public void UseItem(ItemData_SO item)
    {
        Health health = GetComponent<Health>();
        if (health.health + item.useableItemData.healthCover <= health.healthMax)
            health.health += item.useableItemData.healthCover;
        else
            health.health = health.healthMax;

        if (health.ElementShield + item.useableItemData.shieldCover <= health.shieldMax)
            health.ElementShield += item.useableItemData.shieldCover;
        else
            health.ElementShield = health.shieldMax;

        if (health.ElementShieldColdTime > 0)
            health.ElementShieldColdTime = 0;
        health.UpdateShield();
        health.UpdateHealth();
    }
}
