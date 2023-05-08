using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    private static CraftingManager cf;

    private void Start()
    {
        cf = GetComponent<CraftingManager>();
        
        cf.inventario.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void Interact(Item item)
    {
        switch (item.BlockType)
        {
            case BlockType.MESA_DE_CRAFTEO:
                cf.craftingTable.gameObject.SetActive(true);
                cf.gameObject.SetActive(false);
                cf.inventario.gameObject.SetActive(true);
                
                GameManager.instance.player.GetComponent<PlayerController3D>().pararse();
                break;
            default:
                Debug.Log("No coincidio con nada el item: "+ item);
                break;
        }
    }
}
