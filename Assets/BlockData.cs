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
            new BlockData(BlockMeshType.Opaque, BlockMesh.CubeMesh)
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
        },
        {
            Block.CarpetTopSlope,
            new BlockData(BlockMeshType.Model, BlockMesh.TopSlopeMesh)
        },
        {
            Block.CarpetBottomSlope,
            new BlockData(BlockMeshType.Model, BlockMesh.BottomSlopeMesh)
        },
        {
            Block.Carpet,
            new BlockData(BlockMeshType.Opaque, BlockMesh.CubeMesh)
        }
    };

    public readonly BlockMeshType MeshType;
    public readonly BlockMesh Mesh;

    private BlockData(BlockMeshType meshType, BlockMesh mesh)
    {
        MeshType = meshType;
        Mesh = mesh;
    }
}