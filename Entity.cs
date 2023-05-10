using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity: MonoBehaviour
{
    private string nombre;

    private int id;

    private int life = 10;

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

    private void die()
    {
        Destroy(this.gameObject);
    }

    public abstract void Jump(Vector3 x);

    public abstract void randomDirection();
    public abstract void huir();
}
