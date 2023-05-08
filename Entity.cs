using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity: MonoBehaviour
{
    private string nombre;

    private int id;

    private int life;

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

    protected Entity(string nombre, int id, int life)
    {
        this.nombre = nombre;
        this.id = id;
        this.life = life;
    }

    protected Entity()
    {
    }
}
