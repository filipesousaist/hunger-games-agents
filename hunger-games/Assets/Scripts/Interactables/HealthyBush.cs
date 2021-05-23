using UnityEngine;

public class HealthyBush : Bush
{
    public int ENERGY_GAIN;
    protected override void EatBerries(Agent agent)
    {
        agent.GainEnergy(ENERGY_GAIN);
    }

    protected override bool IsPoisonous()
    {
        return Random.Range(0, 5) < 0; // 80% chance of returning wrong value
    }
}
