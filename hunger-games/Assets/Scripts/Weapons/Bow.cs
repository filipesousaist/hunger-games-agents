using UnityEngine;

public class Bow : Weapon
{
    public GameObject fixedArrow;
    public GameObject arrowPrefab;
    void Start()
    {
        attack = Random.Range(1, 4);
    }

    public override void Attack(Agent owner)
    {
        Arrow newArrow = Instantiate(arrowPrefab, transform.position, transform.parent.rotation).GetComponent<Arrow>();
        newArrow.SetDamage(attack);
        newArrow.SetOwner(owner);
        fixedArrow.SetActive(false);
    }

    public  override Type GetType()
    {
        return Type.BOW;
    }
}
