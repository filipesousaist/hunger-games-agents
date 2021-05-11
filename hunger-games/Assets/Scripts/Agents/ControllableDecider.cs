using UnityEngine;

public class ControllableDecider : Decider
{
    private bool isControllable = false;
    public override Agent.Action Decide()
    {
        if (isControllable)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                return Agent.Action.WALK;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                return Agent.Action.ROTATE_LEFT;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                return Agent.Action.ROTATE_RIGHT;
        }
        return Agent.Action.IDLE;
    }

    public override void SetControllable(bool controllable)
    {
        isControllable = controllable;
    }

    public override string GetArchitectureName()
    {
        return "Controllable";
    }
}
