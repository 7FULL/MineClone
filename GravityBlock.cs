using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBlock : MonoBehaviour
{
    private World world;

    public BlockType blockType;
    
    private void Awake()
    {
        world = FindObjectOfType<World>();
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("a");
        world.SetBlock(other.gameObject.GetComponent<ChunkRenderer>(), blockType, transform.position);
        Destroy(this.gameObject);
    }
}
