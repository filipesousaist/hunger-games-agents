using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    private int agentIndex = 0;
    private Agent agent = null;
    private Agent[] agents;

    private CameraManager cameraManager;

    // Start is called before the first frame update
    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        StartCoroutine(GetAgentsWithDelay());
    }

    private IEnumerator GetAgentsWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        agents = FindObjectsOfType<Agent>();
    }

    // Update is called once per frame
    void Update()
    {
        bool clicked = true;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            agentIndex = 1;   
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            agentIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            agentIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            agentIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            agentIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            agentIndex = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            agentIndex = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            agentIndex = 8;
        else
            clicked = false;

        if (clicked)
        {
            agent = agents[agentIndex - 1];
            cameraManager.DisableAll();
            DisableAllCameras();
            agent.cam.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            agent.Walk();
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            agent.RotateLeft();
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            agent.RotateRight();
    }

    public void DisableAllCameras()
    {
        foreach (Agent a in agents)
            a.cam.gameObject.SetActive(false);
    }
}
