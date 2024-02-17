using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public class DragItem
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }
    //TODO:add model finally to save data
    //[Header("Inventory Data")]
    public InventoryData_SO inventoryData;

    public InventoryData_SO buffData;

    public InventoryData_SO actionData;

    [Header("Containers")]
    public ContainerUI inventoryUI;
    public ContainerUI BuffUI;
    public ContainerUI actionBarUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;

    public DragItem currentDrag;

    [Header("Tool Tip")]
    public ItemToolTip itemToolTip;

    private void Awake()
    {
        inventoryData = new InventoryData_SO();
        buffData = new InventoryData_SO();
        actionData = new InventoryData_SO();

        for (int i=0;i<7;i++)
        inventoryData.items.Add(new InventoryItem());

        for(int i = 0; i < 3; i++)
        {
            buffData.items.Add(new InventoryItem());
            actionData.items.Add(new InventoryItem());
        }
        inventoryUI.RefreshUI();
        BuffUI.RefreshUI();
        actionBarUI.RefreshUI();

    }

    #region check if mousePoint is in range of a slot
    public bool CheckInSlotUI(Vector3 position,SlotType type)
    {
        ContainerUI ui;
        switch (type)
        {
            case SlotType.Equipment:
                ui = inventoryUI;
                break;
            case SlotType.Buff:
                ui = BuffUI;
                break;
            case SlotType.Bag:
                ui = actionBarUI;
                break;
            default:
                ui = new ContainerUI();
                break;
        }
        //inventoryUI could be swap by actionUI or buffUI
        //inventoryUI could be used as param of this func
        for (int i = 0; i < ui.slotHolders.Length;i++)
        {
            RectTransform t = ui.slotHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    #endregion
    public bool CheckInInventoryUI(Vector3 position, SlotType type)
    {
        RectTransform ui;
        switch (type)
        {
            case SlotType.Equipment:
                ui = inventoryUI.transform.parent.GetComponent<RectTransform>();
                break;
            case SlotType.Buff:
                ui = BuffUI.transform.parent.GetComponent<RectTransform>();
                break;
            case SlotType.Bag:
                ui = actionBarUI.GetComponent<RectTransform>();
                break;
            default:
                ui = new RectTransform();
                break;
        }
        RectTransform t = ui.transform as RectTransform;
        if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
        {
            return true;
        }
        return false;
    }
}
