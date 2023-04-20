using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f;
    public static float tileSizeX, tileSizeY;
    public static Dictionary<BlockType, Block> blockTextureDataDictionary = new Dictionary<BlockType, Block>();
    public BlockDataSO textureData;

    private void Awake()
    {
        foreach (Block item in textureData.blockDataList)
        {
            if (blockTextureDataDictionary.ContainsKey(item.blockType) == false)
            {
                blockTextureDataDictionary.Add(item.blockType, item);
            };
        }
        tileSizeX = textureData.textureSizeX;
        tileSizeY = textureData.textureSizeY;
    }
}
