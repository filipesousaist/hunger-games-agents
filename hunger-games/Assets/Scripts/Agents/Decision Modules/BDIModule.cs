using System;
using System.Collections.Generic;
using System.Linq;
using static Agent;
using UnityEngine;


public class BDIModule : DecisionModule
{
    enum Desire
    {
        EAT,
        AVOID_OBSTACLE,
        ATTACK_CLOSEST,
        ATTACK_WEAKEST,
        TRAIN,
        FLEE,
        SEARCH_WEAPON,
        EXPLORE
    }

    private int clock;

    private Belief beliefs;
    private List<Desire> desires;
    private Tuple<Desire, Vector3> intention;

    private Stack<Agent.Action> plan;


    public BDIModule(Decider decider) : base(decider)
    {

        beliefs = new Belief();
        desires = new List<Desire>();
        intention = null;
        beliefs = new Belief();
        clock = 0;
    }

    public override void Decide(Perception perception)
    {
        ChooseAction(Agent.Action.WALK); // If no other action is selected, walk

        AgentData myData = perception.myData;

        UpdateBeliefs(perception, myData);

        if (!plan.Any())
        {
            Plan();
        }

        if (Reconsider())
        {
            GenerateOptions(perception, myData);

            FilterIntentions(perception, myData);
        }

        if (!Sound(perception, myData))
        {
            Plan();
        }

        ChooseAction(plan.Pop());

        clock++;
    }

    private void UpdateBeliefs(Perception perception, AgentData myData)
    {
        List<AgentData> otherAgents = perception.visionData.Where((entityData) => entityData.type == Entity.Type.AGENT)
            .Cast<AgentData>().ToList();
        foreach (AgentData otherAgent in otherAgents)
        {
            beliefs.UpdateAgentsData(new Tuple<AgentData, int>(otherAgent, clock));
        }


        beliefs.UpdateMap(perception.visionData);

        List<BushData> bushes = perception.visionData.Where((entityData) => entityData.type == Entity.Type.BUSH)
            .Cast<BushData>().ToList();
        foreach (BushData bush in bushes)
        {
            beliefs.AddBushes(bush);
        }
    }

    private void GenerateOptions(Perception perception, AgentData myData)
    {


    }

    private void FilterIntentions(Perception perception, AgentData myData)
    {

    }

    private bool Reconsider()
    {
        return true;
    }

    private bool Sound(Perception perception, AgentData mydata)
    {

        Agent.Action action = plan.First();
        
        if (action == Agent.Action.TRAIN)
            return mydata.energy > Const.TRAIN_ENERGY_LOSS && mydata.attack < Const.MAX_ATTACK;

        if (action == Agent.Action.EAT_BERRIES)
            return perception.nearestBushData.hasBerries;

        if (action == Agent.Action.USE_CHEST)
            return IsChestWeaponReasonable(perception.nearestChestData, perception.myData);

        if (action == Agent.Action.ATTACK)
            return true;

        if (action == Agent.Action.TRADE)
            return true;

        if (action == Agent.Action.WALK)
            return Const.WALK_DISTANCE == 0; //redo

        return true;

    }


private void Plan()
    {
        
    }

    private bool IsChestWeaponReasonable( ChestData chestData,  AgentData myData)
    {
        return (chestData.weaponAttack > myData.weaponAttack && chestData.weaponType == myData.weaponType) ||
               (chestData.weaponType != myData.weaponType && chestData.weaponType != Weapon.Type.NONE);
        
    }
}
