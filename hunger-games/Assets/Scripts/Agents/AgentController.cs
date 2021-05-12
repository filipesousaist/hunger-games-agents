using UnityEngine;
using UnityEngine.UI;

public class AgentController : MonoBehaviour
{
    public CameraManager cameraManager;

    public Text nameText;
    public Text architectureText;
    public Text energyText;
    public Text attackText;

    private int agentIndex = 0;
    private Agent agent = null;
    private Agent[] agents;

    private bool firstPerson = true;

    public Vector3 FIRST_PERSON_CAM_POS;
    public Vector3 THIRD_PERSON_CAM_POS;

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
        if (Input.GetKeyDown(KeyCode.C))
        {
            firstPerson = !firstPerson;
            SetCameraView();
        }

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
            SetCameraView();

            ToggleAgentControl(true);
            UpdateInfo();
        }
    }

    public void DisableAllCameras()
    {
        foreach (Agent a in agents)
            if (a != null)
                a.cam.gameObject.SetActive(false);
    }

    public void UpdateInfo()
    {
        nameText.text = agent.name;
        architectureText.text = agent.GetArchitectureName() + " agent";
        energyText.text = "Energy: " + agent.energy;
        attackText.text = "Attack: " + agent.attack;
    }

    public void RemoveInfo()
    {
        nameText.text = architectureText.text = energyText.text = attackText.text = "";
    }

    public void ToggleAgentControl(bool isControllable)
    {
        if (agent != null)
        {
            agent.cam.gameObject.SetActive(isControllable);
            agent.SetControllable(isControllable);
        }
    }

    public void SetCameraView()
    {
        if (agent != null)
        {
            Transform cam = agent.cam.transform;
            if (firstPerson)
                cam.localPosition = FIRST_PERSON_CAM_POS;
            else
                cam.localPosition = THIRD_PERSON_CAM_POS;
            }
    }
}
