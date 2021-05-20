using UnityEngine;

public class SimpleDecider : Decider
{
    private Agent.Action sideToRotate;
    public override void Decide(Agent.Perception perception)
    { 
        nextAction = Agent.Action.WALK; // If no other action is selected, walk

        AgentData myData = perception.myData;

        CheckIfRotate(perception, myData);

        CheckIfUseChest(perception, myData);

        CheckIfTrain(perception);
    }

    private void CheckIfTrain(Agent.Perception perception)
    {
        foreach (EntityData data in perception.visionData)
        {
            if (data.type == Entity.Type.ARROW)
            {
                nextAction = Agent.Action.TRAIN;
                return;
            }
        }
    }

    private void CheckIfUseChest(Agent.Perception perception, AgentData myData)
    {
        ChestData chestData = perception.nearestChestData;
        if (chestData != null &&
            chestData.weaponType != Weapon.Type.NONE &&
            (myData.weaponType == Weapon.Type.NONE || chestData.weaponAttack > myData.weaponAttack))
        {
            nextAction = Agent.Action.USE_CHEST;
            return;
        }
    }

    private void CheckIfRotate(Agent.Perception perception, AgentData myData)
    {
        foreach (EntityData data in perception.visionData)
            if ((new Vector2(data.position.x, data.position.z) -
                new Vector2(myData.position.x, myData.position.z)
                ).magnitude <= 1.5)
            {
                nextAction = sideToRotate;
                return;
            }

        sideToRotate = Random.Range(0, 2) == 0 ? Agent.Action.ROTATE_RIGHT : Agent.Action.ROTATE_LEFT;
    }

    public override string GetArchitectureName()
    {
        return "Simple";
    }
}
