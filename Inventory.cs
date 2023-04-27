using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventorySlots[] InventorySlots;

    public GameObject itemPrefab;

    public Transform itemsTransform;

    public void añadirItem(Item item, int itemMuch,ChunkRenderer chunkRenderer)
    {
        bool done = false;
        bool equal = false;
        bool needMore = false;
        
        //Debug.Log("0");
        
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i].item != null && InventorySlots[i].item.item == item)
            {
                //Debug.Log("1");
                if (InventorySlots[i].item.itemMuch < item.maxStack)
                {
                    InventorySlots[i].item.sumarCantidad(itemMuch);
                    equal = true;
                }
                else
                {
                    //Debug.Log("3");
                    needMore = true;
                }
            }
        }

        if (!equal)
        {
            //Debug.Log("4");
            
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].item == null && !done)
                {
                    GameObject x = Instantiate(itemPrefab,itemsTransform);
                
                    x.GetComponent<RectTransform>().anchoredPosition = InventorySlots[i].GetComponent<RectTransform>().anchoredPosition;

                    DragDropItem dragDrop = x.GetComponent<DragDropItem>();

                    dragDrop.chunkRenderer = chunkRenderer;
                
                    dragDrop.item = item;
                    
                    dragDrop.actualizarCantidad(itemMuch);
                
                    InventorySlots[i].item = dragDrop;
                    
                    dragDrop.inicializarFoto();
                    
                    dragDrop.lastWasInventorySlot = true;
                    
                    dragDrop.lastInventorySlot = InventorySlots[i];

                    done = true;
                }
            }
        }

        if (needMore)
        {
            //Debug.Log("6");
            
            done = false;
            
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].item == null && !done)
                {
                    //Debug.Log("7");
                    
                    GameObject x = Instantiate(itemPrefab,itemsTransform);
                
                    x.GetComponent<RectTransform>().anchoredPosition = InventorySlots[i].GetComponent<RectTransform>().anchoredPosition;
                
                    x.GetComponent<DragDropItem>().item = item;
                
                    InventorySlots[i].item = x.GetComponent<DragDropItem>();
                    
                    x.GetComponent<DragDropItem>().inicializarFoto();

                    done = true;
                }
            }
        }
        
        /*DragDropItem otherItem = InventorySlots[i].GetComponent<DragDropItem>();

                    if (otherItem == null) 
                    {
                        //Debug.Log("0");
                        rectTransform.anchoredPosition = startedPosition;
                    }else{
                        if (otherItem.item == item)
                        {
                            //Debug.Log("1");
                            if (otherItem.itemMuch+itemMuch <= item.maxStack)
                            {
                                //Debug.Log("2");
                                otherItem.sumarCantidad(itemMuch);
                                borrarItem();
                            }
                            else
                            {
                                //Debug.Log("3");
                        
                                int x = item.maxStack - (otherItem.itemMuch+itemMuch);

                                if (otherItem.itemMuch < itemMuch)
                                {
                                    otherItem.itemMuch = otherItem.item.maxStack;
                        
                                    actualizarCantidad(-x);
                                    otherItem.actualizarCantidad(item.maxStack);
                                }
                                else
                                {
                                    itemMuch = item.maxStack;
                        
                                    otherItem.actualizarCantidad(-x);
                                    actualizarCantidad(item.maxStack);
                                }
                        
                                rectTransform.anchoredPosition = startedPosition;
                            }
                        }
                        else
                        {
                            otherItem.resetStartPosition();
                    
                            RectTransform otherItemTransform = otherItem.gameObject.GetComponent<RectTransform>();
                            otherItemTransform.anchoredPosition = startedPosition;

                            rectTransform.anchoredPosition = otherItem.startedPosition;

                            resetStartPosition();
                        }
                    }*/
    }
}
