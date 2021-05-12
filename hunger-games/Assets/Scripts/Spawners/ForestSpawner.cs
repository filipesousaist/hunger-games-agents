using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestSpawner : MonoBehaviour
{   
    private enum ForestObject
    {
        CHEST, ROCK, TREE, HEALTHY_BUSH, POISONOUS_BUSH
    }

    public int CHEST_AMOUNT;
    public int ROCK_AMOUNT;
    public int TREE_AMOUNT;
    public int BUSH_AMOUNT;
    public float HEALTHY_BUSH_RATIO;

    public float MIN_CHEST_DISTANCE;
    public float MIN_TREE_DISTANCE;
    public float MIN_DEFAULT_DISTANCE;

    public float SPAWN_RADIUS;

    public int MAX_TRIES;

    // Prefabs
    public GameObject chest;
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
        AMOUNT = CHEST_AMOUNT + ROCK_AMOUNT + TREE_AMOUNT + BUSH_AMOUNT;
        HEALTHY_BUSH_AMOUNT = Mathf.RoundToInt(BUSH_AMOUNT * HEALTHY_BUSH_RATIO);
        POISONOUS_BUSH_AMOUNT = BUSH_AMOUNT - HEALTHY_BUSH_AMOUNT;

        SpawnObjects();
    }

    private void SpawnObjects()
    {
        Vector3 centerPosition = Vector3.zero;
        List<Vector3> positions = new List<Vector3>();
        List<ForestObject> objects = GetShuffledObjects();

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

    private bool Collides(ForestObject obj, List<ForestObject> objects, Vector3 newPosition, List<Vector3> positions)
    {
        float newMinDistance = obj switch
        {
            ForestObject.CHEST => MIN_CHEST_DISTANCE,
            ForestObject.TREE => MIN_TREE_DISTANCE,
            _ => MIN_DEFAULT_DISTANCE
        };  

        for (int i = 0; i < positions.Count; i ++)
        {
            float minDistance = (objects[i] == ForestObject.TREE) ? MIN_TREE_DISTANCE : 0.5f;

            if ((positions[i] - newPosition).magnitude < minDistance + newMinDistance)
                return true;
        }
        return false;
    }

    private void CreateObject(ForestObject obj, Vector3 position)
    {
        GameObject prefab = obj switch
        {
            ForestObject.CHEST =>           chest,
            ForestObject.ROCK =>            rock,
            ForestObject.TREE =>            tree,
            ForestObject.HEALTHY_BUSH =>    healthyBush,
            _ =>                            poisonousBush
        };

        Quaternion rotation = Quaternion.Euler(0, random.Next(4) * 90, 0);
        Instantiate(prefab, position, rotation);
    }

    private List<ForestObject> GetShuffledObjects()
    {
        List<ForestObject> objects = new List<ForestObject>();
        Dictionary<ForestObject, int> amounts = new Dictionary<ForestObject, int>()
        {
            { ForestObject.CHEST, CHEST_AMOUNT },
            { ForestObject.ROCK, ROCK_AMOUNT },
            { ForestObject.TREE, TREE_AMOUNT },
            { ForestObject.HEALTHY_BUSH, HEALTHY_BUSH_AMOUNT },
            { ForestObject.POISONOUS_BUSH, POISONOUS_BUSH_AMOUNT }
        };

        foreach (int n in Utils.ShuffledArray(AMOUNT))
        {
            int current = 0;
            foreach (ForestObject obj in amounts.Keys)
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
