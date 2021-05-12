public class HealthyBush : Bush
{
    public int ENERGY_GAIN;
    protected override void EatBerries(Agent agent)
    {
        agent.energy += ENERGY_GAIN;
    }
}
