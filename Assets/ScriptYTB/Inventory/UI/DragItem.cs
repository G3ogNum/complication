using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemUI currentItemUI;

    public SlotHolder currentSlotHolder;

    public SlotHolder targetSlotHolder;
    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();

        currentSlotHolder = GetComponentInParent<SlotHolder>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.root.GetComponent<PlayerRogue>().InventoryManager.currentDrag = new InventoryManager.DragItem();
        transform.root.GetComponent<PlayerRogue>().InventoryManager.currentDrag.originalHolder = GetComponentInParent<SlotHolder>();
        transform.root.GetComponent<PlayerRogue>().InventoryManager.currentDrag.originalParent=(RectTransform)transform.parent;
        //record original data
        transform.SetParent(transform.root.GetComponent<PlayerRogue>().InventoryManager.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //follow the position of mouse
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //put down item
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if(!transform.root.GetComponent<PlayerRogue>().InventoryManager.CheckInInventoryUI(eventData.position, currentSlotHolder.slotType))
            {
                if (currentSlotHolder.slotType == SlotType.Equipment || currentSlotHolder.slotType == SlotType.Buff)
                {
                    transform.root.GetComponent<PlayerRogue>().UnequipItem(currentItemUI.bag.items[currentItemUI.Index].itemData);
                }

                currentItemUI.bag.items[currentItemUI.Index].itemData = null;
                currentItemUI.bag.items[currentItemUI.Index].amount = 0;

                currentSlotHolder.UpdateItem();
            }
            if (transform.root.GetComponent<PlayerRogue>().InventoryManager.CheckInSlotUI(eventData.position, currentSlotHolder.slotType))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                    targetSlotHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                else
                    targetSlotHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();

                SwapItem();

                currentSlotHolder.UpdateItem();
                targetSlotHolder.UpdateItem();
            }
            transform.SetParent(transform.root.GetComponent<PlayerRogue>().InventoryManager.currentDrag.originalParent);
            
            RectTransform t = transform as RectTransform;

            t.offsetMax = -Vector2.one * 0;
            t.offsetMin = Vector2.one * 0;
        }
    }
    public void SwapItem()
    {
        var targetItem = targetSlotHolder.itemUI.bag.items[targetSlotHolder.itemUI.Index];
        var tempItem = currentSlotHolder.itemUI.bag.items[currentSlotHolder.itemUI.Index];

        bool isSameItem = tempItem.itemData == targetItem.itemData;

        if (isSameItem && targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        else
        {
            currentSlotHolder.itemUI.bag.items[currentSlotHolder.itemUI.Index] = targetItem;
            targetSlotHolder.itemUI.bag.items[targetSlotHolder.itemUI.Index] = tempItem;
        }
    }
}
