using UnityEngine;
using System.Linq;
using static Entity;
using static Agent;

public class SimpleDecider : Decider
{
    private Action sideToRotate;
    public override void Decide(Perception perception)
    { 
        nextAction = Action.WALK; // If no other action is selected, walk

        AgentData myData = perception.myData;

        CheckIfRotate(perception, myData);

        CheckIfUseChest(perception, myData);

        CheckIfTrain(perception);

        CheckIfAttack(perception, myData);
    }

    private void CheckIfAttack(Perception perception, AgentData agentData)
    {
        if (agentData.weaponType == Weapon.Type.BOW && 
            perception.visionData.Any( (entityData) => entityData.type == Type.AGENT )
            ||
            agentData.weaponType != Weapon.Type.BOW &&
            perception.agentsInMeleeRange.Any())
            
            nextAction = Action.ATTACK;
    }

    private void CheckIfTrain(Perception perception)
    {
        if (perception.visionData.Any((data) => data.type == Type.ARROW))
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
            return;
        }
    }
    private void CheckIfEatBerries(Perception perception, AgentData myData)
    {
        BushData bushData = perception.nearestBushData;
        if (bushData != null && perception.nearestBushData.hasBerries && !bushData.poisonous && myData.energy < 10)
        {
            nextAction = Action.EAT_BERRIES;
            return;
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
        return "Simple";
    }
}
