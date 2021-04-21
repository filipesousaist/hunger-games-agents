using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public int AGENT_AMOUNT;

    public int SPAWN_RADIUS;
    
    public GameObject agent;

    // Start is called before the first frame update
    void Start() {
        double angle = Math.PI * 2 / AGENT_AMOUNT;
        
        for (int i = 0; i < AGENT_AMOUNT; i++) {
            GameObject newAgent = Instantiate(agent);

            newAgent.transform.position = new Vector3((float) Math.Cos(angle * i) * SPAWN_RADIUS,0.5f,
                (float) Math.Sin(angle * i) * SPAWN_RADIUS);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
