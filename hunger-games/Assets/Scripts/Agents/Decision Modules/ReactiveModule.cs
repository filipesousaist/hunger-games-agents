using System.Collections.Generic;
using static Agent;
using static Entity;
using UnityEngine;
using System.Linq;

public class ReactiveModule : DecisionModule
{
    public bool isUrgent;

    private Action sideToRotate;
    private bool isBlocked = false;

    private const int DANGEROUS_BOW_ANGLE = 15;
    private const int DANGEROUS_MELEE_DISTANCE = 5;

    private const int FLEE_ANGLE = 15;


    public ReactiveModule(Decider decider) : base(decider) {}

    public override void Decide(Perception perception)
    {
        nextAction = Action.WALK; // If no other action is selected, walk

        AgentData myData = perception.myData;

        CheckOtherAgents(perception, myData);

        CheckIfOutsideArea(myData);

        CheckIfEatBerries(perception, myData); //eats berries he believes aren't poisonous
    }

    private void CheckOtherAgents(Perception perception, AgentData myData)
    {
        IEnumerable<AgentData> dangerousAgentDatas = GetDangerousAgentDatas(perception, myData);
        if (myData.energy < Const.MAX_ENERGY / 5 && dangerousAgentDatas.Any())
        {
            nextAction = FleeFromAgents(dangerousAgentDatas, myData);
            isUrgent = true;
            return;
        }

        IEnumerable<AgentData> strongerAgents = dangerousAgentDatas.Where
        (
            (otherData) => DeciderUtils.IsStrongerThan(otherData, myData, Strength)
        );
        if (strongerAgents.Any())
        {
            nextAction = FleeFromAgents(strongerAgents, myData);
            isUrgent = true;
            return;
        }

        // Se vê outros e tem o arco, ataca, ou alinha-se para atacar
        // 
        if (myData.attackWaitTimer == 0)
        {
            IEnumerable<EntityData> otherAgents =
                perception.visionData.Where((entityData) => entityData.type == Type.AGENT);
            if (myData.weaponType == Weapon.Type.BOW &&
                otherAgents.Any
                (
                    (otherData) => DeciderUtils.IsLookingToPosition(myData, otherData.position, 2)
                    //Utils.CheckIfAligned(agentData.position - otherData.position, Utils.GetForward(((AgentData) otherData).rotation), 2)
                )
                ||
                myData.weaponType != Weapon.Type.BOW &&
                perception.agentsInMeleeRange.Any())

                nextAction = Action.ATTACK;
        }
    }

    private Action FleeFromAgents(IEnumerable<AgentData> agentDatas, AgentData myData)
    {
        IEnumerable<Vector3> dangerousAgentPositions = agentDatas.Select((otherData) => (myData.position - otherData.position).normalized);
        Vector3 directionToFlee = Utils.NormalizedAverage(dangerousAgentPositions);
        return DeciderUtils.GetActionToFlee(myData, directionToFlee, FLEE_ANGLE);
    }

    private int Strength(AgentData agentData)
    {
        return agentData.attack * agentData.energy;
    }

    private IEnumerable<AgentData> GetDangerousAgentDatas(Perception perception, AgentData myData)
    {
        return perception.visionData.Select((data) => (AgentData)data).Where
            (
                (otherData) => otherData.weaponType == Weapon.Type.BOW &&
                                    DeciderUtils.IsLookingToPosition(otherData, myData.position, DANGEROUS_BOW_ANGLE)
                                 || otherData.weaponType != Weapon.Type.BOW &&
                                    (otherData.position - myData.position).magnitude <= DANGEROUS_MELEE_DISTANCE
            );
    }

    private void CheckIfOutsideArea(AgentData myData)
    {
        if (myData.outsideShield && !isBlocked)
        {
            if (DeciderUtils.IsLookingToPosition(myData, Vector3.zero, 5))
                nextAction = Action.WALK;
            else
                nextAction = Random.Range(0, 5) == 1 ? Action.WALK : sideToRotate;
        }
    }

    private void CheckIfEatBerries(Perception perception, AgentData myData)
    {
        BushData bushData = perception.nearestBushData;
        if (bushData != null && perception.nearestBushData.hasBerries && !bushData.poisonous && myData.energy < Const.MAX_ENERGY)
        {
            nextAction = Action.EAT_BERRIES;
            if (myData.energy < Const.MAX_ENERGY / 5)
                isUrgent = true;
        }
    }
}
