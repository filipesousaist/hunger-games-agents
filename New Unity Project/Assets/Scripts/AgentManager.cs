using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public int AGENT_AMOUNT;

    public int SPAWN_RADIUS;
    
    // Prefabs

    public GameObject agent;

    public Material[] headMaterials;

    public Material[] bodyMaterials;

    // Start is called before the first frame update
    void Start() {
        System.Random rnd = new System.Random();
        double angle = Math.PI * 2 / AGENT_AMOUNT;

        int[] indexes = Utils.ShuffledArray(AGENT_AMOUNT);

        foreach (int i in indexes) {
            GameObject newAgent = Instantiate(agent);

            newAgent.transform.position = new Vector3((float) Math.Cos(angle * i) * SPAWN_RADIUS,0.5f,
                (float) Math.Sin(angle * i) * SPAWN_RADIUS);

            newAgent.transform.Find("Head").GetComponent<MeshRenderer>().material = headMaterials[i % 3];
            newAgent.transform.Find("Body").GetComponent<MeshRenderer>().material = bodyMaterials[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
