using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Block Data" ,menuName ="Data/Block Data")]
public class BlockDataSO : ScriptableObject
{
    public float textureSizeX, textureSizeY;
    public List<Block> blockDataList;
}

[Serializable]
public class Block
{
    public BlockType blockType;
    public Vector2 up, down, side;
    public bool isSolid = true;
    public bool generatesCollider = true;

    public int durability = 1;

    public Material particleMaterial;

    public bool isGravitationalBlock = false;
}

