using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        SWORD, BOW
    }

    public WeaponType type;
    private int attack;
}
