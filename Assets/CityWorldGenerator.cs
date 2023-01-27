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
    private HalfChunkType[] _halfChunks;
    private int _zChunks;
    private int _yChunks;
    private int _xChunks;

    public void Generate(Random random, World world, Vector3Int size)
    {
        _zChunks = size.z / Chunk.Size;
        _yChunks = size.y / Chunk.Size;
        _xChunks = size.x / Chunk.Size;
        _halfChunks = new HalfChunkType[_zChunks * 2 * _xChunks * 2];
        Ground(random, world, size);
    }

    private void Ground(Random random, World world, Vector3Int size)
    {
        var zHalfChunks = _zChunks * 2;
        var xHalfChunks = _xChunks * 2;

        Array.Fill(_halfChunks, HalfChunkType.Building);

        const int axisRoadCount = 2;

        for (var i = 0; i < axisRoadCount; i++)
        {
            var z = random.Next(zHalfChunks);

            for (var x = 0; x < xHalfChunks; x++) _halfChunks[x + z * xHalfChunks] = HalfChunkType.Road;
        }

        for (var i = 0; i < axisRoadCount; i++)
        {
            var x = random.Next(xHalfChunks);

            for (var z = 0; z < xHalfChunks; z++) _halfChunks[x + z * xHalfChunks] = HalfChunkType.Road;
        }

        for (var z = 0; z < zHalfChunks; z++)
        for (var x = 0; x < xHalfChunks; x++)
            switch (_halfChunks[x + z * xHalfChunks])
            {
                case HalfChunkType.Road:
                    RoadChunk(world, x, z, size);
                    break;
                case HalfChunkType.Building:
                    BuildingChunk(random, world, x, z, size);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }

    private void RoadChunk(World world, int halfChunkX, int halfChunkZ, Vector3Int size)
    {
        var startZ = halfChunkZ * HalfChunkSize;
        var startX = halfChunkX * HalfChunkSize;

        world.SetRect(Block.Asphalt, new Vector3Int(startX, 0, startZ),
            new Vector3Int(HalfChunkSize, 1, HalfChunkSize));
    }

    private void BuildingChunk(Random random, World world, int halfChunkX, int halfChunkZ, Vector3Int size)
    {
        var startZ = halfChunkZ * HalfChunkSize;
        var startX = halfChunkX * HalfChunkSize;

        const int floorHeight = 6;
        var maxFloors = size.y / floorHeight;
        var floors = random.Next(1, maxFloors + 1);
        
        world.SetRect(Block.Tile, new Vector3Int(startX, 0, startZ), new Vector3Int(HalfChunkSize, 1, HalfChunkSize));

        for (var i = 0; i < floors; i++)
        {
            BuildingFloor(world, startX, floorHeight * i, startZ, size, floorHeight);
        }
        
        // world.SetRectHollow(Block.Brick, new Vector3Int(startX + 2, 0, startZ + 2),
        // new Vector3Int(HalfChunkSize - 4, HalfChunkSize, HalfChunkSize - 4));
        // world.SetRect(Block.Tile, new Vector3Int(startX, 0, startZ), new Vector3Int(HalfChunkSize, 1, HalfChunkSize));
        // world.SetRect(Block.Glass, new Vector3Int(startX + 4, 4, startZ + 2),
        // new Vector3Int(HalfChunkSize - 8, HalfChunkSize - 8, 1));
    }

    private void BuildingFloor(World world, int startX, int startY, int startZ, Vector3Int size, int height)
    {
        var wallStartX = startX + 2;
        var wallStartZ = startZ + 2;
        const int width = HalfChunkSize - 4;
        
        world.SetRectHollow(Block.Brick, new Vector3Int(wallStartX, startY, wallStartZ),
            new Vector3Int(width, height, width));
        
        FullSizeWindowX(world, wallStartX, startY, wallStartZ, size, width, height);
        FullSizeWindowX(world, wallStartX, startY, wallStartZ + width - 1, size, width, height);
        
        FullSizeWindowZ(world, wallStartX, startY, wallStartZ, size, width, height);
        FullSizeWindowZ(world, wallStartX + width - 1, startY, wallStartZ, size, width, height);
        
        Stairs(world, wallStartX, startY, wallStartZ, size, width, height);
    }

    private void FullSizeWindowX(World world, int startX, int startY, int startZ, Vector3Int size, int width, int height)
    {
        world.SetRect(Block.Glass, new Vector3Int(startX + 1, startY + 1, startZ), new Vector3Int(width - 2, height - 2, 1));
    }
    
    private void FullSizeWindowZ(World world, int startX, int startY, int startZ, Vector3Int size, int width, int height)
    {
        world.SetRect(Block.Glass, new Vector3Int(startX, startY + 1, startZ + 1), new Vector3Int(1, height - 2, width - 2));
    }

    private void Stairs(World world, int startX, int startY, int startZ, Vector3Int size, int width,  int height)
    {
        const int skippedWidth = 2;
        
        for (var i = 0; i < width - skippedWidth - 2; i++)
        {
            var heightOffset = i / 2;
            
            var slope = i % 2 == 0 ? Block.CarpetBottomSlope : Block.CarpetTopSlope;
            var position = new Vector3Int(startX + 1, startY + 1 + heightOffset, startZ + 1 + skippedWidth + i);
            world.SetBlock(slope, position);

            for (var y = 1; y <= heightOffset; y++)
            {
                world.SetBlock(Block.Carpet, position - new Vector3Int(0, y, 0));
            }
        }
    }
}