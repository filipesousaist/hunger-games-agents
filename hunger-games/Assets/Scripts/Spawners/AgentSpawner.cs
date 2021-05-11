using System;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public int AGENT_AMOUNT;

    public int SPAWN_RADIUS;
    
    // Prefabs

    public GameObject controllableAgent;
    public GameObject simpleAgent;

    public Material[] headMaterials;

    public Material[] bodyMaterials;

    // Start is called before the first frame update
    void Start() {
        System.Random rnd = new System.Random();
        float angleDeg = 360 / AGENT_AMOUNT;
        double angleRad = Math.PI * 2 / AGENT_AMOUNT;

        int[] indexes = Utils.ShuffledArray(AGENT_AMOUNT);

        foreach (int i in indexes) {
            GameObject prefab = (i == 0) ? controllableAgent : simpleAgent;
            Agent newAgent = Instantiate(prefab).GetComponent<Agent>();

            newAgent.index = i + 1;
            newAgent.name = "Agent " + newAgent.index;

            newAgent.transform.position = new Vector3(
                (float) Math.Cos(angleRad * i) * SPAWN_RADIUS,
                0,
                (float) Math.Sin(angleRad * i) * SPAWN_RADIUS);
            newAgent.transform.Rotate(0, 270 - angleDeg * i, 0);

            newAgent.transform.Find("Head").GetComponent<MeshRenderer>().material = headMaterials[rnd.Next(0, 3)];
            newAgent.transform.Find("Body").GetComponent<MeshRenderer>().material = bodyMaterials[i];
        }
    }
}
