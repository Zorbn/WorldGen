using System.Collections.Generic;

public class BlockData
{
    public static readonly Dictionary<Block, BlockData> Registry = new()
    {
        {
            Block.Air,
            new BlockData(BlockMeshType.Transparent, BlockMesh.CubeMesh)
        },
        {
            Block.Asphalt,
            new BlockData(BlockMeshType.Model, BlockMesh.SlopeMesh)
        },
        {
            Block.Tile,
            new BlockData(BlockMeshType.Opaque, BlockMesh.CubeMesh)
        },
        {
            Block.Brick,
            new BlockData(BlockMeshType.Opaque, BlockMesh.CubeMesh)
        },
        {
            Block.Barrier,
            new BlockData(BlockMeshType.Opaque, BlockMesh.CubeMesh)
        },
        {
            Block.Glass,
            new BlockData(BlockMeshType.Transparent, BlockMesh.CubeMesh)
        }
    };

    public readonly BlockMeshType MeshType;
    public readonly BlockMesh Mesh;

    public BlockData(BlockMeshType meshType, BlockMesh mesh)
    {
        MeshType = meshType;
        Mesh = mesh;
    }
}