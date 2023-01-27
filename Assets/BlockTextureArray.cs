using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BlockTextureArray
{
    private const int TextureSizeX = 128;
    private const int TextureSizeY = 256;
    private const string SingleTextureName = "All";

    private static readonly int Glossiness = Shader.PropertyToID("_Glossiness");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    public Material SolidMaterial { get; private set; }
    public Material TransparentMaterial { get; private set; }
    private Texture2DArray _array;
    private int _blockCount;
    private Dictionary<BlockSide, int> _blockSideTextureIndices;

    public void Initialize()
    {
        _blockSideTextureIndices = new Dictionary<BlockSide, int>();
        _blockCount = Enum.GetValues(typeof(Block)).Length;

        _array = new Texture2DArray(TextureSizeX, TextureSizeY,
            _blockCount * 6, TextureFormat.ARGB32, 3, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        FillTextureArray();

        SolidMaterial = new Material(Shader.Find("Custom/StandardTextureArray"));
        SolidMaterial.SetTexture(MainTex, _array);
        SolidMaterial.SetFloat(Glossiness, 0);
        
        TransparentMaterial = new Material(Shader.Find("Custom/TransparentTextureArray"));
        TransparentMaterial.SetTexture(MainTex, _array);
        TransparentMaterial.SetFloat(Glossiness, 0);
    }

    public int GetTextureIndex(Block block, Direction side)
    {
        return _blockSideTextureIndices[new BlockSide(block, side)];
    }

    private void FillTextureArray()
    {
        var textureIndex = 0;
        for (var bi = 0; bi < _blockCount; bi++)
        {
            var block = (Block)bi;
            var blockName = block.ToString();

            if (File.Exists($"{Application.dataPath}/Resources/Blocks/{blockName}/{SingleTextureName}.png"))
            {
                textureIndex = LoadTextureOneForAllSides(block, blockName, textureIndex);
                continue;
            }
            
            textureIndex = LoadTextureOnePerSide(block, blockName, textureIndex);
        }

        _array.Apply();
    }
    
    private int LoadTextureOneForAllSides(Block block, string blockName, int textureIndex)
    {
        for (var si = 0; si < 6; si++)
        {
            var side = (Direction)si;
            _blockSideTextureIndices.Add(new BlockSide(block, side), textureIndex);
        }
        
        var texturePath = $"Blocks/{blockName}/{SingleTextureName}";
        textureIndex = LoadTexture(texturePath, textureIndex);

        return textureIndex;
    }

    private int LoadTextureOnePerSide(Block block, string blockName, int textureIndex)
    {
        for (var si = 0; si < 6; si++)
        {
            var side = (Direction)si;
            var sideName = side.ToString();
            var texturePath = $"Blocks/{blockName}/{sideName}";
            _blockSideTextureIndices.Add(new BlockSide(block, side), textureIndex);
            textureIndex = LoadTexture(texturePath, textureIndex);
        }

        return textureIndex;
    }

    private int LoadTexture(string texturePath, int textureIndex)
    {
        var texture2D = Resources.Load<Texture2D>(texturePath);

        if (texture2D == null) throw new FileLoadException($"Failed to load texture from '{texturePath}'");

        _array.SetPixels(texture2D.GetPixels(), textureIndex);

        return ++textureIndex;
    }

    private readonly struct BlockSide : IEquatable<BlockSide>
    {
        private readonly Block _block;
        private readonly Direction _side;

        public BlockSide(Block block, Direction side)
        {
            _block = block;
            _side = side;
        }


        public bool Equals(BlockSide other)
        {
            return _block == other._block && _side == other._side;
        }

        public override bool Equals(object obj)
        {
            return obj is BlockSide other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)_block, (int)_side);
        }
    }
}