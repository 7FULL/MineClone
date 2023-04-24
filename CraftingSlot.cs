using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IDropHandler, Slot
{
    CraftingManager cf;
    [SerializeField] private int slot;

    private void Awake()
    {
        cf = GetComponentInParent<CraftingManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;

            DragDropItem item = eventData.pointerDrag.GetComponent<DragDropItem>();

            if (slot == 0)
            {
                cf.item_02GameObject = eventData.pointerDrag;
                cf.item_02 = item.item;
                cf.item_02Much = item.itemMuch;
            }else if (slot == 1)
            {
                cf.item_12GameObject = eventData.pointerDrag;
                cf.item_12 = item.item;
                cf.item_12Much = item.itemMuch;
            }else if (slot == 2)
            {
                cf.item_01GameObject = eventData.pointerDrag;
                cf.item_01 = item.item;
                cf.item_01Much = item.itemMuch;
            }else if (slot == 3)
            {
                cf.item_11GameObject = eventData.pointerDrag;
                cf.item_11 = item.item;
                cf.item_11Much = item.itemMuch;
            }
            
            cf.UpdateOutputSlot();
        }
    }
}
