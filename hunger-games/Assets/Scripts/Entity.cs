using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public enum Type
    {
        AGENT, CHEST, BUSH, ROCK, TREE, ARROW, HAZARD
    }

    public abstract class Data
    {
        public Type type;
        public Vector3 position;
    }

    public abstract Data GetData();
}
