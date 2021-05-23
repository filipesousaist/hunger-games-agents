using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeCollider : MonoBehaviour
{
    private List<Agent> collidingAgents;

    private void Awake()
    {
        collidingAgents = new List<Agent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Agent agent = other.GetComponentInParent<Agent>();
        if (agent != null && !collidingAgents.Contains(agent))
            collidingAgents.Add(agent);
    }

    private void OnTriggerStay(Collider other)
    {
        Agent agent = other.GetComponentInParent<Agent>();
        if (agent != null && !collidingAgents.Contains(agent))
            collidingAgents.Add(agent);
    }

    private void OnTriggerExit(Collider other)
    {
        Agent agent = other.GetComponentInParent<Agent>();
        if (agent != null)
            collidingAgents.Remove(agent);
    }

    public IEnumerable<Agent> GetCollidingAgents()
    {
        collidingAgents = collidingAgents.Where((entity) => entity != null).ToList();
        return collidingAgents;
    }
}
