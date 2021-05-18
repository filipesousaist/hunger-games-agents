using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public enum Type
    {
        NONE, SWORD, BOW
    }

    [ReadOnly] public int attack;

    public Vector3 POSITION_IN_AGENT;
    public Vector3 ROTATION_IN_AGENT;

    public abstract void Attack(Agent owner);

    public new abstract Type GetType();
}
