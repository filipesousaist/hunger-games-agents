public class Fire : Hazard
{
    public int ENERGY_LOSS;
    protected override void Harm(Agent agent)
    {
        agent.LoseEnergy(ENERGY_LOSS);
    }
}

