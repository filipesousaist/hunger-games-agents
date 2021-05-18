using UnityEngine;

public class PoisonousBush : Bush
{
    public int ENERGY_LOSS;
    protected override void EatBerries(Agent agent)
    {
        agent.LoseEnergy(ENERGY_LOSS);
    }

    protected override bool IsPoisonous()
    {
        return Random.Range(0, 5) > 0; // 80% chance of returning wrong value
    }
}
