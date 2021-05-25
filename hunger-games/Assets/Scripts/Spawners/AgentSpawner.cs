using System;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public int SPAWN_RADIUS;

    private int controlableNumber = 2;
    
    // Prefabs

    public GameObject controllableAgent;
    public GameObject baselineAgent;

    public Material[] headMaterials;

    public Material[] bodyMaterials;

    // Start is called before the first frame update
    void Start() {
        System.Random rnd = new System.Random();
        float angleDeg = 360 / Const.NUM_AGENTS;
        double angleRad = Math.PI * 2 / Const.NUM_AGENTS;

        int[] indexes = Utils.ShuffledArray(Const.NUM_AGENTS);

        foreach (int i in indexes) {
            GameObject prefab = (i <= controlableNumber) ? controllableAgent :  baselineAgent;
            Agent newAgent = Instantiate(prefab).GetComponent<Agent>();

            newAgent.index = i + 1;
            newAgent.name = "Agent " + newAgent.index;
            SetAgentLayer(newAgent);
            newAgent.InitInfoPanel();

            newAgent.transform.position = new Vector3(
                (float) Math.Cos(angleRad * i) * SPAWN_RADIUS,
                0,
                (float) Math.Sin(angleRad * i) * SPAWN_RADIUS);
            newAgent.transform.Rotate(0, 270 - angleDeg * i, 0);

            newAgent.head.GetComponent<MeshRenderer>().material = headMaterials[rnd.Next(0, 3)];
            newAgent.torso.GetComponent<MeshRenderer>().material = bodyMaterials[i];
            newAgent.bodyMaterial = bodyMaterials[i];
        }
    }

    private void SetAgentLayer(Agent agent)
    {
        int mask = LayerMask.NameToLayer("Agent " + agent.index);
        agent.gameObject.layer = agent.body.layer = mask;
        for (int i = 0; i < agent.body.transform.childCount; i++)
            agent.body.transform.GetChild(i).gameObject.layer = mask;
    }
}
