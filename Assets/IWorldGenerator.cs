using UnityEngine;
using Random = System.Random;

public interface IWorldGenerator
{
    public void Generate(Random random, World world, Vector3Int size);
}