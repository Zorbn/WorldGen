using UnityEngine;
using Random = System.Random;

public class World : MonoBehaviour
{
    [SerializeField] private int sizeInChunks = 4;
    [SerializeField] private GameObject chunkPrefab;

    private Chunk[] chunks;
    private IWorldGenerator worldGenerator;
    private BlockTextureArray blockTextureArray;
    private Random random;
    
    private void Start()
    {
        blockTextureArray = new BlockTextureArray();
        blockTextureArray.Initialize();
        chunks = new Chunk[sizeInChunks * sizeInChunks];
        
        for (var z = 0; z < sizeInChunks; z++)
        {
            for (var x = 0; x < sizeInChunks; x++)
            {
                var chunkWorldPos = new Vector3Int(x * Chunk.Size, 0, z * Chunk.Size);
                GameObject newChunkGameObject = Instantiate(chunkPrefab, chunkWorldPos, Quaternion.identity);
                var newChunk = newChunkGameObject.GetComponent<Chunk>();
                newChunk.world = this;
                newChunk.chunkPosition = chunkWorldPos;
                newChunk.material = blockTextureArray.Material;
                newChunk.Initialize();
                SetChunk(newChunk, new Vector3Int(x, 0, z));
            }
        }

        random = new Random();
        worldGenerator = new CityWorldGenerator();
        worldGenerator.Generate(random, this, new Vector3Int(sizeInChunks * Chunk.Size, Chunk.Size, sizeInChunks * Chunk.Size));
    }

    public Chunk GetChunk(Vector3Int position)
    {
        if (position.x < 0 || position.x >= sizeInChunks || position.y != 0 ||
            position.z < 0 || position.z >= sizeInChunks) return null;
        return chunks[position.x + position.z * sizeInChunks];
    }

    public void SetChunk(Chunk chunk, Vector3Int position)
    {
        if (position.x < 0 || position.x >= sizeInChunks || position.y != 0 ||
            position.z < 0 || position.z >= sizeInChunks) return;
        chunks[position.x + position.z * sizeInChunks] = chunk;
    }
    
    public Block GetBlock(Vector3Int position)
    {
        Vector3Int chunkPos = position / Chunk.Size;
        Vector3Int localPos = position - chunkPos * Chunk.Size;
        Chunk chunk = GetChunk(chunkPos);
        if (chunk is null) return Block.Barrier;
        return chunk.GetBlock(localPos);
    }
    
    public Direction GetBlockOrientation(Vector3Int position)
    {
        Vector3Int chunkPos = position / Chunk.Size;
        Vector3Int localPos = position - chunkPos * Chunk.Size;
        Chunk chunk = GetChunk(chunkPos);
        if (chunk is null) return Direction.ZPos;
        return chunk.GetBlockOrientation(localPos);
    }

    public void SetBlock(Block block, Vector3Int position, Direction orientation = Direction.ZPos)
    {
        Vector3Int chunkPos = position / Chunk.Size;
        Vector3Int localPos = position - chunkPos * Chunk.Size;
        Chunk chunk = GetChunk(chunkPos);
        if (chunk is null) return;
        chunk.SetBlock(block, localPos, orientation);
    }

    public void SetRect(Block block, Vector3Int position, Vector3Int size, Direction orientation = Direction.ZPos)
    {
        for (var z = 0; z < size.z; z++)
        {
            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    SetBlock(block, position + new Vector3Int(x, y, z), orientation);
                }
            }
        }
    }
    
    public void SetRectHollow(Block block, Vector3Int position, Vector3Int size, Direction orientation = Direction.ZPos)
    {
        SetRect(block, position, new Vector3Int(size.x, size.y, 1), orientation);
        SetRect(block, position, new Vector3Int(size.x, 1, size.z), orientation);
        SetRect(block, position, new Vector3Int(1, size.y, size.z), orientation);

        SetRect(block, position + new Vector3Int(0, 0, size.z - 1), new Vector3Int(size.x, size.y, 1), orientation);
        SetRect(block, position + new Vector3Int(0, size.y - 1, 0), new Vector3Int(size.x, 1, size.z), orientation);
        SetRect(block, position + new Vector3Int(size.x - 1, 0, 0), new Vector3Int(1, size.y, size.z), orientation);
    }
}