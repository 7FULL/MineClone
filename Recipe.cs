using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Data/Recipes")]
public class Recipe : ScriptableObject
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
    
    
    public int item_00Much = 1;
    public int item_10Much = 1;
    public int item_20Much = 1;

    public int item_01Much = 1;
    public int item_11Much = 1;
    public int item_21Much = 1;

    public int item_02Much = 1;
    public int item_12Much = 1;
    public int item_22Much = 1;

    public Item outputItem;
    
    public int outputItemMuch = 1;

    public Dictionary<Item,int> getItems()
    {
        Dictionary<Item,int> x = new Dictionary<Item, int>()
        {
            {item_00,item_00Much},
            {item_10,item_10Much},
            {item_20,item_20Much},
            
            {item_01,item_01Much},
            {item_11,item_11Much},
            {item_21,item_21Much},
            
            {item_02,item_02Much},
            {item_12,item_12Much},
            {item_22,item_22Much},
        };

        return x;
    }
    public Item[] getHalfItems()
    {
        Item[] x= new Item[]{item_02,item_12,item_01,item_11};

        return x;
    }
    
    public int[] getHalfItemsMuch()
    {
        int[] x= new int[]{item_02Much,item_12Much,item_01Much,item_11Much};

        return x;
    }

    public Dictionary<Item,int> getOutputItem()
    {
        return new Dictionary<Item, int>()
        {
            { outputItem, outputItemMuch }
        };
    }
}
