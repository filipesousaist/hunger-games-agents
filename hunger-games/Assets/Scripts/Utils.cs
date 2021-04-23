using System;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
    // Returns a shuffled array containing the integers from 0 to n-1  
    public static int[] ShuffledArray(int n)
    {
        Random rnd = new Random();
        int[] seq = new int[n];

        for (int i = 0; i < n; i++)
            seq[i] = i;
        for (int i = 0; i < n - 1; i++)
        {
            int rnd_i = rnd.Next(i, n);
            // Exchange element in position i with element in position rnd_i
            int tmp = seq[i];
            seq[i] = seq[rnd_i];
            seq[rnd_i] = tmp;
        }

        return seq;
    }
}
