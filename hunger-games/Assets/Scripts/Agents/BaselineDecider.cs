using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Entity;
using static Agent;

public class BaselineDecider : Decider
{
    private Action sideToRotate;
    private bool isBlocked = false;
    public override void Decide(Perception perception)
    { 
        nextAction = Action.WALK; // If no other action is selected, walk
        
        AgentData myData = perception.myData;

        CheckIfRotate(perception, myData);
        
        CheckIfTrain(perception); //only trains if more than 25% of the MAX energy and didnt achieved the MAX_ATTACK

        CheckIfUseChest(perception, myData);

        CheckIfAttack(perception, myData);

        CheckIfOutsideArea(perception, myData);

        CheckIfEatBerries(perception, myData); //eats berries he believes aren't poisonous
    }

    private void CheckIfAttack(Perception perception, AgentData agentData)
    {
        if(agentData.attackWaitTimer==0)
        {
            IEnumerable<EntityData> otherAgents =
                perception.visionData.Where((entityData) => entityData.type == Type.AGENT );
            if (agentData.weaponType == Weapon.Type.BOW && 
                otherAgents.Any((otherAgentData) => Utils.CheckIfAligned(agentData.position-otherAgentData.position, Utils.GetForward(((AgentData) otherAgentData).rotation), 2))||
                agentData.weaponType != Weapon.Type.BOW &&
                perception.agentsInMeleeRange.Any())

                nextAction = Action.ATTACK;
        }
    }

    private void CheckIfTrain(Perception perception)
    {
        if (perception.myData.energy >= 0.25*Const.MAX_ENERGY && perception.myData.attack < Const.MAX_ATTACK)
            nextAction = Action.TRAIN;
    }

    private void CheckIfUseChest(Perception perception, AgentData myData)
    {
        ChestData chestData = perception.nearestChestData;
        if (chestData != null &&
            chestData.weaponType != Weapon.Type.NONE &&
            (myData.weaponType == Weapon.Type.NONE || chestData.weaponAttack > myData.weaponAttack))
        {
            nextAction = Action.USE_CHEST;
        }
    }
    private void CheckIfEatBerries(Perception perception, AgentData myData)
    {
        BushData bushData = perception.nearestBushData;
        if (bushData != null && perception.nearestBushData.hasBerries && !bushData.poisonous && myData.energy < 10)
        {
            nextAction = Action.EAT_BERRIES;
        }
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
                    sideToRotate = Random.Range(0, 2) == 0 ? Action.ROTATE_RIGHT : Action.ROTATE_LEFT;
                    isBlocked = true;
                }
                
                nextAction = sideToRotate;
                return;
            }

        isBlocked = false;
    }

    private void CheckIfOutsideArea(Perception perception, AgentData agentData)
    {
        if (agentData.outsideShield && !isBlocked)
        {
            if (!Utils.CheckIfAligned(-agentData.position, Utils.GetForward(agentData.rotation), 5))
            {
                nextAction = Random.Range(0, 5) == 1 ? Action.WALK : sideToRotate;
            } else
            {
                nextAction = Action.WALK;
            }
        }
    }

    public override string GetArchitectureName()
    {
        return "Baseline Reactive (Aggressive)";
    }
}
