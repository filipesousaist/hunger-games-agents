using System.Collections.Generic;
using UnityEngine;

public class AgentInteractionCollider : MonoBehaviour
{
    public Agent agent;

    private List<Chest> collidingChests;
    private List<Bush> collidingBushes;

    private void Awake()
    {
        collidingChests = new List<Chest>();
        collidingBushes = new List<Bush>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Chest chest = other.GetComponentInParent<Chest>();
        if (chest != null)
        {
            chest.AddInteractionCollider(this);
            if (!collidingChests.Contains(chest))
                collidingChests.Add(chest);
        }

        Bush bush = other.GetComponentInParent<Bush>();
        if (bush != null && !collidingBushes.Contains(bush))
            collidingBushes.Add(bush);
    }

    private void OnTriggerStay(Collider other)
    {
        Chest chest = other.GetComponentInParent<Chest>();
        if (chest != null)
        {
            chest.AddInteractionCollider(this);
            if (!collidingChests.Contains(chest))
                collidingChests.Add(chest);
        }

        Bush bush = other.GetComponentInParent<Bush>();
        if (bush != null && !collidingBushes.Contains(bush))
            collidingBushes.Add(bush);
    }

    private void OnTriggerExit(Collider other)
    {
        Chest chest = other.GetComponentInParent<Chest>();
        if (chest != null)
        {
            collidingChests.Remove(chest);
            chest.RemoveInteractionCollider(this);
        }

        Bush bush = other.GetComponentInParent<Bush>();
        if (bush != null)
            collidingBushes.Remove(bush);
    }

    public Chest GetNearestChest(Vector3 position)
    {
        if (collidingChests.Count == 0)
            return null;

        Chest nearestChest = collidingChests[0];
        float minSqrDistance = (position - nearestChest.transform.position).sqrMagnitude;

        for (int i = 1; i < collidingChests.Count; i ++)
        {
            float sqrDistance = (position - collidingChests[i].transform.position).magnitude;
            if (sqrDistance < minSqrDistance)
            {
                nearestChest = collidingChests[i];
                minSqrDistance = sqrDistance;
            }
        }

        return nearestChest;
    }

    public Bush GetNearestBush(Vector3 position)
    {
        if (collidingBushes.Count == 0)
            return null;

        Bush nearestBush = collidingBushes[0];
        float minSqrDistance = (position - nearestBush.transform.position).sqrMagnitude;

        for (int i = 1; i < collidingBushes.Count; i++)
        {
            float sqrDistance = (position - collidingBushes[i].transform.position).magnitude;
            if (sqrDistance < minSqrDistance)
            {
                nearestBush = collidingBushes[i];
                minSqrDistance = sqrDistance;
            }
        }

        return nearestBush;
    }

    public void RemoveBush(Bush bush)
    {
        collidingBushes.Remove(bush);
    }

    public void OnDestroy()
    {
        foreach (Chest chest in collidingChests)
            chest.RemoveInteractionCollider(this);
    }
}
