public static class BlockExtensions
{
    public static bool IsOpaque(this Block block)
    {
        return BlockData.Registry[block].MeshType == BlockMeshType.Opaque;
    }
}