using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public const int Size = 32;

    public World world;
    public Vector3Int chunkPosition;
    public Material material;
    
    private Block[] blocks;
    private Direction[] blockOrientations;
    private bool isDirty;
    
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector3> uvs;
    private List<int> indices;
    
    public void Initialize()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh
        {
            indexFormat = IndexFormat.UInt32
        };
        meshFilter.mesh = mesh;
        meshRenderer.material = material;

        blocks = new Block[Size * Size * Size];
        blockOrientations = new Direction[Size * Size * Size];
        Array.Fill(blockOrientations, Direction.ZPos);
        
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector3>();
        indices = new List<int>();
    }

    private void Update()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        if (!isDirty) return;
        GenerateMesh();
        isDirty = false;
    }

    private void TestFill()
    {
        for (var z = 0; z < Size; z++)
        {
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var position = new Vector3Int(x, y, z);
                    if (Random.Range(0, 2) == 1) SetBlock(Block.Tile, position);
                }
            }
        }
    }

    private void GenerateMesh()
    {
        for (var z = 0; z < Size; z++)
        {
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var position = new Vector3Int(x, y, z);
                    Block block = GetBlock(position);
                    if (block == Block.Air) continue;
                    BlockData blockData = BlockData.Registry[block];
                    Direction blockOrientation = GetBlockOrientation(position);
                    
                    GenerateFace(block, blockOrientation, blockData, position, Direction.XPos);
                    GenerateFace(block, blockOrientation, blockData, position, Direction.XNeg);
                    GenerateFace(block, blockOrientation, blockData, position, Direction.YPos);
                    GenerateFace(block, blockOrientation, blockData, position, Direction.YNeg);
                    GenerateFace(block, blockOrientation, blockData, position, Direction.ZPos);
                    GenerateFace(block, blockOrientation, blockData, position, Direction.ZNeg);
                }
            }
        }
        
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
    }

    private void GenerateFace(Block block, Direction blockOrientation, BlockData blockData, Vector3Int localPosition, Direction direction)
    {
        Vector3Int directionVec = direction.ToVec();
        Vector3Int worldPosition = localPosition + chunkPosition;
        switch (blockData.MeshType)
        {
            case BlockMeshType.Opaque:
                if (world.GetBlock(worldPosition + directionVec).IsOpaque()) return;
                break;
            case BlockMeshType.Transparent:
                Block adjacentBlock = world.GetBlock(worldPosition + directionVec);
                if (adjacentBlock == block || adjacentBlock.IsOpaque()) return;
                break;
            case BlockMeshType.Model:
            default:
                break;
        }

        int vertexCount = vertices.Count;
        int[] faceIndices = blockData.Mesh.Indices[direction];
        
        for (var i = 0; i < faceIndices.Length; i++)
        {
            indices.Add(faceIndices[i] + vertexCount);
        }

        Quaternion rotation = blockOrientation.ToRotation();
        Vector3[] faceVertices = blockData.Mesh.Vertices[direction];
        Vector3 faceNormal = BlockMesh.Rotate(blockData.Mesh.Normals[direction], rotation);

        for (var i = 0; i < faceVertices.Length; i++)
        {
            vertices.Add(BlockMesh.Rotate(faceVertices[i], rotation) + localPosition);
            normals.Add(faceNormal);
        }

        Vector2[] faceUvs = blockData.Mesh.Uvs[direction];
        int textureId = block.GetTextureId(direction);

        for (var i = 0; i < faceUvs.Length; i++)
        {
            uvs.Add(new Vector3(faceUvs[i].x, faceUvs[i].y, textureId));
        }
    }

    public Block GetBlock(Vector3Int position)
    {
        if (position.x is < 0 or >= Size || position.y is < 0 or >= Size || position.z is < 0 or >= Size)
            return Block.Barrier;
        return blocks[position.x + position.y * Size + position.z * Size * Size];
    }
    
    public Direction GetBlockOrientation(Vector3Int position)
    {
        if (position.x is < 0 or >= Size || position.y is < 0 or >= Size || position.z is < 0 or >= Size)
            return Direction.ZPos;
        return blockOrientations[position.x + position.y * Size + position.z * Size * Size];
    }

    public void SetBlock(Block block, Vector3Int position, Direction orientation = Direction.ZPos)
    {
        if (position.x is < 0 or >= Size || position.y is < 0 or >= Size || position.z is < 0 or >= Size) return;
        int index = position.x + position.y * Size + position.z * Size * Size;
        blocks[index] = block;
        blockOrientations[index] = orientation;
        isDirty = true;
    }
}
