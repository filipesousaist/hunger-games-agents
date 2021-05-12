public class SimpleDecider : Decider
{
    private readonly System.Random random = new System.Random();
    public override Agent.Action Decide()
    {
        int r = random.Next(10);
        if (r == 0)
            return Agent.Action.IDLE;
        else if (r <= 3)
            return Agent.Action.ROTATE_LEFT;
        //else if (r <= 4)
        //    return Agent.Action.ROTATE_RIGHT;
        else
            return Agent.Action.WALK;
    }

    public override string GetArchitectureName()
    {
        return "Simple";
    }
}
