using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public int TREE_AMOUNT;
    
    public int SPAWN_RADIUS;

    public int MIN_DISTANCE;

    public int MAX_TRIES;
    
    public GameObject tree;
    
    private System.Random random = new System.Random();
    
    // Start is called before the first frame update
    void Start()
    {
        int tries;
        bool colliding;
        Vector3 newPosition;
        Vector3 centerPosition = new Vector3(0f, 0f, 0f);
        List<Vector3> positions = new List<Vector3>();
        GameObject newTree;
        
        for (int i = 0; i < TREE_AMOUNT; i++) {
            tries = MAX_TRIES;
            colliding = true;
            newPosition = new Vector3(random.Next(-245, 245), 0f, random.Next(-245, 245));

            while ((newPosition - centerPosition).magnitude < SPAWN_RADIUS) {
                newPosition = new Vector3(random.Next(-245, 245), 0f, random.Next(-245, 245));
            }
            
            while (colliding && tries > 0) {
                colliding = false;
                
                foreach (Vector3 position in positions) {
                    if ((position - newPosition).magnitude < MIN_DISTANCE) {
                        colliding = true;
                        break;
                    }
                }

                if (colliding) {
                    tries--;
                }
            }

            if (tries > 0) {
                positions.Add(newPosition);
                newTree = Instantiate(tree);
                newTree.transform.position = newPosition;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
