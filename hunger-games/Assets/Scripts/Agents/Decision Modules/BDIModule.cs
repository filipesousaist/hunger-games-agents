using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Agent;
using static Const;
using static DeciderUtils;
using Vector3 = UnityEngine.Vector3;


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
        STRONGER_WEAPON,
        DIFFERENT_WEAPON,
        EXPLORE,
    }

    private const int EPOCHS_FOR_RECONSIDERATION = 100;

    private const float MAX_DIST_TO_BE_BLOCKED = 1.3f;
    private const float MIN_ANGLE_TO_BE_BLOCKED = 40;

    private int clock;

    private readonly Belief beliefs;
    private readonly List<Desire> desires;
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

        if (Reconsider())
        {
            GenerateOptions(perception, myData);
            FilterIntentions(perception, myData);
        }

        if (!plan.Any() || !Sound(perception, myData))
            Plan();

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

        if ((myData.energy < MAX_ENERGY / 5 && dangerousAgentDatas.Any()) || strongerAgents.Any())
            desires.Add(Desire.FLEE);
        
        if (allStrongerAgents.Count() != otherDatas.Count()){
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
        
        if (perception.hazardsOrder.Contains(null))
            desires.Add(Desire.TRADE_INFORMATION);
        
        desires.Add(Desire.DIFFERENT_WEAPON);
        desires.Add(Desire.EXPLORE);
        
    }

    private void FilterIntentions(Perception perception, AgentData myData)
    {

        intention = new Pair<Desire, Vector3>
        {
            //TODO: remove desires that don't seem to make sense ? maybe it's not needed tho

            Desire = GetMostUrgentDesire(desires) //highest priority desire
        };

        switch (intention.Desire)
        {
            case Desire.FLEE:
                intention.Position = GetFleePoint(perception.visionData); 
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
            case Desire.STRONGER_WEAPON:
                intention.Position = beliefs.GetNearestStrongerChestPosition(); //change to utilities using the chests from beliefs
                break;
            case Desire.DIFFERENT_WEAPON:
                intention.Position = beliefs.GetNearestStrongerDifferentChestPosition(); //change to use utilities using the chests from beliefs
                break;
            case Desire.TRADE_INFORMATION:
                intention.Position = GetBetterTradingAgent(beliefs.GetAgentsData(),myData);
                break;
            case Desire.EXPLORE:
                intention.Position = beliefs.GetUnexploredPoint();
                break;
        }
    }

    private void Plan()
    {
        switch (intention.Desire)
        {
            case Desire.EAT:
                //walk until intention.Position and eat
                //while{plan.Push();}
                break;
            case Desire.FLEE:
            //flee through intention.Position direction

            case Desire.TRAIN:
                plan.Push(Agent.Action.TRAIN);
                break;
            
            case Desire.EXPLORE:
                //walk until intention.Position
                break;
            
            case Desire.ATTACK_CLOSEST:
                //walk until intention.Position and Attack
                break;
            case Desire.ATTACK_WEAKEST:
                //walk until intention.Position and Attack
                break;
            
            case Desire.TRADE_INFORMATION:
                //put trade_information as true; walk until intention.Position and trade information;
                break;
            
            case Desire.STRONGER_WEAPON:
                //walk until intention.Position and equip it;
                break;
            case Desire.DIFFERENT_WEAPON:
                //walk until intention.Position and equip it;
                break;
        }
    }
    private bool Reconsider()
    {
        if (clock%EPOCHS_FOR_RECONSIDERATION == 0)
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
                true, //redo
            Agent.Action.WALK => 
                !IsBlocked(perception, myData), //redo
            _ => true
        };
    }

    private bool IsBlocked(Perception perception, AgentData myData)
    {
        foreach (EntityData data in perception.visionData)
        {
            Vector3 difference = new Vector3(data.position.x - myData.position.x, 0, data.position.z - myData.position.z);

            if (data.type != Entity.Type.HAZARD_EFFECT &&
                difference.magnitude <= MAX_DIST_TO_BE_BLOCKED &&
                Vector3.Angle(difference, Utils.GetForward(myData.rotation)) >= MIN_ANGLE_TO_BE_BLOCKED)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsReasonableToAttack(Perception perception, AgentData myData)
    {
        return myData.weaponType == Weapon.Type.BOW || // TODO: check beliefs for other agents positions
            (myData.weaponType != Weapon.Type.BOW && perception.agentsInMeleeRange.Any());
    }

    private bool IsReasonableToUseChest(ChestData chestData,  AgentData myData)
    {
        return chestData.state == Chest.State.CLOSED || chestData.state == Chest.State.CLOSING ||
               (chestData.weaponAttack > myData.weaponAttack && chestData.weaponType == myData.weaponType) ||
               (chestData.weaponType != myData.weaponType && chestData.weaponType != Weapon.Type.NONE);
        
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

    private Vector3 GetFleePoint(IEnumerable<EntityData> obstaclesSeen)
    {
        const int RADIUS = 19;

        return beliefs.GetRandomSafe(RADIUS, clock, obstaclesSeen);
        
    }
        
    private Desire GetMostUrgentDesire(IEnumerable<Desire> allDesires)
    {
        //TODO: utility function
        //MAKE IT DEPEND IN ALL THE OTHER FACTORS SUCH AS CURRENT ENERGY, NUMBER OF AGENTS ALIVE, ETC
        
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
        return (agentData.attack + agentData.weaponAttack) * agentData.energy ;
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
