using UnityEngine;
public abstract class Decider : MonoBehaviour
{
    // Decision algorithm to override
    public abstract Agent.Action Decide(Agent.Perception perception);

    // Method to control agents
    public virtual void SetControllable(bool controllable) { }

    public abstract string GetArchitectureName();
}