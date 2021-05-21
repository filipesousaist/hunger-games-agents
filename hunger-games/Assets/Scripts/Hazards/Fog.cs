using UnityEngine;

public class Fog : Hazard
{
    protected override void Harm(Agent agent)
    {
        Debug.Log("Fog activation");
    }
}

