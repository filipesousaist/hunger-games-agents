using System.Collections.Generic;
using UnityEngine;

public class ControllableDecider : Decider
{
    private bool isControllable = false;

    private KeyCode lastKeyPressed = KeyCode.None;

    private readonly Dictionary<KeyCode, Agent.Action> keysToPress = new Dictionary<KeyCode, Agent.Action>() {
        { KeyCode.E,        Agent.Action.EAT_BERRIES },
        { KeyCode.R,        Agent.Action.USE_CHEST },
        { KeyCode.T,        Agent.Action.TRAIN },
        { KeyCode.Space,    Agent.Action.ATTACK }
    };
        

    private readonly Dictionary<KeyCode, Agent.Action> keysToHold = new Dictionary<KeyCode, Agent.Action>() {
        { KeyCode.A,            Agent.Action.ROTATE_LEFT },
        { KeyCode.LeftArrow,    Agent.Action.ROTATE_LEFT },
        { KeyCode.D,            Agent.Action.ROTATE_RIGHT },
        { KeyCode.RightArrow,   Agent.Action.ROTATE_RIGHT },
        { KeyCode.W,            Agent.Action.WALK },
        { KeyCode.UpArrow,      Agent.Action.WALK }
    };

    private void Update()
    {
        foreach (KeyCode keyCode in keysToPress.Keys)
            if (Input.GetKeyDown(keyCode))
            {
                lastKeyPressed = keyCode;
                return;
            }

        foreach (KeyCode keyCode in keysToHold.Keys)
            if (Input.GetKey(keyCode))
            {
                lastKeyPressed = keyCode;
                return;
            }
    }

    public override Agent.Action Decide()
    {
        KeyCode keyCode = lastKeyPressed;
        lastKeyPressed = KeyCode.None;
        if (isControllable && keyCode != KeyCode.None)
        {
            if (keysToHold.ContainsKey(keyCode))
                return keysToHold[keyCode];
            if (keysToPress.ContainsKey(keyCode))
                return keysToPress[keyCode];
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
