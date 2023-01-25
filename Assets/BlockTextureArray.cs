using System;
using System.IO;
using UnityEngine;

// TODO: Save memory by only storing one copy of "All" textures, and two copies for "Vertical" and "Horizontal" textures.
// this will require a rework to the texture indexing system, probably just store indices in a dictionary instead.
public class BlockTextureArray
{
    public const int TexSize = 16;

    private static readonly int Glossiness = Shader.PropertyToID("_Glossiness");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    public Material Material { get; private set; }
    private Texture2DArray array;
    private int blockCount;

    public void Initialize()
    {
        blockCount = Enum.GetValues(typeof(Block)).Length;
        
        array = new Texture2DArray(TexSize, TexSize, 
            blockCount * 6, TextureFormat.ARGB32, 3, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        FillTextureArray();

        Material = new Material(Shader.Find("Custom/StandardTextureArray"));
        Material.SetTexture(MainTex, array);
        Material.SetFloat(Glossiness, 0);
    }
    
    private void FillTextureArray()
    {
        for (var bi = 0; bi < blockCount; bi++)
        {
            var block = (Block)bi;
            var blockName = block.ToString();

            bool singleTexture = File.Exists($"{Application.dataPath}/Resources/Blocks/{blockName}/All.png");
            string sideName = "All";
            
            for (var si = 0; si < 6; si++)
            {
                var side = (Direction)si;
                if (!singleTexture) sideName = side.ToString();
                var texPath = $"Blocks/{blockName}/{sideName}";
                var texture2D = Resources.Load<Texture2D>(texPath);

                if (texture2D == null) throw new FileLoadException($"Failed to load texture from '{texPath}'");

                array.SetPixels(texture2D.GetPixels(), block.GetTextureId(side));
            }
        }

        array.Apply();
    }
}