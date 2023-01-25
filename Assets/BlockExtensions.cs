public static class BlockExtensions
{
    public static int GetTextureId(this Block block, Direction side)
    {
        return (int)side + (int)block * 6;
    }

    public static bool IsOpaque(this Block block)
    {
        return BlockData.Registry[block].MeshType == BlockMeshType.Opaque;
    }
}