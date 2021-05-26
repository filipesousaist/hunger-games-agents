using System.Collections.Generic;
using static Agent;
using static Entity;
using static DeciderUtils;
using static Utils;
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
    private const int BOW_MIN_ANGLE = 2;
    private const int MELEE_MIN_ANGLE = 30;
    private const float URGENT_ENERGY_TO_EAT_BERRIES = 0.2f; // Fraction of max health


    public ReactiveModule(Decider decider) : base(decider) {}

    public override void Decide(Perception perception)
    {
        ChooseAction(Action.WALK); // If no other action is selected, walk

        AgentData myData = perception.myData;

        CheckIfRotate(perception, myData);

        CheckIfTrain(myData); //only trains if more than 25% of the MAX energy and didnt achieved the MAX_ATTACK

        CheckIfUseChest(perception, myData);

        CheckOtherAgents(perception, myData);

        CheckIfOutsideArea(myData);

        CheckIfEatBerries(perception, myData); //eats berries he believes aren't poisonous
    }

    private void CheckIfRotate(Perception perception, AgentData myData)
    {
        foreach (EntityData data in perception.visionData)
            if ((new Vector2(data.position.x, data.position.z) -
                new Vector2(myData.position.x, myData.position.z)
                ).magnitude <= 1.5)
            {
                if (!isBlocked)
                {
                    sideToRotate = GetRandomSide();
                    isBlocked = true;
                }

                ChooseAction(sideToRotate);
                return;
            }

        isBlocked = false;
    }

    private void CheckIfTrain(AgentData myData)
    {
        AgentData myDataAfterTrain = (AgentData) myData.Clone();
        myDataAfterTrain.energy = Mathf.Max(myData.energy - Const.TRAIN_ENERGY_LOSS, 0);
        myDataAfterTrain.attack = Mathf.Min(myData.attack + Const.TRAIN_ATTACK_GAIN, Const.MAX_ATTACK);
        if (Strength(myDataAfterTrain) > Strength(myData))
            ChooseAction(Action.TRAIN);
    }

    private void CheckIfUseChest(Perception perception, AgentData myData)
    {
        ChestData chestData = perception.nearestChestData;
        if (chestData != null &&
            chestData.weaponType != Weapon.Type.NONE &&
            (myData.weaponType == Weapon.Type.NONE || chestData.weaponAttack > myData.weaponAttack))
        {
            ChooseAction(Action.USE_CHEST);
        }
    }

    private void CheckOtherAgents(Perception perception, AgentData myData)
    {
        IEnumerable<AgentData> otherDatas = perception.visionData.Where((data) => data.type == Type.AGENT).Select((data) => (AgentData) data);

        if (otherDatas.Any())
        {
            // If I have low energy, and others are able to attack me, flee
            IEnumerable<AgentData> dangerousAgentDatas = GetDangerousAgentDatas(otherDatas, myData);
            if (myData.energy < Const.MAX_ENERGY / 5 && dangerousAgentDatas.Any())
            {
                FleeFromAgents(dangerousAgentDatas, myData);
                return;
            }

            // Else, if agents which are able to attack me seem stronger, flee
            IEnumerable<AgentData> strongerAgents = dangerousAgentDatas.Where
            (
                (otherData) => IsStrongerThan(otherData, myData, Strength)
            );
            if (strongerAgents.Any())
            {
                FleeFromAgents(strongerAgents, myData);
                return;
            }

            // Else, try to attack:

            otherDatas = otherDatas.OrderBy((otherData) => (otherData.position - myData.position).sqrMagnitude);

            // Attack if has sword/no weapon and is in melee range, or bow if facing opponent
            foreach (AgentData otherData in otherDatas)
                if (IsInPositionToAttack(perception, myData, otherData, BOW_MIN_ANGLE))
                {
                    Debug.Log(myData.index + " wants to attack " + otherData.index);
                    ChooseAction(myData.attackWaitTimer == 0 ? Action.ATTACK : Action.IDLE);
                    return;
                }

            // Else, try to reposition
            ChooseAction(GetActionToPosition(myData, otherDatas.First(), MELEE_MIN_ANGLE));
            RotateIfBlocked();
        }
    }

    private void FleeFromAgents(IEnumerable<AgentData> agentDatas, AgentData myData)
    {
        IEnumerable<Vector3> dangerousAgentPositions = agentDatas.Select((otherData) => (myData.position - otherData.position).normalized);
        Vector3 directionToFlee = NormalizedAverage(dangerousAgentPositions);
        ChooseAction(GetActionToMoveTo(myData, directionToFlee, FLEE_ANGLE));
        RotateIfBlocked();
        isUrgent = true;
    }

    private void RotateIfBlocked()
    {
        if (isBlocked && nextAction == Action.WALK)
            ChooseAction(sideToRotate);
    }

    private int Strength(AgentData agentData)
    {
        return (agentData.attack + agentData.weaponAttack) * agentData.energy;
    }

    private IEnumerable<AgentData> GetDangerousAgentDatas(IEnumerable<AgentData> otherDatas, AgentData myData)
    {
        return otherDatas.Where
            (
                (otherData) => 
                    otherData.weaponType == Weapon.Type.BOW &&
                    IsLookingToPosition(otherData, myData.position, DANGEROUS_BOW_ANGLE)
                    ||
                    otherData.weaponType != Weapon.Type.BOW &&
                    (otherData.position - myData.position).magnitude <= DANGEROUS_MELEE_DISTANCE
            );
    }

    private void CheckIfOutsideArea(AgentData myData)
    {
        if (myData.outsideShield && !isBlocked)
        {
            if (IsLookingToPosition(myData, Vector3.zero, 5))
                ChooseAction(Action.WALK);
            else
                ChooseAction(Random.Range(0, 5) == 1 ? Action.WALK : sideToRotate);
        }
    }

    private void CheckIfEatBerries(Perception perception, AgentData myData)
    {
        BushData bushData = perception.nearestBushData;
        if (bushData != null && perception.nearestBushData.hasBerries && !bushData.poisonous && myData.energy < Const.MAX_ENERGY)
        {
            ChooseAction(Action.EAT_BERRIES);
            if (myData.energy < Const.MAX_ENERGY * URGENT_ENERGY_TO_EAT_BERRIES)
                isUrgent = true;
        }
    }
}
