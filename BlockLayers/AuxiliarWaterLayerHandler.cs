using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuxiliarWaterLayerHandler : BlockLayerHandler
{
    public BlockType blockType = BlockType.WATER;
    public int waterLevel = 1;

    public int desertHeightOffset = 2;
    public float desertNoiseScale = 0.01f;

    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if(y > surfaceHeightNoise && y <= waterLevel)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetBlock(chunkData, pos, blockType);
            if(y == surfaceHeightNoise + 1)
            {
                pos.y = surfaceHeightNoise;
                Chunk.SetBlock(chunkData, pos, BlockType.SAND);
            }
            return true;
        }
        float desertNoise = Mathf.PerlinNoise((x + mapSeedOffset.x) * desertNoiseScale, (z + mapSeedOffset.y) * desertNoiseScale);
        int desertOffset = Mathf.RoundToInt(desertNoise * desertHeightOffset);

        Vector3Int pos2 = new Vector3Int(x, y, z);
                
        if (y <= surfaceHeightNoise + desertOffset && y > surfaceHeightNoise)
        {
            Chunk.SetBlock(chunkData, pos2, BlockType.SAND);
            return true;
        }
        return false;
    }
}