using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandSurfaceLayerHandler : BlockLayerHandler
{
    public BlockType surfaceBlockType;
    
    public int minDistance = 7;

    public int maxDistance = 10;

    private int[] randomAlturas = new int[100];
    
    private int contador = 0;
    
        private void Awake()
        {
            for (int i = 0; i < 100; i++)
            {
                //en la documentacion pone que en caso de generar uno random con ints el maxvalue no se incluye por eso lo ponemos
                randomAlturas[i] = Random.Range(minDistance, maxDistance+1);
            }
        }
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y <= surfaceHeightNoise && y > surfaceHeightNoise - randomAlturas[contador])
        {
            if (contador < randomAlturas.Length-1)
            {
                contador++;
            }
            else
            {
                contador = 0;
            }
            
            Vector3Int pos = new Vector3Int(x, y - chunkData.worldPosition.y, z);
            Chunk.SetBlock(chunkData, pos, surfaceBlockType);
            return true;
        }
        return false;
    }
}
