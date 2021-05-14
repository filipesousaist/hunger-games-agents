using UnityEngine;

public class Bow : Weapon
{
    void Start()
    {
        attack = Random.Range(1, 4);
    }

    public override void Attack(Agent owner)
    {
        throw new System.NotImplementedException();
    }
}
