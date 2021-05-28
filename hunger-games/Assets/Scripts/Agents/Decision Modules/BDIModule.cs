using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using static Agent;
using static Const;
using static DeciderUtils;

public class BDIModule : DecisionModule
{
    enum Desire
    {
        EAT,
        ATTACK_CLOSEST,
        ATTACK_WEAKEST,
        REQUEST_TRADE,
        TRADE_INFORMATION,
        TRAIN,
        FLEE,
        STRONGER_WEAPON,
        DIFFERENT_WEAPON,
        EXPLORE
    }

    private const int EPOCHS_FOR_RECONSIDERATION = 100;

    private const float MAX_DIST_TO_BE_BLOCKED = 1.5f;
    private const float MAX_ANGLE_TO_BE_BLOCKED = 40;

    private int clock;

    private readonly Belief beliefs;
    private readonly List<Desire> desires;
    private Pair<Desire, Vector3> intention;

    private Stack<Agent.Action> plan;

    private readonly Pathfinder pathfinder;

    public BDIModule(Decider decider) : base(decider)
    {
        pathfinder = new Pathfinder(2 * WORLD_SIZE + 1, 2 * WORLD_SIZE + 1);
        beliefs = new Belief(pathfinder);
        desires = new List<Desire>();
        intention = null;
        clock = -1;
        plan = new Stack<Agent.Action>();
    }

    public override void Decide(Perception perception)
    {
        clock ++;
        AgentData myData = perception.myData;
        
        ChooseAction(Agent.Action.WALK); // If no other action is selected, walk
        UpdateBeliefs(perception, myData);

        bool reconsider = Reconsider();
        if (reconsider)
        {
            GenerateOptions(perception, myData);
            FilterIntentions(perception, myData);
        }

        if (!plan.Any() || !Sound(perception, myData))
        {
            if (!reconsider)
            {
                GenerateOptions(perception, myData);
                FilterIntentions(perception, myData);
            }
            Plan();
        }
        if (plan.Any())
            ChooseAction(plan.Pop());
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
        beliefs.UpdateMap(perception.visionData.Where((entityData) => 
            entityData.type != Entity.Type.HAZARD_EFFECT &&
            entityData.type != Entity.Type.AGENT));

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
        desires.Clear();
        desires.Add(Desire.EXPLORE);
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

        if ((myData.energy < MAX_ENERGY / 5 && dangerousAgentDatas.Any()) || strongerAgents.Any() || 
            InHazardRegion(perception, myData))
            desires.Add(Desire.FLEE);
        
        if (allStrongerAgents.Count() != otherDatas.Count())
        {
            desires.Add(Desire.ATTACK_WEAKEST);
            desires.Add(Desire.ATTACK_CLOSEST);
        }
        
        if (myData.energy < MAX_ENERGY)
            desires.Add(Desire.EAT);

        if (myData.attack < MAX_ATTACK)
            desires.Add(Desire.TRAIN);

        if (( myData.weaponAttack < SWORD_MAX_ATTACK) ||
            (myData.weaponAttack < BOW_MAX_ATTACK) )
        {
            desires.Add(Desire.STRONGER_WEAPON);
        }
        
        if (beliefs.GetHazardsOrder().Contains(null))
        {
            desires.Add(Desire.REQUEST_TRADE);
            desires.Add(Desire.TRADE_INFORMATION);
        }
        
        desires.Add(Desire.DIFFERENT_WEAPON);
        
        //Debug.Log(desires.First());
    }

    private bool InHazardRegion(Perception perception, AgentData myData)
    {
        HazardEffectData currentHazardData = beliefs.GetHazardsOrder()[perception.timeslot];
        return currentHazardData != null && currentHazardData.region == HazardsManager.GetRegion(myData.position);

    }

    private void FilterIntentions(Perception perception, AgentData myData)
    {
        Desire desire = GetMostUrgentDesire(perception, myData);
        intention = new Pair<Desire, Vector3>
        {
            Desire = desire, // highest priority desire
            Position = desire switch
            {
                Desire.FLEE =>
                    GetFleePoint(perception),
                Desire.ATTACK_CLOSEST =>
                    GetClosestAgentPosition(perception.visionData, myData),
                Desire.ATTACK_WEAKEST =>
                    GetWeakestAgentPosition(perception.visionData),
                Desire.EAT =>
                    beliefs.GetBushes().Any() ? beliefs.GetNearestBushPosition() : myData.position,
                Desire.STRONGER_WEAPON =>
                    beliefs.GetChests().Any() ? beliefs.GetNearestStrongerChestPosition() : myData.position,
                Desire.DIFFERENT_WEAPON =>
                    beliefs.GetChests().Any() ? beliefs.GetNearestStrongerDifferentChestPosition() : myData.position,
                Desire.TRADE_INFORMATION =>
                    GetBetterTradingAgent(beliefs.GetAgentsData(), myData),
                Desire.EXPLORE =>
                    beliefs.GetUnexploredPoint(10, perception),
                _ => myData.position
            }
        };
    }

    private void Plan()
    {
        plan.Clear();
        AgentData myData = beliefs.GetMyData();
        switch (intention.Desire)
        {
            case Desire.EAT:
                // walk until intention.Position and eat
                AddActionsToWalkTo(myData, intention.Position);
                plan.Push(Agent.Action.EAT_BERRIES);
                break;

            case Desire.FLEE:
                //flee through intention.Position direction ??
                AddActionsToWalkTo(myData, intention.Position);
                break;

            case Desire.TRAIN:
                plan.Push(Agent.Action.TRAIN);
                PushActions(Agent.Action.IDLE, TRAIN_DURATION - 1);                
                break;
            
            case Desire.EXPLORE:
                AddActionsToWalkTo(myData, intention.Position);
                break;
            
            case Desire.ATTACK_CLOSEST:
            case Desire.ATTACK_WEAKEST:
                AddActionsToWalkTo(myData, intention.Position);
                break;

            case Desire.REQUEST_TRADE:
                plan.Push(Agent.Action.TRADE);
                break;
            
            case Desire.TRADE_INFORMATION:
                AddActionsToWalkTo(myData, intention.Position);
                plan.Push(Agent.Action.TRADE);
                break;
            
            case Desire.STRONGER_WEAPON:
            case Desire.DIFFERENT_WEAPON:
                AddActionsToWalkTo(myData, intention.Position);
                break;
        }
    }

    private void PushActions(Agent.Action action, int n)
    {
        for (int i = 0; i < n; i++)
            plan.Push(action);
    }

    private void AddActionsToWalkTo(AgentData myData, Vector3 destination)
    {
        Debug.Log("Agent " + myData.index + ": start: " + Mathf.RoundToInt(myData.position.x) + "," + Mathf.RoundToInt(myData.position.z) + "; end: " + destination.x + "," + destination.z);
        pathfinder.AddActionsToStack(myData.position, destination, myData.rotation, plan);
    }

    private bool Reconsider()
    {
        if (clock % EPOCHS_FOR_RECONSIDERATION == 0)
            return true;
        return false;
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
                beliefs.GetHazardsOrder().Contains(null),
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

    private bool IsReasonableToAttack(Perception perception, AgentData myData)
    {
        return myData.weaponType == Weapon.Type.BOW || // TODO: check beliefs for other agents positions
            (myData.weaponType != Weapon.Type.BOW && perception.agentsInMeleeRange.Any());
    }

    private bool IsBlocked(Perception perception, AgentData myData)
    {
        foreach (EntityData data in perception.visionData)
        {
            Vector3 difference = new Vector3(data.position.x - myData.position.x, 0, data.position.z - myData.position.z);

            if (data.type != Entity.Type.HAZARD_EFFECT &&
                difference.magnitude <= MAX_DIST_TO_BE_BLOCKED &&
                Vector3.Angle(difference, Utils.GetForward(myData.rotation)) <= MAX_ANGLE_TO_BE_BLOCKED)
            {
                Debug.Log("Agent " + myData.index + " is blocked");
                return true;
            }
        }
        return false;
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

    private Vector3 GetBetterTradingAgent(Tuple<AgentData, int>[] otherAgents, AgentData myData)
    {
        IEnumerable<Tuple<AgentData, int>> tradingAgents = otherAgents.Where((otherAgent) => otherAgent.Item1.readyToTrade);
        Tuple<AgentData, int> betterTradingAgent = tradingAgents.First();
        foreach (Tuple<AgentData, int> agent in tradingAgents)
        {
            if(TradingAgentUtility(agent, myData)>TradingAgentUtility(betterTradingAgent, myData))
            {
                betterTradingAgent = agent;
            }
        }

        return betterTradingAgent.Item1.position;
    }

    private Vector3 GetFleePoint(Perception perception)
    {
        const int RADIUS = 10;

        return beliefs.GetRandomSafe(RADIUS, clock, perception);
        
    }
        
    private Desire GetMostUrgentDesire(Perception perception, AgentData myData)
    {
        //TODO: utility function
        //MAKE IT DEPEND IN ALL THE OTHER FACTORS SUCH AS CURRENT ENERGY, NUMBER OF AGENTS ALIVE, ETC

        float[] utilities = new float[desires.Count];
        for (int i = 0; i < desires.Count; i++)
        {
            utilities[i] = 0;
        }


        //REMOVE LATER; only here to avoid crashing
        return desires.First();

    }
    
    private int Strength(AgentData agentData)
    {
        return (agentData.attack + agentData.weaponAttack) * agentData.energy;
    }

    private int TradingAgentUtility(Tuple<AgentData, int> agentInfo, AgentData myData)
    {  
        int ATTACK_WEIGHT = 2;
        int POSITION_WEIGHT = 4;
        int CLOCK_WEIGHT = 4;
        return  1/(POSITION_WEIGHT*(int)(agentInfo.Item1.position - myData.position).magnitude + CLOCK_WEIGHT*(clock-agentInfo.Item2+1)+ATTACK_WEIGHT*Strength(agentInfo.Item1));

    }
    
    public class Pair<T1, T2>
    {
        public T1 Desire { get; set; }
        public T2 Position { get; set; }
    }
    
}
