using System;
using UnityEngine;
using Random = System.Random;

// TODO: Make a method to fill a rectangle.
public class CityWorldGenerator : IWorldGenerator
{
    private enum HalfChunkType
    {
        Road,
        Building
    }

    private const int HalfChunkSize = Chunk.Size / 2;
    private HalfChunkType[] halfChunks;
    private int zChunks;
    private int xChunks;

    public void Generate(Random random, World world, Vector3Int size)
    {
        zChunks = size.z / Chunk.Size;
        xChunks = size.x / Chunk.Size;
        halfChunks = new HalfChunkType[zChunks * 2 * xChunks * 2];
        Floor(random, world, size);
    }

    private void Floor(Random random, World world, Vector3Int size)
    {
        int zHalfChunks = zChunks * 2;
        int xHalfChunks = xChunks * 2;

        Array.Fill(halfChunks, HalfChunkType.Building);

        const int axisRoadCount = 2;

        for (var i = 0; i < axisRoadCount; i++)
        {
            int z = random.Next(zHalfChunks);
            
            for (var x = 0; x < xHalfChunks; x++)
            {
                halfChunks[x + z * xHalfChunks] = HalfChunkType.Road;
            }
        }
        
        for (var i = 0; i < axisRoadCount; i++)
        {
            int x = random.Next(xHalfChunks);
            
            for (var z = 0; z < xHalfChunks; z++)
            {
                halfChunks[x + z * xHalfChunks] = HalfChunkType.Road;
            }
        }

        for (var z = 0; z < zHalfChunks; z++)
        {
            for (var x = 0; x < xHalfChunks; x++)
            {
                switch (halfChunks[x + z * xHalfChunks])
                {
                    case HalfChunkType.Road:
                        RoadChunk(world, x, z);
                        break;
                    case HalfChunkType.Building:
                        BuildingChunk(world, x, z);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void RoadChunk(World world, int halfChunkX, int halfChunkZ)
    {
        int startZ = halfChunkZ * HalfChunkSize;
        int startX = halfChunkX * HalfChunkSize;
        
        world.SetRect(Block.Asphalt, new Vector3Int(startX, 0, startZ), new Vector3Int(HalfChunkSize, 1, HalfChunkSize));
    }

    private void BuildingChunk(World world, int halfChunkX, int halfChunkZ)
    {
        int startZ = halfChunkZ * HalfChunkSize;
        int startX = halfChunkX * HalfChunkSize;
        
        world.SetRectHollow(Block.Brick, new Vector3Int(startX + 2, 0, startZ + 2), new Vector3Int(HalfChunkSize - 4, HalfChunkSize, HalfChunkSize - 4));
        world.SetRect(Block.Tile, new Vector3Int(startX, 0, startZ), new Vector3Int(HalfChunkSize, 1, HalfChunkSize));
        world.SetRect(Block.Glass, new Vector3Int(startX + 4, 4, startZ + 2), new Vector3Int(HalfChunkSize - 8, HalfChunkSize - 8, 1));
    }
}