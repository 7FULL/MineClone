using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    private List<GameObject> mobs = new List<GameObject>();
    
    private GameObject[] mobPrefab;
    
    public GameObject[] dayMobs;
    public GameObject[] nightMobs;

    public float outerRadius = 128f;

    public float middleRadius = 32f;

    public float innerRadius = 24f;

    public float detectionRadius = 16f;

    public float despawnTimer = 2f;

    public float despawnProbability = 0.029f;
    
    public Transform playerTransform;

    public int entityCap = 70;

    void Start()
    {
        //playerTransform = transform;
        mobPrefab = dayMobs;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SpawnMob();
        }
    }

    private void FixedUpdate()
    {
        if (mobs.Count < entityCap)
        {
            SpawnMob();
        }
        
        for (int i = 0; i < mobs.Count; i++)
        {
            if (mobs[i] == null)
            {
                mobs.Remove(mobs[i]);
            }
            else
            {
                float distancia = Vector3.Distance(mobs[i].transform.position, transform.position);
                handle(distancia, mobs[i]);
            }
        }   
    }
    
    private bool SpawnMob()
    {
        Vector3 spawnPos = transform.position + Random.insideUnitSphere * outerRadius;

        // Comprobar si el objeto tiene suelo debajo
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, outerRadius))
        {
            spawnPos = hit.point + new Vector3(0,0.2f,0);
        }
        else
        {
            // Comprobar si el objeto tiene suelo arriba
            RaycastHit hit2;
            if (Physics.Raycast(spawnPos, Vector3.up, out hit2, outerRadius))
            {
                spawnPos = hit2.point + new Vector3(0,0.2f,0);
            }
            else
            {
                return false;
            }
        }
        
        

        Vector3 directionToPlayer = playerTransform.position - spawnPos;
        if (Vector3.Dot(directionToPlayer.normalized, playerTransform.forward) < 0f)
            return false;

        int x = Random.Range(0, mobPrefab.Length);
        
        //Debug.Log(x);
    
        GameObject mob = Instantiate(mobPrefab[x], spawnPos, Quaternion.identity);
    
        mobs.Add(mob);

        float distancia = Vector3.Distance(spawnPos, playerTransform.position);
    
        handle(distancia, mob);
        
        if (GameManager.instance.isNight)
        {
            mobPrefab = nightMobs;
        }
        else
        {
            mobPrefab = dayMobs;
        }

        return true;
    }


    private void handle(float distancia, GameObject mob)
    {
        if (!mob.GetComponent<Entity>().enabled)
        {
            mob.GetComponent<Entity>().enabled = true;
        }
        
        //Si estamos fuera de rango
        if (distancia > outerRadius)
        {
            //Debug.Log("Despawneo");
            
            Vector3 directionToPlayer = playerTransform.position - mob.transform.position;
            if (Vector3.Dot(directionToPlayer.normalized, playerTransform.forward) > 0f)
            {
                Destroy(mob);
            }
        }
        
        //Si estamos en la bola 4
        if (distancia <= outerRadius && distancia > middleRadius)
        {
            //Debug.Log("Despawneo aleatorio y no me muevo");
            if (Random.value <= despawnProbability)
            {
                Vector3 directionToPlayer = playerTransform.position - mob.transform.position;
                if (Vector3.Dot(directionToPlayer.normalized, transform.forward) > 0f)
                {
                    Destroy(mob);
                }
            }
            mob.GetComponent<Entity>().enabled = false;
        }
        
        //Si estamos en la bola 3
        if (distancia <= middleRadius && distancia > innerRadius)
        {
            if (Random.value <= despawnProbability)
            {
                Vector3 directionToPlayer = playerTransform.position - mob.transform.position;
                if (Vector3.Dot(directionToPlayer.normalized, transform.forward) > 0f)
                {
                    Destroy(mob);
                }
            }
        }
        
        //Estamos en la bola 2
        if (distancia <= innerRadius && distancia > detectionRadius)
        {
            //Debug.Log("Me spawneo aleatorio yuju");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, middleRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
