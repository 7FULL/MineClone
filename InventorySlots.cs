using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlots : MonoBehaviour, IDropHandler, Slot
{
    public DragDropItem item;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;

            DragDropItem item = eventData.pointerDrag.GetComponent<DragDropItem>();

            this.item = item;
        }
    }

    public void implementDrag(DragDropItem dragItem)
    {
        this.item = dragItem;
    }

    public void clear()
    {
        item = null;
    }

    public void asign(DragDropItem dragDropItem)
    {
        item = dragDropItem;
    }
}
