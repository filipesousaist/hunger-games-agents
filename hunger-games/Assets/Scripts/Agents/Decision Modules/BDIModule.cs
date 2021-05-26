using System.Collections.Generic;
using static Agent;
using UnityEngine;

public class BDIModule : DecisionModule
{
    public BDIModule(Decider decider) : base(decider) {}

    public override void Decide(Perception perception)
    {
        throw new System.NotImplementedException();
    }
}
