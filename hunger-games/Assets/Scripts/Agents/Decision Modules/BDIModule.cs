using System;
using System.Collections.Generic;
using System.Linq;
using static Agent;
using UnityEngine;
using static Entity;

public class BDIModule : DecisionModule
{
    enum Desire
    {
        EAT, AVOID_OBSTACLE, ATTACK_CLOSEST, ATTACK_WEAKEST, TRAIN, FLEE, SEARCH_WEAPON, EXPLORE
    }

    private Tuple<AgentData, int>[] beliefs;
    private List<Desire> desires;
    private Tuple<Desire, Vector3> intention;

    private List<Agent.Action> plan;
    private List<BushData> bushes;
    private List<List<EntityData>> map;
    
    private HazardsManager hazardsManager;

    public BDIModule(Decider decider, HazardsManager hazardsManager) : base(decider)
    {
        this.hazardsManager = hazardsManager;
        
        beliefs = new Tuple<AgentData, int>[8];
        desires = new List<Desire>();
        intention = null;
    }

    public override void Decide(Perception perception)
    {
        ChooseAction(Agent.Action.WALK); // If no other action is selected, walk
        
        AgentData myData = perception.myData;

        updateBeliefs(perception, myData);

        if (reconsider())
        {
            generateOptions(perception, myData);
            
            filterIntentions(perception, myData);
        }

        if (plan.Count == 0)
        {
            getPlan();
        }

        if (!sound())
        {
            getPlan();
        }

        nextAction = plan.First();
        plan = plan.GetRange(1, plan.Count - 1);
    }

    private void updateBeliefs(Perception perception, AgentData myData)
    {
        IEnumerable<AgentData> agents = perception.visionData.Where((entityData) => entityData.type == Entity.Type.AGENT).Cast<AgentData>();
        Debug.Log("Hey");
    }

    private void generateOptions(Perception perception, AgentData myData)
    {
        
    }

    private void filterIntentions(Perception perception, AgentData myData)
    {
        
    }

    private bool reconsider()
    {
        return true;
    }

    private bool sound()
    {
        return true;
    }

    private void getPlan()
    {
        
    }
}
