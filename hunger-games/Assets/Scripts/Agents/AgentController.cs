using UnityEngine;
using UnityEngine.UI;

public class AgentController : MonoBehaviour
{
    public UIManager uIManager;
    public CameraManager cameraManager;
    public Environment environment;

    public Text nameText;
    public Text architectureText;
    public Text energyText;
    public Text attackText;

    private int agentIndex = 0;
    private Agent agent = null;

    private bool firstPerson = false;

    public Vector3 FIRST_PERSON_CAM_POS;
    public Vector3 THIRD_PERSON_CAM_POS;

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
            SetAgentIndex(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetAgentIndex(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetAgentIndex(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SetAgentIndex(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            SetAgentIndex(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            SetAgentIndex(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            SetAgentIndex(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            SetAgentIndex(7);
        else
            clicked = false;

        if (clicked)
        {
            if (agent != null)
                ToggleAgentControl(false);

            agent = environment.GetAgent(agentIndex);
            if (agent != null)
            {
                cameraManager.DisableAll();
                SetCameraView();

                ToggleAgentControl(true);
                uIManager.UpdateAgentInfo(agent);
            }
        }
    }

    private void SetAgentIndex(int index)
    {
        if (environment.GetAgent(index) != null)
            agentIndex = index;
    }

    public void Disable()
    {
        agent = null;
    }

    public void DisableAllCameras()
    {
        foreach (Agent a in environment.GetAllAgents())
            if (a != null)
                a.cam.gameObject.SetActive(false);
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
