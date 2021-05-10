using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    public int CHEST_AMOUNT;

    public int SPAWN_RADIUS;

    public GameObject chest;
    public GameObject sword;
    public GameObject bow;

    private readonly System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < CHEST_AMOUNT; i ++)
        {
            float angleDeg = 360 / CHEST_AMOUNT;
            double angleRad = Math.PI * 2 / CHEST_AMOUNT;
            GameObject newChest = Instantiate(chest);

            newChest.transform.position = new Vector3(
                (float) Math.Cos(angleRad * i) * SPAWN_RADIUS, 
                0,
                (float) Math.Sin(angleRad * i) * SPAWN_RADIUS);
            newChest.transform.Rotate(0, -angleDeg * i, 0);

            GameObject prefab = random.Next(2) == 0 ? sword : bow;
            GameObject newWeapon = Instantiate(prefab, newChest.transform.position + Vector3.up * 0.3f, Quaternion.Euler(new Vector3(0, -45, 90)));
            newWeapon.transform.Rotate(new Vector3(0, 0, 45), Space.Self);

            newChest.GetComponent<Chest>().SetWeapon(newWeapon.GetComponent<Weapon>());
        }
    }
}
