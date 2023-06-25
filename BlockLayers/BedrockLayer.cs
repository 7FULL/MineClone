using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedrockLayer : BlockLayerHandler
{
    public BlockType bedrock;

    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        Vector3Int pos = new Vector3Int(x, 0, z);
        Chunk.SetBlock(chunkData, pos, bedrock);
        return true;
    }
}
