using UnityEngine;

public class Sword : Weapon
{
    void Start()
    {
        attack = Random.Range(2, 5);
    }

    public override void Attack(Agent owner)
    {
        foreach (Agent agent in owner.meleeCollider.GetCollidingAgents())
            agent.LoseEnergy(owner.attack);
    }
}
