using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    //Fucnion a utilizar por todos los objetos interactuables
    public void interact(Vector3Int posicion = new Vector3Int(), RaycastHit hit = default);
}
