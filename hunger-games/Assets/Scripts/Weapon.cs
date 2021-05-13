using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        SWORD, BOW
    }

    public Type type;
    [ReadOnly] public int attack;

    private void Start()
    {
        attack = (type == Type.SWORD) ? Random.Range(2, 5) : Random.Range(1, 4);
    }
}
