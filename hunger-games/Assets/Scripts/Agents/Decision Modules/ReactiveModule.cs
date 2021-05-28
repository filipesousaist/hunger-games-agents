using System.Collections.Generic;
using static Agent;
using static Entity;
using static DeciderUtils;
using static Utils;
using UnityEngine;
using System.Linq;
using static Const;

public class ReactiveModule : DecisionModule
{
    public bool isUrgent;

    //private Action sideToRotate;
    //private bool isBlocked = false;
    private Vector3 directionToFlee;
    

    private const float MIN_DISTANCE_TO_AVOID_OBSTACLE = 1.5f;

    private const int FLEE_ANGLE = 15;
    private const int MELEE_MIN_ANGLE = 30;
    private const float URGENT_ENERGY_TO_EAT_BERRIES = 0.2f; // Fraction of max health


    public ReactiveModule(Decider decider) : base(decider) {}

    public override void Decide(Perception perception)
    {
        isUrgent = false;
        ChooseAction(Action.WALK); // If no other action is selected, walk

        AgentData myData = perception.myData;

        directionToFlee = Vector3.zero;

        CheckIfRotate(perception, myData);

        CheckIfTrain(myData); //only trains if more than 25% of the MAX energy and didnt achieved the MAX_ATTACK

        CheckIfUseChest(perception, myData);

        CheckOtherAgents(perception, myData);

        CheckIfFleeFromHazard(perception, myData);

        CheckIfOutsideArea(perception, myData);

        CheckIfEatBerries(perception, myData); //eats berries he believes aren't poisonous

        if (directionToFlee != Vector3.zero)
            ChooseAction(GetActionToMoveTo(myData, directionToFlee, FLEE_ANGLE));
    }

    private void CheckIfRotate(Perception perception, AgentData myData)
    {
        foreach (EntityData data in perception.visionData)
        {
            Vector3 difference = new Vector3(myData.position.x - data.position.x, 0, myData.position.z - data.position.z);

            if (data.type != Type.HAZARD_EFFECT && 
                difference.magnitude <= MIN_DISTANCE_TO_AVOID_OBSTACLE)
            {
                directionToFlee += difference.normalized;
            }
        }
    }

    private void CheckIfTrain(AgentData myData)
    {
        AgentData myDataAfterTrain = (AgentData) myData.Clone();
        myDataAfterTrain.energy = Mathf.Max(myData.energy - Const.TRAIN_ENERGY_LOSS, 0);
        myDataAfterTrain.attack = Mathf.Min(myData.attack + Const.TRAIN_ATTACK_GAIN, Const.MAX_ATTACK);
        if (Strength(myDataAfterTrain) >= 1.1f * Strength(myData))
            ChooseAction(Action.TRAIN);
    }

    private void CheckIfUseChest(Perception perception, AgentData myData)
    {
        ChestData chestData = perception.nearestChestData;
        if (chestData != null) {
            if (chestData.state == Chest.State.OPENING)
                ChooseAction(Action.IDLE); // Wait for chest opening
            else if (chestData.weaponType != Weapon.Type.NONE &&
                    (myData.weaponType == Weapon.Type.NONE || chestData.weaponAttack > myData.weaponAttack) ||
                     chestData.state == Chest.State.CLOSED)
                ChooseAction(Action.USE_CHEST);
        }
    }

    private void CheckOtherAgents(Perception perception, AgentData myData)
    {
        IEnumerable<AgentData> otherDatas = perception.visionData.Where((data) => data.type == Type.AGENT).Select((data) => (AgentData) data);

        if (otherDatas.Any())
        {
            // If I have low energy, and others are able to attack me, flee
            IEnumerable<AgentData> dangerousAgentDatas = DeciderUtils.GetDangerousAgentDatas(otherDatas, myData);
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
                    ChooseAction(myData.attackWaitTimer == 0 ? Action.ATTACK : Action.IDLE);
                    return;
                }

            // Else, try to reposition
            ChooseAction(GetActionToPosition(myData, otherDatas.First(), MELEE_MIN_ANGLE));
        }
    }

    private void FleeFromAgents(IEnumerable<AgentData> agentDatas, AgentData myData)
    {
        IEnumerable<Vector3> dangerousAgentDirections = agentDatas.Select((otherData) => (myData.position - otherData.position).normalized);

        foreach (Vector3 direction in dangerousAgentDirections)
            directionToFlee += direction;

        isUrgent = true;
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

            isUrgent = true;
        }
        
    }

    private void CheckIfOutsideArea(Perception perception, AgentData myData)
    {
        if ((Mathf.Abs(myData.position.x) > perception.shieldRadius ||
             Mathf.Abs(myData.position.z) > perception.shieldRadius))
        {
            directionToFlee -= myData.position.normalized;
            isUrgent = true;
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
