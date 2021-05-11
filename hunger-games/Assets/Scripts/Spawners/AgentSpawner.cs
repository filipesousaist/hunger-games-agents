using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
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
        float angleDeg = 360 / AGENT_AMOUNT;
        double angleRad = Math.PI * 2 / AGENT_AMOUNT;

        int[] indexes = Utils.ShuffledArray(AGENT_AMOUNT);

        foreach (int i in indexes) {
            GameObject newAgent = Instantiate(agent);

            newAgent.name = "Agent " + (i + 1) + " (" + bodyMaterials[i].name + ")";

            newAgent.transform.position = new Vector3(
                (float) Math.Cos(angleRad * i) * SPAWN_RADIUS,
                0,
                (float) Math.Sin(angleRad * i) * SPAWN_RADIUS);
            newAgent.transform.Rotate(0, 270 - angleDeg * i, 0);

            newAgent.transform.Find("Head").GetComponent<MeshRenderer>().material = headMaterials[rnd.Next(0, 3)];
            newAgent.transform.Find("Body").GetComponent<MeshRenderer>().material = bodyMaterials[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
