using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 *
 *
 *El objeto tiene que ser hijo dle canvas si no las escalas no funcionan
 * 
 */
public class DragDropItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public Canvas canvas;
    public RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 startedPosition;

    public Item item;
    public int itemMuch;

    public bool isOutput = false;

    public CraftingManager cf;

    public TMP_Text texto;

    public Image imagen;

    public bool lastWasCraftingSlot = false;
    
    public bool lastWasInventorySlot = false;

    public CraftingSlot lastCraftingSlot;
    
    public InventorySlots lastInventorySlot;
    
    public GameObject dropableItem;
    
    public GameObject dragDropItem;

    private PointerEventData PointerEventData = null;

    public ChunkRenderer chunkRenderer;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        imagen.sprite = item.sprite;

        cf = GameManager.instance.player.GetComponent<PlayerController3D>().craftingManager;
        
        //Utilizar esto en escena de pruebas
        //cf = FindObjectOfType<CraftingManager>();
        
        canvas = cf.getCanvas();
    }

    public void inicializarFoto()
    {
        imagen.sprite = item.sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PointerEventData = eventData;
        
        transform.SetAsLastSibling();
        
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        resetStartPosition();
        
        if (isOutput)
        {
            cf.outputDone();
            isOutput = false;
            cf.update();
        }

        if (lastWasCraftingSlot)
        {
            lastCraftingSlot.clear();

            cf.update();
        }
        
        if (lastWasInventorySlot)
        {
            lastInventorySlot.clear();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        PointerEventData = null;
        //canvasGroup.alpha = 1;

        hacerCosasDeObjeto(eventData,this);
        
        canvasGroup.blocksRaycasts = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && PointerEventData != null)
        {
            if (PointerEventData.pointerEnter.name == "Panel")
            {
                //Debug.Log("0");
                
                dropearSoloUno();
            }
            else if (PointerEventData.pointerEnter.transform.parent.GetComponent<Slot>() != null)
            {
                //Debug.Log("1");
                
                GameObject aux = Instantiate(dragDropItem, this.transform.parent.transform);

                DragDropItem dragDrop= aux.GetComponent<DragDropItem>();

                dragDrop.rectTransform.anchoredPosition = PointerEventData.pointerEnter.transform.parent.GetComponent<RectTransform>().anchoredPosition;

                dragDrop.item = item;
                dragDrop.actualizarCantidad(1);
                dragDrop.inicializarFoto();
                
                PointerEventData.pointerEnter.transform.parent.GetComponent<Slot>().implementDrag(dragDrop);
                
                restarCantidad(1);
                
                //bool x = hacerCosasDeObjeto(PointerEventData,dragDrop);

                transform.SetAsLastSibling();

                /*if (x)
                {
                    Debug.Log("Destruido");
                    Destroy(aux);
                }*/
                
                aux.GetComponent<CanvasGroup>().blocksRaycasts = true;
                
                Type t = PointerEventData.pointerEnter.transform.parent.GetComponent<Slot>().GetType();
                
                if (t.Equals(typeof(CraftingSlot)))
                {
                    if (!PointerEventData.pointerEnter.transform.parent.GetComponent<CraftingSlot>().isOutput)
                    {
                        //Debug.Log("a");
                        
                        lastWasCraftingSlot = true;
                    
                        lastCraftingSlot = PointerEventData.pointerEnter.transform.parent.GetComponent<CraftingSlot>();
                    }
                    else
                    {
                        rectTransform.anchoredPosition = startedPosition;

                        if (lastWasCraftingSlot)
                        {
                            lastCraftingSlot.asign(this);
                        }
                    }
                    
                    cf.update();
                }

                if (t.Equals(typeof(InventorySlots)))
                {
                    lastWasInventorySlot = true;
                    
                    lastInventorySlot = PointerEventData.pointerEnter.transform.parent.GetComponent<InventorySlots>();

                    PointerEventData.pointerEnter.transform.parent.GetComponent<InventorySlots>().item = this;
                }
                
                cf.update();
            }
            else if(PointerEventData.pointerEnter.transform.parent.GetComponent<DragDropItem>() != null)
            {
                //Debug.Log(PointerEventData.pointerEnter.transform.parent.name);
                
                GameObject aux = Instantiate(dragDropItem, this.transform.parent.transform);

                DragDropItem dragDrop= aux.GetComponent<DragDropItem>();

                dragDrop.item = item;
                dragDrop.actualizarCantidad(1);
                dragDrop.inicializarFoto();
                
                restarCantidad(1);
                
                hacerCosasDeObjeto(PointerEventData,dragDrop);

                aux.GetComponent<CanvasGroup>().blocksRaycasts = true;
                
                transform.SetAsLastSibling();

                Destroy(aux);
                
                cf.update();
            }
        }
    }

    public bool hacerCosasDeObjeto(PointerEventData eventData,DragDropItem dropItem)
    {
        bool aux = false;
        
        Slot slot = eventData.pointerEnter.transform.parent.GetComponent<Slot>();
            if (slot == null)
            {
                DragDropItem otherItem = eventData.pointerEnter.transform.parent.GetComponent<DragDropItem>();

                if ((otherItem == null || otherItem.isOutput || isOutput) && eventData.pointerEnter.name != "Panel")
                {
                    //Debug.Log("0");
                    aux = true;
                    dropItem.rectTransform.anchoredPosition = dropItem.startedPosition;
                }else{
                    if (eventData.pointerEnter.name != "Panel")
                    {
                        if (otherItem.item == dropItem.item)
                        {
                        //Debug.Log("1");
                        if (otherItem.itemMuch+dropItem.itemMuch <= dropItem.item.maxStack)
                        {
                            //Debug.Log("2");
                            otherItem.sumarCantidad(dropItem.itemMuch);
                            dropItem.borrarItem();
                        }
                        else
                        {
                            //Debug.Log("3");
                        
                            int x = dropItem.item.maxStack - (otherItem.itemMuch+dropItem.itemMuch);

                            if (otherItem.itemMuch < dropItem.itemMuch)
                            {
                                otherItem.itemMuch = otherItem.item.maxStack;
                        
                                dropItem.actualizarCantidad(-x);
                                otherItem.actualizarCantidad(dropItem.item.maxStack);
                            }
                            else
                            {
                                dropItem.itemMuch = dropItem.item.maxStack;
                        
                                otherItem.actualizarCantidad(-x);
                                dropItem.actualizarCantidad(dropItem.item.maxStack);
                            }

                            aux = true;
                            dropItem.rectTransform.anchoredPosition = dropItem.startedPosition;
                        }
                        
                        //Updateamos aqui los slots tambien porque si juntamos 2 bloques dentro del craft no los detecta
                        cf.update();
                        }
                    else
                    {
                        //Debug.Log("0");

                        otherItem.resetStartPosition();
                        
                        RectTransform otherItemTransform = otherItem.gameObject.GetComponent<RectTransform>();
                        otherItemTransform.anchoredPosition = dropItem.startedPosition;

                        otherItem.lastCraftingSlot = dropItem.lastCraftingSlot;
                        otherItem.lastInventorySlot = dropItem.lastInventorySlot;
                        
                        otherItem.lastWasCraftingSlot = dropItem.lastWasCraftingSlot;
                        otherItem.lastWasInventorySlot = dropItem.lastWasInventorySlot;

                        dropItem.rectTransform.anchoredPosition = otherItem.startedPosition;

                        dropItem.resetStartPosition();
                        otherItem.resetStartPosition();
                        
                        if (dropItem.lastWasCraftingSlot)
                        {
                            dropItem.lastCraftingSlot.asign(this);
                        }
                        
                        if (otherItem.lastWasInventorySlot)
                        {
                            otherItem.lastInventorySlot.asign(this);
                        }
                        
                        if (dropItem.lastWasInventorySlot)
                        {
                            dropItem.lastInventorySlot.asign(this);
                        }
                        
                        if (otherItem.lastWasCraftingSlot)
                        {
                            otherItem.lastCraftingSlot.asign(this);
                        }

                        aux = true;
                    }
                    }
                }
            }
            else
            {
                Type t = slot.GetType();
                
                if (t.Equals(typeof(CraftingSlot)))
                {
                    if (!eventData.pointerEnter.transform.parent.GetComponent<CraftingSlot>().isOutput)
                    {
                        //Debug.Log("a");
                        
                        dropItem.lastWasCraftingSlot = true;
                    
                        dropItem.lastCraftingSlot = eventData.pointerEnter.transform.parent.GetComponent<CraftingSlot>();
                    }
                    else
                    {
                        dropItem.rectTransform.anchoredPosition = startedPosition;

                        if (dropItem.lastWasCraftingSlot)
                        {
                            dropItem.lastCraftingSlot.asign(this);
                        }
                    }
                    
                    cf.update();
                }

                if (t.Equals(typeof(InventorySlots)))
                {
                    dropItem.lastWasInventorySlot = true;
                    
                    dropItem.lastInventorySlot = eventData.pointerEnter.transform.parent.GetComponent<InventorySlots>();

                    eventData.pointerEnter.transform.parent.GetComponent<InventorySlots>().item = this;
                }
            }
            if(eventData.pointerEnter.name == "Panel")
            {
                dropItem.dropearItem();
            }
            
            cf.update();

            return aux;
    }

    private void dropearItem()
    {
        GameObject drop = null;
                        
        drop = Instantiate(dropableItem,GameManager.instance.player.transform.position+(GameManager.instance.player.transform.forward*0.5f), Quaternion.identity);

        drop.GetComponent<DropableItem>().Item = item;
        
        drop.GetComponent<DropableItem>().itemMuch = itemMuch;
        
        drop.GetComponent<DropableItem>().chunkRenderer = chunkRenderer;
        
        drop.GetComponent<Rigidbody>().AddForce(GameManager.instance.player.transform.forward*200);

        Block block = GameManager.instance.getBlockData(item.BlockType);
                            
        if (block != null && block.particleMaterial != null)
        {
            drop.GetComponent<MeshRenderer>().material = block.particleMaterial;
        }
        else
        {
            drop.GetComponent<MeshRenderer>().material = GameManager.instance.defaultMaterial;
        }
        
        Destroy(this.gameObject);
    }
    
    private void dropearSoloUno()
    {
        GameObject drop = null;
                        
        drop = Instantiate(dropableItem,GameManager.instance.player.transform.position+(GameManager.instance.player.transform.forward*0.5f), Quaternion.identity);

        drop.GetComponent<DropableItem>().Item = item;
        
        drop.GetComponent<DropableItem>().itemMuch = 1;
        
        drop.GetComponent<DropableItem>().chunkRenderer = chunkRenderer;
        
        drop.GetComponent<Rigidbody>().AddForce(GameManager.instance.player.transform.forward*200);

        Block block = GameManager.instance.getBlockData(item.BlockType);

        if (block.particleMaterial != null)
        {
            drop.GetComponent<MeshRenderer>().material = block.particleMaterial;
        }
        else
        {
            drop.GetComponent<MeshRenderer>().material = GameManager.instance.defaultMaterial;
        }
        
        restarCantidad(1);
    }
    
    public void resetStartPosition()
    {
        startedPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        PointerEventData = eventData;
        
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor / 1.5f / 1.5f;

        if (transform.GetSiblingIndex() != transform.parent.childCount - 1)
        {
            transform.SetAsLastSibling();
        }

        /*if (Input.GetMouseButtonDown(1) && eventData.pointerEnter.name == "Panel")
        {
            dropearSoloUno();
        }*/
    }

    public void OnDrop(PointerEventData eventData)
    {
        /*if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.GetComponent<Slot>() == null)
            {
                rectTransform.anchoredPosition = startedPosition;
            }
        }
        else
        {
            rectTransform.anchoredPosition = startedPosition;
        }*/
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isOutput && Input.GetKey(KeyCode.LeftShift))
        {
            realizarOutput(item);
        }
    }

    private void realizarOutput(Item item)
    {
        int x = cf.outputAllDone();
        isOutput = false;
        
        cf.update();

        Debug.Log(x);
        
        itemMuch = x;
            
        GameManager.instance.player.GetComponent<PlayerController3D>().inventory.añadirItem(item,x,chunkRenderer);
    }

    public void actualizarCantidad(int x)
    {
        itemMuch = x;
        texto.text = itemMuch.ToString();

        if (x == 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void sumarCantidad(int x)
    {
        itemMuch += x;
        texto.text = itemMuch.ToString();
    }
    
    public void restarCantidad(int x)
    {
        itemMuch -= x;
        texto.text = itemMuch.ToString();

        if (itemMuch == 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void borrarItem()
    {
        Destroy(this.gameObject);
    }
}
