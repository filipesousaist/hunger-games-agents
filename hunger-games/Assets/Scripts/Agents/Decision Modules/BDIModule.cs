using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using static Agent;
using UnityEngine;
using static DeciderUtils;


public class BDIModule : DecisionModule
{
    enum Desire
    {
        EAT,
        ATTACK_CLOSEST,
        ATTACK_WEAKEST,
        TRADE_INFORMATION,
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
        //update belief my data
        beliefs.UpdateMyData(myData);
        
        //update belief hazard order
        beliefs.UpdateHazardsOrder(perception.hazardsOrder);
        
        //update belief agentsData
        List<AgentData> otherAgents = perception.visionData.Where((entityData) => entityData.type == Entity.Type.AGENT)
            .Cast<AgentData>().ToList();
        foreach (AgentData otherAgent in otherAgents)
        {
            beliefs.UpdateAgentsData(new Tuple<AgentData, int>(otherAgent, clock));
        }
        
        //update belief map
        beliefs.UpdateMap(perception.visionData);

        //update belief bushes
        List<BushData> bushes = perception.visionData.Where((entityData) => entityData.type == Entity.Type.BUSH)
            .Cast<BushData>().ToList();
        foreach (BushData bush in bushes)
            beliefs.AddBushes(bush);
        
        //update belief chests
        List<ChestData> chests = perception.visionData.Where((entityData) => entityData.type == Entity.Type.CHEST)
            .Cast<ChestData>().ToList();
        foreach (ChestData chest in chests)
            beliefs.AddChests(chest);
    }

    private void GenerateOptions(Perception perception, AgentData myData)
    {
        IEnumerable<AgentData> otherDatas = perception.visionData.Where((data) => data.type == Entity.Type.AGENT)
            .Select((data) => (AgentData) data);
        IEnumerable<AgentData> dangerousAgentDatas = GetDangerousAgentDatas(otherDatas, myData);
        IEnumerable<AgentData> strongerAgents = dangerousAgentDatas.Where
        (
            (otherData) => IsStrongerThan(otherData, myData, Strength)
        );
        IEnumerable<AgentData> allStrongerAgents = otherDatas.Where
        (
            (otherData) => IsStrongerThan(otherData, myData, Strength)
        );

        if (myData.energy < Const.MAX_ENERGY)
            desires.Add(Desire.EAT);

        if (myData.attack < Const.MAX_ATTACK)
            desires.Add(Desire.TRAIN);

        if ((myData.energy < Const.MAX_ENERGY / 5 && dangerousAgentDatas.Any()) || strongerAgents.Any())
            desires.Add(Desire.FLEE);

        if ((myData.weaponType == Weapon.Type.SWORD && myData.weaponAttack < Const.BOW_MAX_ATTACK) ||
                (myData.weaponType == Weapon.Type.SWORD && myData.weaponAttack < Const.SWORD_MAX_ATTACK) ||
                myData.weaponType == Weapon.Type.NONE)
            desires.Add(Desire.SEARCH_WEAPON);

        if (allStrongerAgents.Count() != otherDatas.Count()){
            desires.Add(Desire.ATTACK_WEAKEST);
            desires.Add(Desire.ATTACK_CLOSEST);
        }

        if (perception.hazardsOrder.Contains(null))
            desires.Add(Desire.TRADE_INFORMATION);

        desires.Add(Desire.EXPLORE);
        
    }

    private void FilterIntentions(Perception perception, AgentData myData)
    {
        
    }

    private bool Reconsider()
    {
        return true;
    }
    
    private void Plan()
    {
    }

    private bool Sound(Perception perception, AgentData mydata)
    {

        Agent.Action action = plan.First();
        
        switch(action)
        {
            case Agent.Action.TRAIN: 
                return mydata.energy > Const.TRAIN_ENERGY_LOSS && mydata.attack < Const.MAX_ATTACK;

            case Agent.Action.EAT_BERRIES:
                return perception.nearestBushData.hasBerries;
            
            case Agent.Action.USE_CHEST:
                return IsChestWeaponReasonable(perception.nearestChestData, perception.myData);
            
            case  Agent.Action.ATTACK:
                return true; //redo
            
            case Agent.Action.TRADE:
                return true; //redo
            
            case Agent.Action.WALK:
                return Const.WALK_DISTANCE > 0; //redo
            
            case Agent.Action.ROTATE_LEFT:
                return true; //redo
            
            case Agent.Action.ROTATE_RIGHT:
                return true; //redo
        }
        return true;
    }

    private bool IsChestWeaponReasonable( ChestData chestData,  AgentData myData)
    {
        return (chestData.weaponAttack > myData.weaponAttack && chestData.weaponType == myData.weaponType) ||
               (chestData.weaponType != myData.weaponType && chestData.weaponType != Weapon.Type.NONE);
        
    }
    
    private int Strength(AgentData agentData)
    {
        return (agentData.attack + agentData.weaponAttack) * agentData.energy;
    }
}
