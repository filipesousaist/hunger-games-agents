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

    public static Action GetActionToMoveTo(AgentData agentData, Vector3 direction, float minAngle)
    {
        Vector3 agentDirection = Utils.GetForward(agentData.rotation);
        if (Utils.CheckIfAligned(direction, agentDirection, minAngle))
            return Action.WALK;

        if (Vector3.SignedAngle(direction, agentDirection, Vector3.up) < 0)
            return Action.ROTATE_LEFT;
        return Action.ROTATE_RIGHT;
    }

    public static bool IsStrongerThan(AgentData agent1, AgentData agent2, System.Func<AgentData, int> Heuristic)
    {
        return Heuristic(agent1) > Heuristic(agent2);
    }

    public static bool IsInPositionToAttack(Perception perception, AgentData attacker, AgentData defender, float bowMinAngle)
    {
        if (attacker.weaponType == Weapon.Type.BOW)
            return IsLookingToPosition(attacker, defender.position, bowMinAngle);

        return perception.agentsInMeleeRange.Select((otherData) => otherData.index).Contains(defender.index);
    }

    /// <summary>
    /// Returns the best action to position agent "attacker" in order to attack agent "defender".
    /// </summary>
    public static Action GetActionToPosition(AgentData attacker, AgentData defender, float meleeMinAngle)
    {
        Vector3 desiredDirection = defender.position - attacker.position;

        if (attacker.weaponType == Weapon.Type.BOW)
        {
            if (Vector3.SignedAngle(Utils.GetForward(attacker.rotation), desiredDirection, Vector3.up) < 0)
                return Action.ROTATE_LEFT;
            return Action.ROTATE_RIGHT;
        }

        return GetActionToMoveTo(attacker, desiredDirection, meleeMinAngle);
    }
}

