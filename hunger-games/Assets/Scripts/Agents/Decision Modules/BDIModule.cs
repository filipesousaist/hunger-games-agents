using System;
using System.Collections.Generic;
using System.Linq;
using static Agent;
using static Const;
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
        NEAREST_STRONGER_WEAPON,
        NEAREST_DIFFERENT_WEAPON,
        NEAREST_STRONGER_DIFFERENT_WEAPON,
        STRONGEST_STRONGER_WEAPON,
        STRONGEST_DIFFERENT_WEAPON,
        STRONGEST_STRONGER_DIFFERENT_WEAPON,
        EXPLORE
    }

    private const float MAX_DIST_TO_BE_BLOCKED = 1.3f;
    private const float MIN_ANGLE_TO_BE_BLOCKED = 40;

    private int clock;

    private Belief beliefs;
    private List<Desire> desires;
    private Pair<Desire, Vector3> intention;

    private readonly Stack<Agent.Action> plan;


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
        AgentData myData = perception.myData;
        
        ChooseAction(Agent.Action.WALK); // If no other action is selected, walk
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

        if ((myData.energy < Const.MAX_ENERGY / 5 && dangerousAgentDatas.Any()) || strongerAgents.Any())
            desires.Add(Desire.FLEE);
        
        if (allStrongerAgents.Count() != otherDatas.Count())
        {
            desires.Add(Desire.ATTACK_WEAKEST);
            desires.Add(Desire.ATTACK_CLOSEST);
        }
        
        if (myData.energy < Const.MAX_ENERGY)
            desires.Add(Desire.EAT);

        if (myData.attack < Const.MAX_ATTACK)
            desires.Add(Desire.TRAIN);

        if ((myData.weaponType == Weapon.Type.SWORD && myData.weaponAttack < Const.SWORD_MAX_ATTACK) ||
            (myData.weaponType == Weapon.Type.SWORD && myData.weaponAttack < Const.BOW_MAX_ATTACK) ||
            (myData.weaponType == Weapon.Type.BOW && myData.weaponAttack < Const.SWORD_MAX_ATTACK) ||
            (myData.weaponType == Weapon.Type.BOW && myData.weaponAttack < Const.BOW_MAX_ATTACK))
        {
            desires.Add(Desire.NEAREST_STRONGER_WEAPON);
            desires.Add(Desire.STRONGEST_STRONGER_WEAPON);
            
        }
        if ((myData.weaponType == Weapon.Type.SWORD && myData.weaponAttack < Const.BOW_MAX_ATTACK) ||
            (myData.weaponType == Weapon.Type.BOW && myData.weaponAttack < Const.SWORD_MAX_ATTACK) ||
            myData.weaponType == Weapon.Type.NONE)
        {
            desires.Add(Desire.NEAREST_STRONGER_DIFFERENT_WEAPON);
            desires.Add(Desire.STRONGEST_STRONGER_DIFFERENT_WEAPON);
            
        }
        

        if (perception.hazardsOrder.Contains(null))
            desires.Add(Desire.TRADE_INFORMATION);
        
        desires.Add(Desire.NEAREST_DIFFERENT_WEAPON); 
        desires.Add(Desire.STRONGEST_DIFFERENT_WEAPON); 
        desires.Add(Desire.EXPLORE);
        
    }

    private void FilterIntentions(Perception perception, AgentData myData)
    {
        intention = new Pair<Desire, Vector3>();
       
        //TODO: remove desires that don't seem to make sense ? maybe it's not needed tho
        
        intention.Desire = GetMostUrgentDesire(desires); //highest priority desire

        switch (intention.Desire)
        {
            case Desire.FLEE:
                //TODO: intention.Position = GetFleeDirection(perception.visionData); 
                break;
            case Desire.ATTACK_CLOSEST:
                intention.Position = GetClosestAgentPosition(perception.visionData,myData);
                break;
            case Desire.ATTACK_WEAKEST:
                intention.Position = GetWeakestAgentPosition(perception.visionData);
                break;
            case Desire.EAT: 
                intention.Position = beliefs.GetNearestBushPosition();
                break;
            case Desire.TRAIN:
                intention.Position = myData.position;
                break;
            
            //assume that this desire isn't the one with more utility when
            //i don't believe there is a chest that satisfies my desires
            case Desire.NEAREST_STRONGER_WEAPON:
                intention.Position = beliefs.GetNearestStrongerChestPosition();
                break;
            case Desire.NEAREST_STRONGER_DIFFERENT_WEAPON:
                intention.Position = beliefs.GetNearestStrongerDifferentChestPosition();
                break;
                ;
            case Desire.NEAREST_DIFFERENT_WEAPON:
                intention.Position = beliefs.GetNearestDifferentChestPosition();
                break;
                ;
            case Desire.STRONGEST_STRONGER_WEAPON:
                intention.Position = beliefs.GetStrongestStrongerChestPosition();
                break;
            case Desire.STRONGEST_STRONGER_DIFFERENT_WEAPON:
                intention.Position = beliefs.GetStrongestStrongerDifferentChestPosition();
                break;
                ;
            case Desire.STRONGEST_DIFFERENT_WEAPON:
                intention.Position = beliefs.GetStrongestDifferentChestPosition();
                break;
                ;
            case Desire.TRADE_INFORMATION:
                break;
            case Desire.EXPLORE:
                break;
                
        }


    }

    private bool Reconsider()
    {
        //TODO
        return true;
    }
    
    private void Plan()
    {
        //TODO
    }

    private bool Sound(Perception perception, AgentData myData)
    {
        return plan.Peek() switch
        {
            Agent.Action.TRAIN => 
                myData.energy > TRAIN_ENERGY_LOSS && myData.attack < MAX_ATTACK,
            Agent.Action.EAT_BERRIES => 
                perception.nearestBushData.hasBerries,
            Agent.Action.USE_CHEST =>
                IsReasonableToUseChest(perception.nearestChestData, myData),
            Agent.Action.ATTACK =>
                IsReasonableToAttack(perception, myData),
            Agent.Action.TRADE => 
                true, //redo
            Agent.Action.WALK => 
                !IsBlocked(perception, myData), //redo
            _ => true
        };
    }

    private bool IsReasonableToUseChest(ChestData chestData,  AgentData myData)
    {
        return chestData.state == Chest.State.CLOSED || chestData.state == Chest.State.CLOSING ||
               (chestData.weaponAttack > myData.weaponAttack && chestData.weaponType == myData.weaponType) ||
               (chestData.weaponType != myData.weaponType && chestData.weaponType != Weapon.Type.NONE);
        
    }

    private bool IsReasonableToAttack(Perception perception, AgentData agentData)
    {
        return true;
    }

    private bool IsBlocked(Perception perception, AgentData agentData)
    {
        return true;
    }

    private Vector3 GetClosestAgentPosition(IEnumerable<EntityData> visionData, AgentData myData)
    {
        Vector3 myPosition = myData.position;
        IEnumerable<AgentData> otherDatas = visionData.Where((data) => data.type == Entity.Type.AGENT)
            .Select((data) => (AgentData) data);
        Vector3 closestAgentPosition = otherDatas.First().position;
        foreach (AgentData agent in otherDatas)
        {
            if ((closestAgentPosition - myPosition).magnitude > (agent.position - myPosition).magnitude)
                closestAgentPosition = agent.position;
        }

        return closestAgentPosition;
    }
    
    private Vector3 GetWeakestAgentPosition(IEnumerable<EntityData> visionData)
    {
        IEnumerable<AgentData> otherDatas = visionData.Where((data) => data.type == Entity.Type.AGENT)
            .Select((data) => (AgentData) data);
        AgentData weakestAgent = otherDatas.First();
        foreach (AgentData agent in otherDatas)
        {
            if (Strength(agent) < Strength(weakestAgent))
                weakestAgent = agent;
        }

        return weakestAgent.position;
    }

    private Desire GetMostUrgentDesire(IEnumerable<Desire> allDesires)
    {
        //utility function
        AgentData agentData = beliefs.GetMyData();
        if((agentData.weaponType == Weapon.Type.SWORD && agentData.weaponAttack < Const.BOW_MAX_ATTACK) ||
            (agentData.weaponType == Weapon.Type.BOW && agentData.weaponAttack < Const.SWORD_MAX_ATTACK))
        {
            //importance of search different weapon?
        }

        //REMOVE LATER; only here to avoid crashing
        return Desire.EAT;

    }
    private int Strength(AgentData agentData)
    {
        return (agentData.attack + agentData.weaponAttack) * agentData.energy;
    }

    public class Pair<T1, T2>
    {
        public T1 Desire { get; set; }
        public T2 Position { get; set; }
    }
    
}
