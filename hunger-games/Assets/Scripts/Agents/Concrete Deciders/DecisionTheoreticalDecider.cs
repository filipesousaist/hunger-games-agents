using System.Runtime.CompilerServices;
using UnityEngine;
using static Agent;

public class DecisionTheoreticalDecider : Decider
{
    private DecisionTheoreticalModule decisionTheoreticalModule;

    private void Awake()
    {
        decisionTheoreticalModule = new DecisionTheoreticalModule(this);
    }

    public override void Decide(Perception perception)
    {
        decisionTheoreticalModule.Decide(perception);
        //Debug.Log(nextAction);
    }

    public override string GetArchitectureName()
    {
        return "Decision Theoretical";
    }
}
