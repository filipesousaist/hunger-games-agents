using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    public enum Type { FIRE, FOG, RAIN, RADIATION }
    public GameObject prefab;

    public float SPAWN_CHANCE;

    private List<Vector3> region;

    private readonly List<GameObject> effects = new List<GameObject>();

    public void SetRegion(List<Vector3> region)
    {
        this.region = region;
    }

    public void Activate()
    {
        foreach (Vector3 position in region)
            if (Random.Range(0f, 1f) <= SPAWN_CHANCE)
            {
                GameObject effect = Instantiate(prefab);
                effect.transform.position = position;
                effects.Add(effect);
            }
    }

    public void Stop()
    {
        foreach (GameObject effect in effects)
            Destroy(effect);
        effects.Clear();
    }
}
