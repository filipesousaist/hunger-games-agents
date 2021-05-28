using System.Collections.Generic;
using static Agent;
using UnityEngine;

public class HybridDecider : Decider
{
    private ReactiveModule reactiveModule;
    private DecisionTheoreticalModule dTModule;

    private DecisionModule currentModule;

    private int lastEnergy = Const.MAX_ENERGY;

    private const float RANDOM_MODULE_CHANCE = 0.2f;

    private Dictionary<DecisionModule, int> score;
    private Dictionary<DecisionModule, DecisionModule> other;

    private void Awake()
    {
        reactiveModule = new ReactiveModule(this);
        dTModule = new DecisionTheoreticalModule(this);
        score = new Dictionary<DecisionModule, int>()
        {
            { reactiveModule, 0 },
            { dTModule, 0 }
        };
        other = new Dictionary<DecisionModule, DecisionModule>()
        {
            { reactiveModule, dTModule },
            { dTModule, reactiveModule }
        };
        currentModule = reactiveModule;
    }

    public override void Decide(Perception perception)
    {
        score[currentModule] += GetScore(perception);

        reactiveModule.Decide(perception);
        if (!reactiveModule.isUrgent)
        {
            if (score[currentModule] < score[other[currentModule]]) // Change module if other seems better
                currentModule = other[currentModule];

            DecisionModule decisionModule = 
                Random.Range(0, 1f) >= RANDOM_MODULE_CHANCE ? currentModule : other[currentModule];

            if (decisionModule.Equals(dTModule))
                dTModule.Decide(perception);
        }
        lastEnergy = perception.myData.energy;
            
    }

    private int GetScore(Perception perception)
    {
        int diff = perception.myData.energy - lastEnergy;

        return diff >  0 ? 1 :
               diff == 0 ? 0 :
                          -1;
    }

    public override string GetArchitectureName()
    {
        return "Hybrid";
    }
}
