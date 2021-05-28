using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class VisionCollider : MonoBehaviour
{
    public Agent agent;
    private List<Entity> collidingEntities;

    private LayerMask LAYER_MASK;

    private void Awake()
    {
        collidingEntities = new List<Entity>();
    }

    public void InitLayerMask()
    {
        LAYER_MASK = LayerMask.GetMask("Default", "Arrow");

        for (int i = 1; i < Const.NUM_AGENTS + 1; i++)
            if (i != agent.index)
                LAYER_MASK |= LayerMask.GetMask("Agent " + i);
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

    public IEnumerable<EntityData> GetCollidingEntitiesData()
    {
        collidingEntities = collidingEntities.Where((entity) => entity != null).ToList();

        return collidingEntities.Where((entity) => CanSee(entity)).Select((entity) => entity.GetData());
    }

    private bool CanSee(Entity entity)
    {
        Vector3 position = agent.head.transform.position;
        Vector3 difference = entity.transform.position - position;

        RaycastHit[] hits = Physics.RaycastAll(position, difference.normalized, difference.magnitude, LAYER_MASK);

        //DrawDebugRays(entity, hits, position, difference);

        return hits.All(
            (hit) =>
            hit.transform.IsChildOf(entity.transform) ||
            hit.transform == entity.transform
        );
    }

    private void DrawDebugRays(Entity entity, RaycastHit[] hits, Vector3 position, Vector3 difference)
    {
        Color color = hits.All(
            (hit) =>
            hit.transform.IsChildOf(entity.transform) ||
            hit.transform == entity.transform
        ) ? Color.green : Color.red;

        Debug.DrawRay(position, difference, color);
        if (color == Color.green && agent.index == 1)
            Debug.Log(entity.name);
    }
}
