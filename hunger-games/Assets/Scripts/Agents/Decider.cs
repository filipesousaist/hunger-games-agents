using UnityEngine;
public abstract class Decider : MonoBehaviour
{
    /// <summary>
    /// The agent should store in this variable the action to do next.
    /// </summary>
    public Agent.Action nextAction;

    /// <summary>
    /// Agent's decision algorithm.
    /// </summary>
    /// <param name="perception">The information the agent got from the environment.</param>
    public abstract void Decide(Agent.Perception perception);

    public virtual void SetControllable(bool controllable) { }

    public abstract string GetArchitectureName();
}