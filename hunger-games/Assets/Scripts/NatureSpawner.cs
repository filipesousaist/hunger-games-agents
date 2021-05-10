using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureSpawner : MonoBehaviour
{
    
    private enum NatureObject
    {
        ROCK, TREE, HEALTHY_BUSH, POISONOUS_BUSH
    }

    public int ROCK_AMOUNT;
    public int TREE_AMOUNT;
    public int BUSH_AMOUNT;
    public float HEALTHY_BUSH_RATIO;
    
    public float MIN_TREE_DISTANCE;
    public float SPAWN_RADIUS;

    public int MAX_TRIES;

    // Prefabs
    public GameObject rock;
    public GameObject tree;
    public GameObject healthyBush;
    public GameObject poisonousBush;

    private int AMOUNT;
    private int HEALTHY_BUSH_AMOUNT;
    private int POISONOUS_BUSH_AMOUNT;

    private readonly System.Random random = new System.Random();

    // Start is called before the first frame update
    private void Start()
    {
        AMOUNT = ROCK_AMOUNT + TREE_AMOUNT + BUSH_AMOUNT;
        HEALTHY_BUSH_AMOUNT = Mathf.RoundToInt(BUSH_AMOUNT * HEALTHY_BUSH_RATIO);
        POISONOUS_BUSH_AMOUNT = BUSH_AMOUNT - HEALTHY_BUSH_AMOUNT;

        SpawnObjects();
    }

    private void SpawnObjects()
    {
        Vector3 centerPosition = Vector3.zero;
        List<Vector3> positions = new List<Vector3>();
        List<NatureObject> objects = GetShuffledObjects();

        for (int i = 0; i < AMOUNT; i++)
        {
            int tries = MAX_TRIES;
            bool colliding = true;

            while (colliding && tries > 0)
            {
                Vector3 newPosition;
                do
                    newPosition = new Vector3(random.Next(-245, 245), 0f, random.Next(-245, 245));
                while ((newPosition - centerPosition).magnitude < SPAWN_RADIUS);

                if (Collides(objects[i], objects, newPosition, positions))
                    tries--;
                else
                {
                    colliding = false;
                    positions.Add(newPosition);
                    CreateObject(objects[i], newPosition);
                }
            }
        }
    }

    private bool Collides(NatureObject obj, List<NatureObject> objects, Vector3 newPosition, List<Vector3> positions)
    {
        float newMinDistance = obj switch
        {
            NatureObject.TREE => MIN_TREE_DISTANCE,
            _ => 1
        };

        for (int i = 0; i < positions.Count; i ++)
        {
            float minDistance = objects[i] switch
            {
                NatureObject.TREE => MIN_TREE_DISTANCE,
                _ => 1
            };
            if ((positions[i] - newPosition).magnitude < minDistance + newMinDistance)
                return true;
        }
        return false;
    }

    private void CreateObject(NatureObject obj, Vector3 position)
    {
        GameObject prefab = obj switch
        {
            NatureObject.ROCK =>            rock,
            NatureObject.TREE =>            tree,
            NatureObject.HEALTHY_BUSH =>    healthyBush,
            _ =>                            poisonousBush
        };
        Instantiate(prefab, position, Quaternion.identity);
    }

    private List<NatureObject> GetShuffledObjects()
    {
        List<NatureObject> objects = new List<NatureObject>();
        Dictionary<NatureObject, int> amounts = new Dictionary<NatureObject, int>()
        {
            { NatureObject.ROCK, ROCK_AMOUNT },
            { NatureObject.TREE, TREE_AMOUNT },
            { NatureObject.HEALTHY_BUSH, HEALTHY_BUSH_AMOUNT },
            { NatureObject.POISONOUS_BUSH, POISONOUS_BUSH_AMOUNT }
        };

        foreach (int n in Utils.ShuffledArray(AMOUNT))
        {
            int current = 0;
            foreach (NatureObject obj in amounts.Keys)
            {
                current += amounts[obj];
                if (n < current)
                {
                    objects.Add(obj);
                    break;
                }
            }
        }

        return objects;
    }
}
