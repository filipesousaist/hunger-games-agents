using System.Linq;
using UnityEngine;

public class SimpleDecider : Decider
{
    private Agent.Action sideToRotate;
    public override Agent.Action Decide(Agent.Perception perception)
    {
        Agent.AgentData myData = perception.myData;

        Chest.ChestData chestData = perception.nearestChestData;
        if (chestData != null &&
            chestData.weaponType != Weapon.Type.NONE &&
            (myData.weaponType == Weapon.Type.NONE || chestData.weaponAttack > myData.weaponAttack))
            return Agent.Action.USE_CHEST;
               

        foreach (Entity.Data data in perception.visionData)
            if ((new Vector2(data.position.x, data.position.z) -
                new Vector2(myData.position.x, myData.position.z)
                ).magnitude <= 1.5)
            {
                return sideToRotate;
            }

        sideToRotate = Random.Range(0, 2) == 0 ? Agent.Action.ROTATE_RIGHT : Agent.Action.ROTATE_LEFT;
        return Agent.Action.WALK;
    }

    public override string GetArchitectureName()
    {
        return "Simple";
    }
}
