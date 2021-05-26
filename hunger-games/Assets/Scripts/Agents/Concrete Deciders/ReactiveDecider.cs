using static Agent;

public class ReactiveDecider : Decider
{
    private ReactiveModule reactiveModule;

    private void Awake()
    {
        reactiveModule = new ReactiveModule(this);
    }

    public override void Decide(Perception perception)
    {
        reactiveModule.Decide(perception);
    }

    public override string GetArchitectureName()
    {
        return "Reactive";
    }
}
