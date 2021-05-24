using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Entity;
using static Agent;

public class BaselineDecider : Decider
{
    private Action sideToRotate;
    public override void Decide(Perception perception)
    { 
        nextAction = Action.WALK; // If no other action is selected, walk
        
        AgentData myData = perception.myData;

        CheckIfRotate(perception, myData);
        
        CheckIfTrain(perception); //only trains if more than 25% of the MAX energy and didnt achieved the MAX_ATTACK

        CheckIfUseChest(perception, myData);
       
        CheckIfEatBerries(perception, myData); //eats berries he believes aren't poisonous
        
        CheckIfAttack(perception, myData);
    }

    private void CheckIfAttack(Perception perception, AgentData agentData)
    {
        if(agentData.attackWaitTimer==0)
        {
            IEnumerable<EntityData> otherAgents =
                perception.visionData.Where((entityData) => entityData.type == Type.AGENT );
            if (agentData.weaponType == Weapon.Type.BOW && 
                otherAgents.Any((otherAgentData) => CheckIfArrowIntersects(agentData, (AgentData) otherAgentData))||
                agentData.weaponType != Weapon.Type.BOW &&
                perception.agentsInMeleeRange.Any())

                nextAction = Action.ATTACK;
        }
    }

    private bool CheckIfArrowIntersects(AgentData myData, AgentData otherData)
    {
        return Mathf.Abs(Vector3.Angle(myData.position-otherData.position, Utils.GetForward(myData.rotation))) <= 2 ;
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
                nextAction = sideToRotate;
                return;
            }

        sideToRotate = Random.Range(0, 2) == 0 ? Action.ROTATE_RIGHT : Action.ROTATE_LEFT;
    }

    public override string GetArchitectureName()
    {
        return "Baseline Reactive (Aggressive)";
    }
}
