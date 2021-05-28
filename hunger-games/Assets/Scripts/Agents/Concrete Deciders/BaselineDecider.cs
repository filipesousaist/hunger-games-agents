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
        if (agentData.attackWaitTimer == 0)
        {
            IEnumerable<EntityData> otherAgents =
                perception.visionData.Where((entityData) => entityData.type == Type.AGENT );
            if (agentData.weaponType == Weapon.Type.BOW && 
                otherAgents.Any
                (
                    (otherAgentData) => DeciderUtils.IsLookingToPosition(agentData, otherAgentData.position, 2)
                )
                ||
                agentData.weaponType != Weapon.Type.BOW &&
                perception.agentsInMeleeRange.Any())

                nextAction = Action.ATTACK;
        }
    }

    private void CheckIfTrain(Perception perception)
    {
        if (perception.myData.energy >= 0.5 * Const.MAX_ENERGY && perception.myData.attack < Const.MAX_ATTACK)
            nextAction = Action.TRAIN;
    }

    private void CheckIfUseChest(Perception perception, AgentData myData)
    {
        ChestData chestData = perception.nearestChestData;
        if (chestData != null)
        {
            if (chestData.state == Chest.State.OPENING)
                nextAction = Action.IDLE; // Wait for chest opening
            else if (chestData.weaponType != Weapon.Type.NONE &&
                    (myData.weaponType == Weapon.Type.NONE || chestData.weaponAttack > myData.weaponAttack) ||
                     chestData.state == Chest.State.CLOSED)
                nextAction = Action.USE_CHEST;
        }
    }

    private void CheckIfEatBerries(Perception perception, AgentData myData)
    {
        BushData bushData = perception.nearestBushData;
        if (bushData != null && perception.nearestBushData.hasBerries && !bushData.poisonous && myData.energy < Const.MAX_ENERGY)
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
                    sideToRotate = DeciderUtils.GetRandomSide();
                    isBlocked = true;
                }
                
                nextAction = sideToRotate;
                return;
            }

        isBlocked = false;
    }

    private void CheckIfOutsideArea(Perception perception, AgentData myData)
    {
        if ((Mathf.Abs(myData.position.x) > perception.shieldRadius ||
             Mathf.Abs(myData.position.z) > perception.shieldRadius)
            && !isBlocked)
        {
            if (DeciderUtils.IsLookingToPosition(myData, Vector3.zero, 5))
                nextAction = Action.WALK;
            else
                nextAction = Random.Range(0, 5) == 1 ? Action.WALK : sideToRotate;
        }
    }

    public override string GetArchitectureName()
    {
        return "Baseline";
    }
}
