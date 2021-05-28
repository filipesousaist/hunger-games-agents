using static Agent;

public class DecisionTheoreticalDecider : Decider
{
    private DecisionTheoreticalModule decisionTheoreticalModule;

    private void Awake()
    {
        decisionTheoreticalModule = new DecisionTheoreticalModule(this);
    }

    public override void Decide(Perception perception)
    {
        decisionTheoreticalModule.Decide(perception);
    }

    public override string GetArchitectureName()
    {
        return "Decision Theoretical";
    }
}
