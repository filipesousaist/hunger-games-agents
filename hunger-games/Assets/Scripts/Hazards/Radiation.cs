using UnityEngine;

public class Radiation : Hazard
{
    public int ATTACK_LOSS;
    protected override void Harm(Agent agent)
    {
        agent.attack = Mathf.Max(agent.attack - ATTACK_LOSS, agent.MIN_ATTACK);
        agent.UpdateInfo();
    }
}

