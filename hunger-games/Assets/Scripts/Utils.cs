using UnityEngine;

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

    // Clamps a number between a and b (mod b-a)

    public static float ClampMod(float n, float a, float b)
    {
        return (n - a) % (b - a) + a;
    }

    public static string GenerateAgentName(Decider decider, int index)
    {
        return decider.GetArchitectureName() + " " + index;
    }
}
