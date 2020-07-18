using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Fractal Brownian Motion.
    /// Expands the Perlin Noise function adding several ovtaves together, with each octave 
    /// added being larger than the previous one, based on the 'persistence' factor.
    /// </summary>
    public static float fBM(float x, float y, int octave, float persistance)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octave; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistance;
            frequency *= 2;
        }

        return total / maxValue;
    }

    /// <summary>
    /// Reescale a value inside an originalMin and originalMax range to a targetMin and targetMax rannge.
    /// </summary>
    public static float Map(float value, float originalMin, float originalMax, float targetMin, float targetMax)
    {
        return (value - originalMin) * (targetMax - targetMin) / (originalMax - originalMin) + targetMin;
    }

    //Fisher-Yates Shuffle
    public static System.Random random = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
