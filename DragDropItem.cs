using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        startedPosition = rectTransform.anchoredPosition;

        if (isOutput)
        {
            cf.outputDone();
            isOutput = false;
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
                rectTransform.anchoredPosition = startedPosition;
            }
            else
            {
                Type t = slot.GetType();
                
                //Logica de añadir los items borrarlos del anterior o devolverlos etc
                
                if (t.Equals(typeof(CraftingSlot)))
                {

                }
            }
        }
        else
        {
            rectTransform.anchoredPosition = startedPosition;
        }
        
        canvasGroup.blocksRaycasts = true;
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
    }
}
