using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IDropHandler, Slot
{
    CraftingManager cf;
    [SerializeField] private int slot;

    private DragDropItem item;

    private GameObject gameObjectItem;
    
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

            this.item = item;

            gameObjectItem = eventData.pointerDrag;

            update(eventData);
            
            cf.UpdateOutputSlot();
        }
    }

    public void update(PointerEventData eventData)
    {
        if (item != null)
        {
            if (slot == 0)
            {
                cf.item_02GameObject = gameObjectItem;
                cf.item_02 = item.item;
                cf.item_02Much = item.itemMuch;
            }else if (slot == 1)
            {
                cf.item_12GameObject = gameObjectItem;
                cf.item_12 = item.item;
                cf.item_12Much = item.itemMuch;
            }else if (slot == 2)
            {
                cf.item_01GameObject = gameObjectItem;
                cf.item_01 = item.item;
                cf.item_01Much = item.itemMuch;
            }else if (slot == 3)
            {
                cf.item_11GameObject = gameObjectItem;
                cf.item_11 = item.item;
                cf.item_11Much = item.itemMuch;
            }
        }
        else
        {
            if (slot == 0)
            {
                cf.item_02GameObject = null;
                cf.item_02 = GameManager.instance.defaultItem;
                cf.item_02Much = 0;
            }else if (slot == 1)
            {
                cf.item_12GameObject = null;
                cf.item_12 = GameManager.instance.defaultItem;
                cf.item_12Much = 0;
            }else if (slot == 2)
            {
                cf.item_01GameObject = null;
                cf.item_01 = GameManager.instance.defaultItem;
                cf.item_01Much = 0;
            }else if (slot == 3)
            {
                cf.item_11GameObject = null;
                cf.item_11 = GameManager.instance.defaultItem;
                cf.item_11Much = 0;
            }
        }
    }
}
