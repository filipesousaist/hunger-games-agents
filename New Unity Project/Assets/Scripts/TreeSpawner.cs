using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public int TREE_AMOUNT;
    
    public GameObject tree;
    
    private System.Random random = new System.Random();
    
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < TREE_AMOUNT; i++) {
            GameObject newTree = Instantiate(tree);

            newTree.transform.position = new Vector3(random.Next(-245, 245), 0f, random.Next(-245, 245));
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
