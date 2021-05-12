using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        SWORD, BOW
    }

    public Type type;
    private int attack;
}
