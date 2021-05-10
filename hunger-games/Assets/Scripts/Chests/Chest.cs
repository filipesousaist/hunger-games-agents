using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public float DISPLAY_WEAPON_SPEED;
    public float DISPLAY_WEAPON_HEIGHT;

    private Weapon currentWeapon;
    private float weaponOriginalHeight;

    private Coroutine displayWeaponCo;
    private Coroutine hideWeaponCo;


    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        weaponOriginalHeight = weapon.transform.position.y;
    }

    public void DisplayWeapon()
    {
        displayWeaponCo = StartCoroutine(DisplayWeaponCo());
    }

    public void HideWeapon()
    {
        hideWeaponCo = StartCoroutine(HideWeaponCo());
    }

    private IEnumerator DisplayWeaponCo()
    {
        if (hideWeaponCo != null)
            StopCoroutine(hideWeaponCo);

        Transform weaponTransform = currentWeapon.transform;
        while (weaponTransform.position.y < DISPLAY_WEAPON_HEIGHT)
        {
            float distance = Mathf.Min(DISPLAY_WEAPON_SPEED * Time.deltaTime, DISPLAY_WEAPON_HEIGHT - weaponTransform.position.y);
            weaponTransform.position += Vector3.up * distance;
            yield return null;
        }
    }

    public IEnumerator HideWeaponCo()
    {
        if (displayWeaponCo != null)
            StopCoroutine(displayWeaponCo);

        Transform weaponTransform = currentWeapon.transform;
        while (weaponTransform.position.y > weaponOriginalHeight)
        {
            float distance = Mathf.Min(DISPLAY_WEAPON_SPEED * Time.deltaTime, weaponTransform.position.y - weaponOriginalHeight);
            weaponTransform.position += Vector3.down * distance;
            yield return null;
        }
    }
}
