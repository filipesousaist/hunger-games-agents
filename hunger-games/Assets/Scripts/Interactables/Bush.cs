using UnityEngine;

public abstract class Bush : Interactable
{
    public GameObject otherPrefab;
    public bool hasBerries;

    public override void Interact(Agent agent)
    {
        if (hasBerries)
        {
            EatBerries(agent);
            agent.UpdateInfo();

            GameObject newBush = Instantiate(otherPrefab);
            newBush.transform.position = transform.position;

            agent.interactionCollider.RemoveBush(this);
            Destroy(gameObject);
        }
    }

    protected abstract void EatBerries(Agent agent);
}
