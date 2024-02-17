using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Equipment,Buff,UsableItem,Weapon }

[CreateAssetMenu(fileName ="New Item",menuName ="Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;

    public string itemName;

    public Sprite itemIcon;

    public int itemAmount;

    public bool stackable;

    [TextArea]
    public string description ="";

    [Header("Weapon")]
    public GameObject weaponPrefab;

    public AttackData_SO attackData;

    [Header("UseableItem")]
    public UseableItemData_SO useableItemData;
}
