using UnityEngine;
using Random = System.Random;

public class World : MonoBehaviour
{
    public static readonly Vector3Int BlockScale = new(1, 2, 1);
    
    [SerializeField] private int sizeInChunks = 4;
    [SerializeField] private int heightInChunks = 2;
    [SerializeField] private GameObject chunkPrefab;

    private Chunk[] _chunks;
    private IWorldGenerator _worldGenerator;
    private BlockTextureArray _blockTextureArray;
    private Random _random;

    private void Start()
    {
        _blockTextureArray = new BlockTextureArray();
        _blockTextureArray.Initialize();
        _chunks = new Chunk[sizeInChunks * heightInChunks * sizeInChunks];

        for (var z = 0; z < sizeInChunks; z++)
        for (var y = 0; y < heightInChunks; y++)
        for (var x = 0; x < sizeInChunks; x++)
        {
            var position = new Vector3Int(x, y, z);
            var chunkWorldPos = position * Chunk.Size;
            var newChunkGameObject = Instantiate(chunkPrefab, chunkWorldPos * BlockScale, Quaternion.identity);
            var newChunk = newChunkGameObject.GetComponent<Chunk>();
            newChunk.world = this;
            newChunk.chunkPosition = chunkWorldPos;
            newChunk.BlockTextureArray = _blockTextureArray;
            newChunk.Initialize();
            SetChunk(newChunk, position);
        }

        _random = new Random();
        _worldGenerator = new CityWorldGenerator();
        _worldGenerator.Generate(_random, this,
            new Vector3Int(sizeInChunks * Chunk.Size, heightInChunks * Chunk.Size, sizeInChunks * Chunk.Size));
    }

    public Chunk GetChunk(Vector3Int position)
    {
        if (position.x < 0 || position.x >= sizeInChunks || position.y < 0 || position.y >= heightInChunks ||
            position.z < 0 || position.z >= sizeInChunks) return null;
        return _chunks[position.x + position.y * sizeInChunks + position.z * sizeInChunks * heightInChunks];
    }

    public void SetChunk(Chunk chunk, Vector3Int position)
    {
        if (position.x < 0 || position.x >= sizeInChunks || position.y < 0 || position.y >= heightInChunks ||
            position.z < 0 || position.z >= sizeInChunks) return;
        _chunks[position.x + position.y * sizeInChunks + position.z * sizeInChunks * heightInChunks] = chunk;
    }

    public Block GetBlock(Vector3Int position)
    {
        var chunkPos = position / Chunk.Size;
        var localPos = position - chunkPos * Chunk.Size;
        var chunk = GetChunk(chunkPos);
        if (chunk is null) return Block.Barrier;
        return chunk.GetBlock(localPos);
    }

    public Direction GetBlockOrientation(Vector3Int position)
    {
        var chunkPos = position / Chunk.Size;
        var localPos = position - chunkPos * Chunk.Size;
        var chunk = GetChunk(chunkPos);
        if (chunk is null) return Direction.ZPos;
        return chunk.GetBlockOrientation(localPos);
    }

    public void SetBlock(Block block, Vector3Int position, Direction orientation = Direction.ZPos)
    {
        var chunkPos = position / Chunk.Size;
        var localPos = position - chunkPos * Chunk.Size;
        var chunk = GetChunk(chunkPos);
        if (chunk is null) return;
        chunk.SetBlock(block, localPos, orientation);
    }

    public void SetRect(Block block, Vector3Int position, Vector3Int size, Direction orientation = Direction.ZPos)
    {
        for (var z = 0; z < size.z; z++)
        for (var y = 0; y < size.y; y++)
        for (var x = 0; x < size.x; x++)
            SetBlock(block, position + new Vector3Int(x, y, z), orientation);
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