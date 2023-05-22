using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventorySlots[] InventorySlots;

    public GameObject itemPrefab;

    public Transform itemsTransform;

    /*private void Awake()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i] == null)
            {
                //El primer .item es el DragDropItem del inventario
                InventorySlots[i].item.item = GameManager.instance.defaultItem;
            }
        }
    }*/

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
                if (InventorySlots[i].item.itemMuch + itemMuch < item.maxStack)
                {
                    if (!equal)
                    {
                        InventorySlots[i].item.sumarCantidad(itemMuch);
                        equal = true;
                    }
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
    
     public void añadirItem(DragDropItem item)
    {
        bool done = false;
        bool equal = false;
        bool needMore = false;
        
        //Debug.Log("0");
        
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i].item != null && InventorySlots[i].item.item == item.item)
            {
                //Debug.Log("1");
                if (InventorySlots[i].item.itemMuch + item.itemMuch < item.item.maxStack)
                {
                    if (!equal)
                    {
                        InventorySlots[i].item.sumarCantidad(item.itemMuch);
                        equal = true;
                    }
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

                    dragDrop.chunkRenderer = item.chunkRenderer;
                
                    dragDrop.item = item.item;
                    
                    dragDrop.actualizarCantidad(item.itemMuch);
                
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
                
                    x.GetComponent<DragDropItem>().item = item.item;
                
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

    /*private void Update()
    {
        if (Input.GetMouseButtonDown())
        {
            
        }
    }*/
    public DragDropItem[] getHandSlots()
    {
        
        //Como todavia no hemos mtido ningun item hay que inicializarlos todos al item por defecto porque si no devuelve null
        
        //Al no inicializarlo el DragDropItem tambien nos puede devolver null asique mejor que comporbar si es null y
        //asignarle un item al DragDropItem mejor hacer la comporbacion y devolver directamente el item por defecto
        List<DragDropItem> x = new List<DragDropItem>();
        
        for (int i = 0; i < 9; i++)
        {
            if (InventorySlots[i].item == null)
            {
                //Si no hay dragdropitem asignado devolvemos directamente el item por defecto
                x.Add(null);
            }
            else
            {
                x.Add(InventorySlots[i].item);
            }
        }

        return x.ToArray();
    }
}
