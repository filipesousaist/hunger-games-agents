using System;
using UnityEngine;

public class MiddleSpawner : MonoBehaviour
{
    public int CHEST_AMOUNT;

    public int SPAWN_RADIUS;

    public GameObject chest;

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
        }
    }
}
