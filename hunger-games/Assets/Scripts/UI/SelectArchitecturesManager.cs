using System.Collections.Generic;
using UnityEngine;

public class SelectArchitecturesManager : MonoBehaviour
{
    public Decider[] deciders;

    [ReadOnly] public Dictionary<string, Decider> namesToArchitectures = new Dictionary<string, Decider>();

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Decider decider in deciders)
        {
            namesToArchitectures.Add(decider.GetArchitectureName(), decider);
        }
    }
}
