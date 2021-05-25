using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public int SPAWN_RADIUS;

    public int NUM_CONTROLLABLE;

    // Prefabs

    public GameObject controllableAgent;
    public GameObject baselineAgent;

    public Material[] headMaterials;

    public Material[] bodyMaterials;

    // Start is called before the first frame update
    void Start() {
        float angleDeg = 360 / Const.NUM_AGENTS;
        float angleRad = Mathf.PI * 2 / Const.NUM_AGENTS;

        int[] randomIndexes = Utils.ShuffledArray(Const.NUM_AGENTS);


        for (int i = 0; i < Const.NUM_AGENTS; i++)
        {
            int r = randomIndexes[i];
            GameObject prefab = Global.architectures[r] != null ?
                Global.architectures[r].gameObject : SelectPrefab(r);

            Agent newAgent = Instantiate(prefab).GetComponent<Agent>();

            newAgent.index = r + 1;
            newAgent.name = Utils.GenerateAgentName(newAgent.GetComponent<Decider>(), newAgent.index);
            SetAgentLayer(newAgent);
            newAgent.InitInfoPanel();

            newAgent.transform.position = new Vector3(
                -Mathf.Sin(angleRad * (i + 0.5f)) * Const.SPAWN_RADIUS,
                0,
                Mathf.Cos(angleRad * (i + 0.5f)) * Const.SPAWN_RADIUS);
            newAgent.transform.Rotate(0, 180 - angleDeg * (i + 0.5f), 0);

            newAgent.head.GetComponent<MeshRenderer>().material = headMaterials[Random.Range(0, 3)];
            newAgent.torso.GetComponent<MeshRenderer>().material = bodyMaterials[r];
            newAgent.bodyMaterial = bodyMaterials[r];
        }
    }

    private GameObject SelectPrefab(int index)
    {
        return (index < NUM_CONTROLLABLE) ? controllableAgent : baselineAgent;
    }

    private void SetAgentLayer(Agent agent)
    {
        int mask = LayerMask.NameToLayer("Agent " + agent.index);
        agent.gameObject.layer = agent.body.layer = mask;
        for (int i = 0; i < agent.body.transform.childCount; i++)
            agent.body.transform.GetChild(i).gameObject.layer = mask;
    }
}
