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
            Debug.Log(decider);
            Debug.Log(decider.GetArchitectureName());
            namesToArchitectures.Add(decider.GetArchitectureName(), decider);
        }
    }
}
