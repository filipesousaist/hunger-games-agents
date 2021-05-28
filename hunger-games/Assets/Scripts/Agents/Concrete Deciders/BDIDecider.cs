using static Agent;

public class BDIDecider : Decider
{
    private BDIModule bDIModule;

    private void Awake()
    {
        bDIModule = new BDIModule(this);
    }

    public override void Decide(Perception perception)
    {
        bDIModule.Decide(perception);
    }

    public override string GetArchitectureName()
    {
        return "BDI";
    }
}
