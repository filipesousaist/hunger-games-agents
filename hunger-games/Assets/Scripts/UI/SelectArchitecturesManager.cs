using System.Collections.Generic;
using UnityEngine;

public class SelectArchitecturesManager : MonoBehaviour
{
    public Decider[] deciders;

    [ReadOnly] public Dictionary<string, Decider> namesToArchitectures = new Dictionary<string, Decider>();

    [ReadOnly] public Dictionary<string, int> namesToIndices = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < deciders.Length; i ++)
        {
            string name = deciders[i].GetArchitectureName();
            namesToArchitectures.Add(name, deciders[i]);
            namesToIndices.Add(name, i);
        }
    }
}
