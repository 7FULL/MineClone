using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public Item item_00;
    public Item item_10;
    public Item item_20;

    public Item item_01;
    public Item item_11;
    public Item item_21;

    public Item item_02;
    public Item item_12;
    public Item item_22;
    
    public GameObject item_00GameObject;
    public GameObject item_10GameObject;
    public GameObject item_20GameObject;

    public GameObject item_01GameObject;
    public GameObject item_11GameObject;
    public GameObject item_21GameObject;

    public GameObject item_02GameObject;
    public GameObject item_12GameObject;
    public GameObject item_22GameObject;

    //GameObject

    public int item_00Much = 0;
    public int item_10Much = 0;
    public int item_20Much = 0;

    public int item_01Much = 0;
    public int item_11Much = 0;
    public int item_21Much = 0;

    public int item_02Much = 0;
    public int item_12Much = 0;
    public int item_22Much = 0;


    public Item outputItem;

    public int outputItemMuch = 0;

    [SerializeField] private Recipe[] recipes;

    private Item[] auxRecipeItem;
    private int[] auxRecipeItemMuch;

    [SerializeField] private GameObject outputItemGameObjectPrefab; 
    
    private GameObject outputItemGameObjec;
    
    [SerializeField] public RectTransform outputItemSlot;
    [SerializeField] public RectTransform outputItemSlot2;

    public Canvas canvas;

    private Recipe lastRecipe;
    
    private int[] lastAuxRecipeItemMuch;

    public GameObject items;

    public CraftingSlot[] craftingSlots;

    public GameObject craftingTable;
    
    private GameObject crafting2x2;
    
    private ChunkRenderer chunkRederer;

    public GameObject inventario;

    // Start is called before the first frame update
    void Awake()
    {
        item_00 = GameManager.instance.defaultItem;
        item_10 = GameManager.instance.defaultItem;
        item_20 = GameManager.instance.defaultItem;

        item_01 = GameManager.instance.defaultItem;
        item_11 = GameManager.instance.defaultItem;
        item_21 = GameManager.instance.defaultItem;
        
        item_02 = GameManager.instance.defaultItem;
        item_12 = GameManager.instance.defaultItem;
        item_22 = GameManager.instance.defaultItem;

        crafting2x2 = this.gameObject;

        for (int i = 0; i < craftingSlots.Length; i++)
        {
            craftingSlots[i].cf = this;
        }
    }

    public void update()
    {
        //Debug.Log("3");
        
        for (int i = 0; i < craftingSlots.Length; i++)
        {
            if (craftingSlots[i].gameObject.activeInHierarchy)
            {
                craftingSlots[i].update();
            }
            
            //craftingSlots[i].update();
        }
        UpdateOutputSlot();
    }
    
    private bool UpdateOutputSlot()
    {
        //Debug.Log("2");
        
        outputItem = null;
        outputItemMuch = 0;

        if (outputItemGameObjec != null)
        {
            Destroy(outputItemGameObjec);
        }
        
        UpdateDictionary();

        for (int i = 0; i < recipes.Length; i++)
        {
            Item[] recipeItems = recipes[i].getItems();
            int[] recipeItemsMuch = recipes[i].getItemsMuch();

            bool done = false;
            
            for (int j = 0; j < recipeItems.Length; j++)
            {
                if (auxRecipeItem[j] == null)
                {
                    auxRecipeItem[j] = GameManager.instance.defaultItem;
                }
                
                if (recipeItems[j] != auxRecipeItem[j])
                {
                    //Debug.Log("Receta: "+recipeItems[j]);
                    //Debug.Log("Item: "+auxRecipeItem[j]);
                    done = true;
                }
            }
            
            for (int j = 0; j < recipeItemsMuch.Length; j++)
            {
                if (recipeItemsMuch[j] > auxRecipeItemMuch[j])
                {
                    done = true;
                }
            }
            
            if (!done)
            {
                output(recipes[i]);
                lastAuxRecipeItemMuch = recipeItemsMuch;
                return true;
            }
        }
        
        return false;
    }
    
    private int getMuchToOutput()
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            Item[] recipeItems = recipes[i].getItems();
            int[] recipeItemsMuch = recipes[i].getItemsMuch();

            bool done = false;
            
            for (int j = 0; j < recipeItems.Length; j++)
            {
                if (recipeItems[j] != auxRecipeItem[j])
                {
                    //Debug.Log("Receta: "+recipeItems[j]);
                    //Debug.Log("Item: "+auxRecipeItem[j]);
                    done = true;
                }
            }
            
            for (int j = 0; j < recipeItemsMuch.Length; j++)
            {
                if (recipeItemsMuch[j] > auxRecipeItemMuch[j])
                {
                    done = true;
                }
            }
            
            if (!done)
            {
                int x = Int32.MaxValue;

                int[] y = auxRecipeItemMuch;

                for (int j = 0; j < recipeItemsMuch.Length; j++)
                {
                    int z = 0;
                    
                    while (recipeItemsMuch[j] != 0 && recipeItemsMuch[j] < y[j])
                    {
                        y[j] -= recipeItemsMuch[j];
                        z++;
                    }

                    if (z < x)
                    {
                        x = z;
                    }
                }

                return x;
            }
        }

        return -1;
    }

    private void UpdateDictionary()
    {
        //Debug.Log("0");
        
        auxRecipeItem = new Item[]{item_02,item_12,item_01,item_11,item_00,item_10,item_20,item_21,item_22};
        auxRecipeItemMuch = new int[]{item_02Much,item_12Much,item_01Much,item_11Much,item_00Much,item_10Much,item_20Much,item_21Much,item_22Much};
    }

    
    private void output(Recipe recipe)
    {
        outputItem = recipe.outputItem;

        outputItemMuch = recipe.outputItemMuch;
        
        GameObject x = Instantiate(outputItemGameObjectPrefab,items.transform);

        outputItemGameObjec = x;

        if (outputItemSlot.gameObject.activeInHierarchy)
        {
            x.GetComponent<RectTransform>().anchoredPosition = outputItemSlot.anchoredPosition;
        }
        else
        {
            x.GetComponent<RectTransform>().anchoredPosition = outputItemSlot2.anchoredPosition;
        }

        DragDropItem item = x.GetComponent<DragDropItem>();

        item.isOutput = true;
        item.item = recipe.outputItem;

        for (int i = 0; i < craftingSlots.Length; i++)
        {
            if (craftingSlots[i].item != null)
            {
                chunkRederer = craftingSlots[i].item.chunkRenderer;
            }
        }
        
        item.chunkRenderer = chunkRederer;
        
        item.actualizarCantidad(recipe.outputItemMuch);
        
        item.canvas = canvas;
        item.cf = this;

        item.imagen.sprite = recipe.outputItem.sprite;

        lastRecipe = recipe;
    }

    public Canvas getCanvas()
    {
        return canvas;
    }

    public void outputDone()
    {
        //Debug.Log("1");
        GameObject[] gameobjects = new[] { item_02GameObject, item_12GameObject, item_01GameObject, item_11GameObject, item_00GameObject, item_10GameObject, item_20GameObject,item_21GameObject,item_22GameObject};
        
        for (int i = 0; i < auxRecipeItemMuch.Length; i++)
        {
            auxRecipeItemMuch[i] -= lastAuxRecipeItemMuch[i];

            if (gameobjects[i] != null)
            {
                //gameobjects[i].GetComponent<DragDropItem>().item = auxRecipeItem[i];
                gameobjects[i].GetComponent<DragDropItem>().restarCantidad(lastAuxRecipeItemMuch[i]);
            }
        }

        limpiarTodo();
    }
    
    public int outputAllDone()
    {
        int x = getMuchToOutput();
        
        GameObject[] gameobjects = new[] { item_02GameObject, item_12GameObject, item_01GameObject, item_11GameObject, item_00GameObject, item_10GameObject, item_20GameObject,item_21GameObject,item_22GameObject};

        for (int i = 0; i < gameobjects.Length; i++)
        {
            if (gameobjects[i] != null)
            {
                //gameobjects[i].GetComponent<DragDropItem>().item = auxRecipeItem[i];
                gameobjects[i].GetComponent<DragDropItem>().restarCantidad(x);
            }
        }

        return x * outputItemMuch;
    }

    private void limpiarTodo()
    {
        item_02GameObject = null;
        item_02 = GameManager.instance.defaultItem;
        item_02Much = 0;

        item_12GameObject = null;
        item_12 = GameManager.instance.defaultItem;
        item_12Much = 0;

        item_01GameObject = null;
        item_01 = GameManager.instance.defaultItem;
        item_01Much = 0;

        item_11GameObject = null;
        item_11 = GameManager.instance.defaultItem;
        item_11Much = 0;

        item_00GameObject = null;
        item_00 = GameManager.instance.defaultItem;
        item_00Much = 0;

        item_10GameObject = null;
        item_10 = GameManager.instance.defaultItem;
        item_10Much = 0;

        item_20GameObject = null;
        item_20 = GameManager.instance.defaultItem;
        item_20Much = 0;

        item_21GameObject = null;
        item_21 = GameManager.instance.defaultItem;
        item_21Much = 0;

        item_22GameObject = null;
        item_22 = GameManager.instance.defaultItem;
        item_22Much = 0;
        
        outputItemGameObjec = null;
        outputItem = null;
        outputItemMuch = 0;

        update();
    }
}
