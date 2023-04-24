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
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 startedPosition;

    public Item item;
    public int itemMuch;

    public bool isOutput = false;

    public CraftingManager cf;

    public TMP_Text texto;

    public Image imagen;

    private bool lastWasCraftingSlot = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        imagen.sprite = item.sprite;

        cf = FindObjectOfType<CraftingManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
        
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        resetStartPosition();

        if (isOutput)
        {
            cf.outputDone();
            isOutput = false;
        }

        if (lastWasCraftingSlot)
        {
            cf.updateItems(eventData.pointerDrag);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 1;

        if (eventData.pointerEnter.transform.parent != null)
        {
            Slot slot = eventData.pointerEnter.transform.parent.GetComponent<Slot>();
            if (slot == null)
            {
                DragDropItem otherItem = eventData.pointerEnter.transform.parent.GetComponent<DragDropItem>();

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
                        
                        //Updateamos aqui los slots tambien porque si juntamos 2 bloques dentro del craft no los detecta
                        cf.update(eventData);
                    }
                    else
                    {
                        otherItem.resetStartPosition();
                    
                        RectTransform otherItemTransform = otherItem.gameObject.GetComponent<RectTransform>();
                        otherItemTransform.anchoredPosition = startedPosition;

                        rectTransform.anchoredPosition = otherItem.startedPosition;

                        resetStartPosition();
                    }
                }
            }
            else
            {
                Type t = slot.GetType();
                
                if (t.Equals(typeof(CraftingSlot)))
                {
                    lastWasCraftingSlot = true;
                }
            }
        }
        else
        {
            rectTransform.anchoredPosition = startedPosition;
        }
        
        canvasGroup.blocksRaycasts = true;
    }

    public void resetStartPosition()
    {
        startedPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor / 1.5f / 1.5f;
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
        
    }

    public void actualizarCantidad(int x)
    {
        itemMuch = x;
        texto.text = itemMuch.ToString();
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
    }

    public void borrarItem()
    {
        Destroy(this.gameObject);
    }
}
