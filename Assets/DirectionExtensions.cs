using System;
using UnityEngine;

public static class DirectionExtensions
{
    public static Vector3Int ToVec(this Direction direction) => direction switch
    {
        Direction.XPos => Vector3Int.right,
        Direction.XNeg => Vector3Int.left,
        Direction.YPos => Vector3Int.up,
        Direction.YNeg => Vector3Int.down,
        Direction.ZPos => Vector3Int.forward,
        Direction.ZNeg => Vector3Int.back,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
    
    public static Quaternion ToRotation(this Direction direction) => direction switch
    {
        Direction.XPos => Quaternion.Euler(0f, 90f, 0f),
        Direction.XNeg => Quaternion.Euler(0f, 270f, 0f),
        Direction.YPos => Quaternion.Euler(90f, 0, 0f),
        Direction.YNeg => Quaternion.Euler(270f, 0, 0f),
        Direction.ZPos => Quaternion.identity,
        Direction.ZNeg => Quaternion.Euler(0f, 180f, 0f),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}