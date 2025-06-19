using UnityEngine;

public static class MathExtensions
{
    public static Vector2 To2D(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
}