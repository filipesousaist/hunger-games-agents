using System.Collections.Generic;
using UnityEngine;

public class VisionCollider : MonoBehaviour
{
    private List<Entity> collidingEntities;

    private void Awake()
    {
        collidingEntities = new List<Entity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponentInParent<Entity>();
        if (entity != null && !collidingEntities.Contains(entity))
            collidingEntities.Add(entity);
    }

    private void OnTriggerStay(Collider other)
    {
        Entity entity = other.GetComponentInParent<Entity>();
        if (entity != null && !collidingEntities.Contains(entity))
            collidingEntities.Add(entity);
    }

    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.GetComponentInParent<Entity>();
        if (entity != null)
            collidingEntities.Remove(entity);
    }

    public IEnumerable<Entity.Data> GetCollidingEntitiesData()
    {
        for (int i = collidingEntities.Count - 1; i >= 0; i--)
            if (collidingEntities[i] == null)
                collidingEntities.RemoveAt(i);

        return collidingEntities.ConvertAll((entity) => entity.GetData());
    }
}
