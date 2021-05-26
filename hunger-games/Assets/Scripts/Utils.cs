using UnityEngine;
using System.Collections.Generic;

public static class Utils
{
    /// <returns>A shuffled array containing the integers from minInclusive to maxExclusive - 1.</returns>
    public static int[] ShuffledArray(int minInclusive, int maxExclusive)
    {
        int size = maxExclusive - minInclusive;
        int[] seq = new int[size];

        for (int i = 0; i < size; i++)
            seq[i] = minInclusive + i;
        for (int i = 0; i < size - 1; i++)
        {
            int rnd_i = Random.Range(i, size);
            // Exchange element in position i with element in position rnd_i
            int tmp = seq[i];
            seq[i] = seq[rnd_i];
            seq[rnd_i] = tmp;
        }

        return seq;
    }

    /// <returns>A shuffled array containing the integers from 0 to maxExclusive - 1.</returns>
    public static int[] ShuffledArray(int maxExclusive)
    {
        return ShuffledArray(0, maxExclusive);
    }

    ///<summary>Clamps a number n between a and b (mod b-a).</summary> 

    public static float ClampMod(float n, float a, float b)
    {
        return (n - a) % (b - a) + a;
    }

    public static string GenerateAgentName(Decider decider, int index)
    {
        return decider.GetArchitectureName() + " " + index;
    }

    public static Vector3 GetForward(float rotation_y)
    {
        return Quaternion.AngleAxis(rotation_y, Vector3.up) * Vector3.forward;
    }
    
    public static bool CheckIfAligned(Vector3 direction1, Vector3 direction2, float threshold)
    {
        return Mathf.Abs(Vector3.Angle(direction1, direction2)) <= threshold;
    }

    public static Vector3 NormalizedAverage(IEnumerable<Vector3> vectors)
    {
        Vector3 total = Vector3.zero;

        foreach (Vector3 vector in vectors)
            total += vector;

        return total.normalized;
    }

    private static readonly Dictionary<int, string> DIFF_ORDINALS =
        new Dictionary<int, string>()
        {
            { 1 , "1st" }, { 2 , "2nd" }, { 3 , "3rd" }
        };

    public static string Ordinal(int n)
    {
        return n <= 3 ? DIFF_ORDINALS[n] : n + "th";
    }
}
