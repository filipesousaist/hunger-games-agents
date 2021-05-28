using System;
using System.Collections.Generic;
using static Agent;
using static DeciderUtils;
using static Utils;
using UnityEngine;
using System.Linq;
using static Const;

public class DecisionTheoreticalModule : DecisionModule
{

    //private Action sideToRotate;
    //private bool isBlocked = false;
    private Vector3 directionToFlee;
    

    private const float MIN_DISTANCE_TO_AVOID_OBSTACLE = 1.5f;

    private const int FLEE_ANGLE = 15;
    private const int MELEE_MIN_ANGLE = 30;
    private const float URGENT_ENERGY_TO_EAT_BERRIES = 0.2f; // Fraction of max health


    public DecisionTheoreticalModule(Decider decider) : base(decider) {}

    public override void Decide(Perception perception)
    {
        ChooseAction(getMostUtilityAction(perception)); 
    }

    public Agent.Action getMostUtilityAction(Perception perception)
    {
        AgentData myData = perception.myData;

        int rotate_right_considerate = 0;
        int rotate_left_considerate = 0;
        int walk_considerate = 0;

        directionToFlee = Vector3.zero;

        
        float[] otherAgentsUtilities = CheckOtherAgents(perception, myData);
        IEnumerable<EntityData> otherAgents =
            perception.visionData.Where(((otherData) => otherData.type == Entity.Type.AGENT));

        float tradeUtility = 0;
        if (otherAgents.Any())
        {
            if (!perception.myData.readyToTrade)
                tradeUtility = 0.5f *perception.hazardsOrder.Count(data => data == null) / NUM_REGIONS;
        
            else
                tradeUtility = 0.5f * perception.hazardsOrder.Count(data => data == null) / NUM_REGIONS + 0.5f * otherAgents.Count(agent => ((AgentData) agent).readyToTrade)/otherAgents.Count();
        }

        tradeUtility *= 0.1f;

        float attackUtility = otherAgentsUtilities[0];
        float walkUtility = 0.1f;
        walk_considerate += walkUtility > 0 ? 1 : 0;
        walkUtility = ((walk_considerate * walkUtility / (walk_considerate+1)) +
                       (otherAgentsUtilities[2]/ (walk_considerate+1)));
        walk_considerate += walkUtility > 0 ? 1 : 0;
        
        float rotateRightUtility = 0.1f;
        rotate_right_considerate += rotateRightUtility > 0 ? 1 : 0;
        rotateRightUtility = (( rotate_right_considerate * rotateRightUtility / ( rotate_right_considerate+1)) +
                       (otherAgentsUtilities[3]/ ( rotate_right_considerate+1)));
        rotate_right_considerate += rotateRightUtility > 0 ? 1 : 0;
        
        float rotateLeftUtility = 0.1f;
        rotate_left_considerate += rotateRightUtility > 0 ? 1 : 0;
        rotateLeftUtility = (( rotate_left_considerate * rotateLeftUtility / ( rotate_left_considerate+1)) +
                              (otherAgentsUtilities[4]/ ( rotate_left_considerate+1)));
        rotate_left_considerate += rotateLeftUtility > 0 ? 1 : 0;
        
        CheckIfRotate(perception, myData);

        float trainUtility = CheckIfTrain(myData); //only trains if more than 25% of the MAX energy and didnt achieved the MAX_ATTACK

        float useChestUtility = CheckIfUseChest(perception, myData).Item1;
        float idleUtility = (CheckIfUseChest(perception, myData).Item2 + otherAgentsUtilities[1])/2;
        
        CheckIfFleeFromHazard(perception, myData);

        CheckIfOutsideArea(perception,myData);

        float eatUtility = CheckIfEatBerries(perception, myData); //eats berries he believes aren't poisonous

        if (directionToFlee != Vector3.zero)
        {
            Agent.Action action = GetActionToMoveTo(myData, directionToFlee, FLEE_ANGLE);
            float walk = action == Agent.Action.WALK ? (float)perception.numberOfAliveAgents/NUM_AGENTS : 0;
            float rotateRight = action == Agent.Action.ROTATE_RIGHT ? (float)perception.numberOfAliveAgents/NUM_AGENTS : 0;
            float rotateLeft = action == Agent.Action.ROTATE_LEFT ? (float)perception.numberOfAliveAgents/NUM_AGENTS : 0;
            
            walkUtility = walk*((walk_considerate * walkUtility / (walk_considerate+1)) +
                                    (1 / (walk_considerate+1)));
            
            walk_considerate += walkUtility > 0 ? 1 : 0;
            rotateRightUtility = rotateRight*(((rotate_right_considerate) * rotateRightUtility / (rotate_right_considerate+1)) +
                            (1 / (rotate_right_considerate+1)));
            rotate_right_considerate += rotateRightUtility > 0 ? 1 : 0;
            rotateLeftUtility = rotateLeft*(((rotate_left_considerate) * rotateLeftUtility / (rotate_left_considerate+1)) +
                                                (1 / (rotate_left_considerate+1)));
            rotate_left_considerate += rotateLeftUtility > 0 ? 1 : 0;
        }

        List<Tuple<Agent.Action,float>> actions = new List<Tuple<Agent.Action, float>>();
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.EAT_BERRIES,eatUtility));
        //Debug.Log(Time.time + "Eat " + eatUtility );
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.ATTACK,attackUtility));
        //Debug.Log(Time.time + "Attack " + attackUtility);
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.IDLE,idleUtility ));
        //Debug.Log(Time.time + "IDLE " + idleUtility);
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.ROTATE_RIGHT,rotateRightUtility ));
        //Debug.Log(Time.time + "RotateRight " + rotateRightUtility);
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.ROTATE_LEFT,rotateLeftUtility ));
        //Debug.Log(Time.time + "RotateLeft " + rotateLeftUtility);
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.WALK,walkUtility ));
        //Debug.Log(Time.time + "Walk " + walkUtility);
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.TRAIN,trainUtility ));
        //Debug.Log(Time.time + "Train " + trainUtility);
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.USE_CHEST,useChestUtility ));
        //Debug.Log(Time.time + "Chest " + useChestUtility);
        actions.Add(new Tuple<Agent.Action,float> (Agent.Action.TRADE,tradeUtility ));
        //Debug.Log(Time.time + "Trade " + tradeUtility);
        
        IEnumerable<Tuple<Agent.Action,float>> orderedActions = actions.OrderBy(element => element.Item2);
        
        return orderedActions.Last().Item1;

    }

    private void CheckIfRotate(Perception perception, AgentData myData)
    {
        foreach (EntityData data in perception.visionData)
        {
            Vector3 difference = new Vector3(myData.position.x - data.position.x, 0, myData.position.z - data.position.z);

            if (data.type != Entity.Type.HAZARD_EFFECT && 
                difference.magnitude <= MIN_DISTANCE_TO_AVOID_OBSTACLE)
            {
                directionToFlee += difference.normalized;
            }
        }
    }

    private float CheckIfTrain(AgentData myData)
    {
        AgentData myDataAfterTrain = (AgentData) myData.Clone();
        myDataAfterTrain.energy = Mathf.Max(myData.energy - Const.TRAIN_ENERGY_LOSS, 0);
        myDataAfterTrain.attack = Mathf.Min(myData.attack + Const.TRAIN_ATTACK_GAIN, Const.MAX_ATTACK);
        if (Strength(myDataAfterTrain) > Strength(myData))
            return  (float)(MAX_ATTACK - myData.attack)/MAX_ATTACK;

        return 0;
    }

    private Tuple<float,float> CheckIfUseChest(Perception perception, AgentData myData)
    {
        float use_chest_utility = 0;
        float idle_utility = 0;
        
        ChestData chestData = perception.nearestChestData;
        if (chestData != null)
        {
            if (chestData.state == Chest.State.OPENING)
                idle_utility = 1; // Wait for chest opening
            else if (chestData.weaponType != Weapon.Type.NONE &&
                     (myData.weaponType == Weapon.Type.NONE || chestData.weaponAttack > myData.weaponAttack) ||
                     chestData.state == Chest.State.CLOSED)
                use_chest_utility = 1;
        }

        return new Tuple<float, float>(use_chest_utility, idle_utility);
    }

    private float[] CheckOtherAgents(Perception perception, AgentData myData)
    {
        IEnumerable<AgentData> otherDatas = perception.visionData.Where((data) => data.type == Entity.Type.AGENT).Select((data) => (AgentData) data);
        float[] utilities = new float[5];
        
        int attackUtility = 0;
        utilities[0] = attackUtility;
        int idleUtility = 0;
        utilities[1] = idleUtility;
        int walkUtility = 0;
        utilities[2] = walkUtility;
        int rotateRightUtility = 0;
        utilities[3] = rotateRightUtility;
        int rotateLeftUtility = 0;
        utilities[4] = rotateLeftUtility;
        
        if (otherDatas.Any())
        {
            // If I have low energy, and others are able to attack me, flee
            IEnumerable<AgentData> dangerousAgentDatas = DeciderUtils.GetDangerousAgentDatas(otherDatas, myData);
            if (myData.energy < Const.MAX_ENERGY / 5 && dangerousAgentDatas.Any())
            {
                FleeFromAgents(dangerousAgentDatas, myData);
                return utilities;
            }

            // Else, if agents which are able to attack me seem stronger, flee
            IEnumerable<AgentData> strongerAgents = dangerousAgentDatas.Where
            (
                (otherData) => IsStrongerThan(otherData, myData, Strength)
            );
            if (strongerAgents.Any())
            {
                FleeFromAgents(strongerAgents, myData);
                return utilities;
            }

            // Else, try to attack:

            otherDatas = otherDatas.OrderBy((otherData) => (otherData.position - myData.position).sqrMagnitude);

            // Attack if has sword/no weapon and is in melee range, or bow if facing opponent
            foreach (AgentData otherData in otherDatas)
                if (IsInPositionToAttack(perception, myData, otherData, BOW_MIN_ANGLE))
                {
                    utilities[0] = myData.attackWaitTimer == 0 ? 1 : 0;
                    utilities[1] = myData.attackWaitTimer > 0 ? 1 : 0;
                    return utilities;
                }

            // Else, try to reposition
            Agent.Action action = GetActionToPosition(myData, otherDatas.First(), MELEE_MIN_ANGLE);
            utilities[2] = action==Agent.Action.WALK? 1 : 0;
            utilities[3] = action==Agent.Action.ROTATE_RIGHT? 1 : 0;
            utilities[4] = action==Agent.Action.ROTATE_LEFT? 1 : 0;
            return utilities;
        }
        return utilities;
    }

    private void FleeFromAgents(IEnumerable<AgentData> agentDatas, AgentData myData)
    {
        IEnumerable<Vector3> dangerousAgentDirections = agentDatas.Select((otherData) => (myData.position - otherData.position).normalized);

        foreach (Vector3 direction in dangerousAgentDirections)
            directionToFlee += direction;
        
    }
    
    private int Strength(AgentData agentData)
    {
        return (agentData.attack + agentData.weaponAttack) * agentData.energy;
    }

    private void CheckIfFleeFromHazard(Perception perception, AgentData myData)
    {
        HazardEffectData hazardEffectData = perception.hazardsOrder[perception.timeslot];
        if (hazardEffectData != null && myData.currentRegion != 0 && hazardEffectData.region == myData.currentRegion) // Inside region with hazard 
        {
            Vector3 regionVector = Quaternion.AngleAxis(360 / Const.NUM_REGIONS * (myData.currentRegion - 1.5f) , Vector3.up) * Vector3.forward;
            float angle = Vector3.SignedAngle(myData.position, regionVector, Vector3.up);
            Vector3 desiredDirection = (angle < 0) ?
                Quaternion.AngleAxis(90, Vector3.up) * myData.position :
                Quaternion.AngleAxis(-90, Vector3.up) * myData.position;
            directionToFlee += desiredDirection.normalized;
        }
    }


    private float CheckIfEatBerries(Perception perception, AgentData myData)
    {
        BushData bushData = perception.nearestBushData;
        if (bushData != null && perception.nearestBushData.hasBerries && !bushData.poisonous && myData.energy < MAX_ENERGY)
        {
            
            if (myData.energy <= Const.MAX_ENERGY * URGENT_ENERGY_TO_EAT_BERRIES)
            {
                return ((float)(MAX_ENERGY-myData.energy)/MAX_ENERGY);
                
            }
            return ((float)(MAX_ENERGY-myData.energy)/MAX_ENERGY) * (MAX_ENERGY*URGENT_ENERGY_TO_EAT_BERRIES)/myData.energy;
        }

        return 0;
    }
    private void CheckIfOutsideArea(Perception perception, AgentData myData)
    {
        if ((Mathf.Abs(myData.position.x) > perception.shieldRadius ||
             Mathf.Abs(myData.position.z) > perception.shieldRadius))
        {
            directionToFlee -= myData.position.normalized;
        }
    }
}
