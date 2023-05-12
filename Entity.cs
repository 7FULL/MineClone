using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity: MonoBehaviour
{
    private string nombre;

    private int id;

    private int life = 10;

    public Item itemDropable;

    public int itemMuch = 1;

    public GameObject dropItem;

    private void Update()
    {
        if (transform.position.y < 256)
        {
            Destroy(this.gameObject);
        }
    }

    public void dropear()
    {
        GameObject drop = Instantiate(dropItem,transform.position, Quaternion.identity);

        drop.GetComponent<DropableItem>().chunkRenderer = GameManager.instance.player.GetComponent<PlayerController3D>().lastChunkRenderer;
        
        //Debug.Log(blockTypeToCompare);

        drop.GetComponent<DropableItem>().Item = itemDropable;
        drop.GetComponent<MeshRenderer>().material = GameManager.instance.getBlockData(itemDropable.BlockType).particleMaterial;
        drop.GetComponent<DropableItem>().itemMuch = itemMuch;
    }

    public string Nombre
    {
        get => nombre;
        set => nombre = value;
    }

    public int ID
    {
        get => id;
        set => id = value;
    }

    public int Life
    {
        get => life;
        set => life = value;
    }

    public void takeDamage(int damage)
    {
        life -= damage;

        if (life <= 0)
        {
            die();   
        }
    }

    public void die()
    {
        dropear();
        
        Destroy(this.gameObject);
    }

    public abstract void Jump(Vector3 x);

    public abstract void randomDirection();
    public abstract void huir();
}
