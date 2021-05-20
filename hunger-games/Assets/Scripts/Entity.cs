using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public enum Type
    {
        AGENT, CHEST, BUSH, ROCK, TREE, ARROW, HAZARD_EFFECT
    }

    public abstract EntityData GetData();
}

public abstract class EntityData
{
    public Entity.Type type;
    public Vector3 position;
}
