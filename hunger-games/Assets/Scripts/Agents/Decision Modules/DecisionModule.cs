using static Agent;

public abstract class DecisionModule
{
    private Decider decider;
    protected Action nextAction;
    public DecisionModule(Decider decider)
    {
        this.decider = decider;
    }

    public abstract void Decide(Perception perception);

    protected void ChooseAction(Action action, bool commit = true)
    {
        nextAction = action;
        if (commit)
            decider.nextAction = action;
    }
}

