using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public Item item_01;
    public Item item_11;

    public Item item_02;
    public Item item_12;
    
    public GameObject item_01GameObject;
    public GameObject item_11GameObject;

    public GameObject item_02GameObject;
    public GameObject item_12GameObject;


    public int item_01Much = 0;
    public int item_11Much = 0;
    public int item_02Much = 0;
    public int item_12Much = 0;


    public Item outputItem;

    public int outputItemMuch = 0;

    [SerializeField] private Recipe[] recipes;

    private Item[] auxRecipeItem;
    private int[] auxRecipeItemMuch;

    [SerializeField] private GameObject outputItemGameObjectPrefab; 
    
    private GameObject outputItemGameObjec;
    
    [SerializeField] private RectTransform outputItemSlot;

    public Canvas canvas;

    private Recipe lastRecipe;
    
    private int[] lastAuxRecipeItemMuch;

    public GameObject items;

    public CraftingSlot[] craftingSlots;

    // Start is called before the first frame update
    void Start()
    {
        item_01 = GameManager.instance.defaultItem;
        item_11 = GameManager.instance.defaultItem;

        item_02 = GameManager.instance.defaultItem;
        item_12 = GameManager.instance.defaultItem;
    }

    public void updateItems(GameObject x)
    {
        if (item_01GameObject == x)
        {
            item_01 = GameManager.instance.defaultItem;
            item_01Much = 0;
        }
        if (item_02GameObject == x)
        {
            item_02 = GameManager.instance.defaultItem;
            item_02Much = 0;
        }
        if (item_11GameObject == x)
        {
            item_11 = GameManager.instance.defaultItem;
            item_11Much = 0;
        }
        if (item_12GameObject == x)
        {
            item_12 = GameManager.instance.defaultItem;
            item_12Much = 0;
        }
        
        UpdateOutputSlot();
    }

    public void update()
    {
        for (int i = 0; i < craftingSlots.Length; i++)
        {
            craftingSlots[i].update();
        }
        UpdateDictionary();
    }
    
    public void UpdateOutputSlot()
    {
        outputItem = null;
        outputItemMuch = 0;

        if (outputItemGameObjec != null)
        {
            Destroy(outputItemGameObjec);
        }
        
        UpdateDictionary();

        for (int i = 0; i < recipes.Length; i++)
        {
            Item[] recipeItems = recipes[i].getHalfItems();
            int[] recipeItemsMuch = recipes[i].getHalfItemsMuch();

            bool done = false;
            
            for (int j = 0; j < recipeItems.Length; j++)
            {
                if (auxRecipeItem[j] == null)
                {
                    auxRecipeItem[j] = GameManager.instance.defaultItem;
                }
                
                if (recipeItems[j].name != auxRecipeItem[j].name)
                {
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
            }
        }
    }

    public void UpdateDictionary()
    {
        auxRecipeItem = new Item[] { item_02, item_12, item_01, item_11 };
        auxRecipeItemMuch = new int[] { item_02Much, item_12Much, item_01Much, item_11Much};
    }

    
    private void output(Recipe recipe)
    {
        outputItem = recipe.outputItem;

        outputItemMuch = recipe.outputItemMuch;
        
        GameObject x = Instantiate(outputItemGameObjectPrefab,items.transform);

        outputItemGameObjec = x;

        x.GetComponent<RectTransform>().anchoredPosition = outputItemSlot.anchoredPosition;

        DragDropItem item = x.GetComponent<DragDropItem>();

        item.isOutput = true;
        item.item = recipe.outputItem;
        
        item.actualizarCantidad(recipe.outputItemMuch);
        
        item.canvas = canvas;
        item.cf = this;

        item.imagen.sprite = recipe.outputItem.sprite;

        lastRecipe = recipe;
    }

    public void outputDone()
    {
        GameObject[] gameobjects = new[] { item_02GameObject, item_12GameObject, item_01GameObject, item_11GameObject};
        
        for (int i = 0; i < auxRecipeItemMuch.Length; i++)
        {
            auxRecipeItemMuch[i] -= lastAuxRecipeItemMuch[i];

            if (gameobjects[i] != null)
            {
                //gameobjects[i].GetComponent<DragDropItem>().item = auxRecipeItem[i];
                gameobjects[i].GetComponent<DragDropItem>().restarCantidad(lastAuxRecipeItemMuch[i]);
            }
        }

        outputItemGameObjec = null;
    }
}
