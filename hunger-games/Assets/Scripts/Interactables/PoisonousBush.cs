public class PoisonousBush : Bush
{
    public int ENERGY_LOSS;
    protected override void EatBerries(Agent agent)
    {
        agent.energy -= ENERGY_LOSS;
    }
}
