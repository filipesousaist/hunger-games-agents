using UnityEngine;
using static Agent;
using static Entity;
using System.Linq;

public static class DeciderUtils
{
    public static bool IsLookingToPosition(AgentData agentData, Vector3 position, float minAngle)
    {
        return Utils.CheckIfAligned(position - agentData.position, Utils.GetForward(agentData.rotation), minAngle);
    }

    public static Action GetRandomSide()
    {
        return Random.Range(0, 2) == 0 ? Action.ROTATE_RIGHT : Action.ROTATE_LEFT;
    }

    public static bool IsSeeingAny(Type type, Perception perception)
    {
        return perception.visionData.Any((data) => data.type == type);
    }

    public static Action GetActionToFlee(AgentData agentData, Vector3 directionToFlee, float minAngle)
    {
        Vector3 agentDirection = Utils.GetForward(agentData.rotation);
        if (Utils.CheckIfAligned(directionToFlee, agentDirection, minAngle))
            return Action.WALK;

        if (Vector3.SignedAngle(directionToFlee, agentDirection, Vector3.up) < 0)
            return Action.ROTATE_LEFT;
        return Action.ROTATE_RIGHT;
    }

    public static bool IsStrongerThan(AgentData agent1, AgentData agent2, System.Func<AgentData, int> Heuristic)
    {
        return Heuristic(agent1) - Heuristic(agent2) > 0;
    }
}

