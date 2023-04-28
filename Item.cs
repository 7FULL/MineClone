using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item")]
public class Item : ScriptableObject,Interactable
{
    public string name;

    public Sprite sprite;

    public int maxStack;

    public BlockType BlockType;

    public bool isPlacable = true;

    public bool interactable = false;
    public void interact()
    {
        InteractableManager.Interact(this);
    }

    public override string ToString()
    {
        return "Nombre: "+name+"/n"+
               "Sprite: "+sprite.name+"/n"+
               "MaxStack: "+maxStack+"/n"+
               "BlockType: "+BlockType+"/n"+
               "IsPlacable: "+isPlacable+"/n"+
               "Interactable: "+interactable+"/n";
    }
}
