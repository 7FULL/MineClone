using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IDropHandler, Slot
{
    public CraftingManager cf;
    [SerializeField] private int slot;

    public DragDropItem item;

    private GameObject gameObjectItem;

    public bool isOutput = false;

    private void Awake()
    {
        //cf = FindObjectOfType<CraftingManager>();
    }

    public void asign(DragDropItem dragDropItem)
    {
        //Debug.Log("1");

        if (!isOutput)
        {
            dragDropItem.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;

            this.item = dragDropItem;

            gameObjectItem = dragDropItem.gameObject;

            dragDropItem.lastWasCraftingSlot = true;

            dragDropItem.lastCraftingSlot = this;

            update();
            
            cf.update();
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && !isOutput)
        {
            //Debug.Log("1");
            
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;

            DragDropItem item = eventData.pointerDrag.GetComponent<DragDropItem>();

            this.item = item;

            gameObjectItem = eventData.pointerDrag;

            update();
            
            cf.update();
        }
    }

    public void implementDrag(DragDropItem dragItem)
    {
        this.item = dragItem;

        gameObjectItem = dragItem.gameObject;
    }

    public void update()
    {
        if (item != null && gameObjectItem != null && item.itemMuch != 0)
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
            else if (slot == 4)
            {
                cf.item_00GameObject = gameObjectItem;
                cf.item_00 = item.item;
                cf.item_00Much = item.itemMuch;
            }
            else if (slot == 5)
            {
                cf.item_10GameObject = gameObjectItem;
                cf.item_10 = item.item;
                cf.item_10Much = item.itemMuch;
            }
            else if (slot == 6)
            {
                cf.item_20GameObject = gameObjectItem;
                cf.item_20 = item.item;
                cf.item_20Much = item.itemMuch;
            }
            else if (slot == 7)
            {
                cf.item_21GameObject = gameObjectItem;
                cf.item_21 = item.item;
                cf.item_21Much = item.itemMuch;
            }
            else if (slot == 8)
            {
                cf.item_22GameObject = gameObjectItem;
                cf.item_22 = item.item;
                cf.item_22Much = item.itemMuch;
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
            else if (slot == 4)
            {
                cf.item_00GameObject = null;
                cf.item_00 = GameManager.instance.defaultItem;
                cf.item_00Much = 0;
            }
            else if (slot == 5)
            {
                cf.item_10GameObject = null;
                cf.item_10 = GameManager.instance.defaultItem;
                cf.item_10Much = 0;
            }
            else if (slot == 6)
            {
                cf.item_20GameObject = null;
                cf.item_20 = GameManager.instance.defaultItem;
                cf.item_20Much = 0;
            }
            else if (slot == 7)
            {
                cf.item_21GameObject = null;
                cf.item_21 = GameManager.instance.defaultItem;
                cf.item_21Much = 0;
            }
            else if (slot == 8)
            {
                cf.item_22GameObject = null;
                cf.item_22 = GameManager.instance.defaultItem;
                cf.item_22Much = 0;
            }
        }
    }

    public void clear()
    {
        gameObjectItem = null;

        item = null;
    }
}
