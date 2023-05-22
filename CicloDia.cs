using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CicloDia : MonoBehaviour
{
    
   public int rotationScale = 10;

   private Light light;

   private void Start()
   {
       light = GetComponent<Light>();
   }

   void Update()
    {
        transform.Rotate(rotationScale * Time.deltaTime ,0,0);

        if (transform.rotation.eulerAngles.x > 200)
        {
            light.enabled = false;
        }
        else
        {
            light.enabled = true;
        }
    }
}
