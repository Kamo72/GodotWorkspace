using Godot;
using System;

public static class SimpleNoise
{
    public static float GenerateNoise(float x, int seed)
    {
        int n = (int)x + seed * 57;
        n = (n << 13) ^ n;
        return (1.0f - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);
    }
}