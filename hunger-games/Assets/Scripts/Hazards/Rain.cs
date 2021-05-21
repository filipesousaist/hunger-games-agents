public class Rain : Hazard
{
    public int ATTACK_LOSS;

    protected override void Harm(Agent agent)
    {
        agent.CorrodeWeapon(ATTACK_LOSS);
    }
}

