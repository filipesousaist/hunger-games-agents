using UnityEngine;
using UnityEngine.UI;

public class AgentController : MonoBehaviour
{
    public CameraManager cameraManager;

    public Text nameText;
    public Text architectureText;

    private int agentIndex = 0;
    private Agent agent = null;
    private Agent[] agents;

    private int NUM_AGENTS;

    // Start is called before the first frame update
    void Awake()
    {
        NUM_AGENTS = FindObjectOfType<AgentSpawner>().AGENT_AMOUNT;
        agents = new Agent[NUM_AGENTS];
    }

    public void AddAgent(Agent agent)
    {
        agents[agent.index - 1] = agent;
    }

    // Update is called once per frame
    void Update()
    {
        bool clicked = true;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            agentIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            agentIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            agentIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            agentIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            agentIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            agentIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            agentIndex = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            agentIndex = 7;
        else
            clicked = false;

        if (clicked)
        {
            cameraManager.DisableAll();
            if (agent != null)
                ToggleAgentControl(false);

            agent = agents[agentIndex];

            ToggleAgentControl(true);
            UpdateAgentInfo();
        }
    }

    public void DisableAllCameras()
    {
        foreach (Agent a in agents)
            if (a != null)
                a.cam.gameObject.SetActive(false);
    }

    public void UpdateAgentInfo()
    {
        nameText.text = agent.name;
        architectureText.text = agent.GetArchitectureName() + " agent";
    }

    public void ToggleAgentControl(bool isControllable)
    {
        agent.cam.gameObject.SetActive(isControllable);
        agent.SetControllable(isControllable);
    }
}
