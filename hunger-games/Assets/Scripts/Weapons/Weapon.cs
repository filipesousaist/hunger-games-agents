using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public enum Type
    {
        NONE, SWORD, BOW
    }

    public int MIN_ATTACK;
    public int MAX_ATTACK;

    [ReadOnly] public int attack;

    public Vector3 POSITION_IN_AGENT;
    public Vector3 ROTATION_IN_AGENT;

    private void Start()
    {
        attack = Random.Range(MIN_ATTACK, MAX_ATTACK + 1);
    }
    public void Corrode(int attackLoss)
    {
        attack = Mathf.Max(attack - attackLoss, MIN_ATTACK);
    }

    public abstract void Attack(Agent owner);

    public new abstract Type GetType();
}
