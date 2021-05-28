using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Collections;

public class Environment : MonoBehaviour
{
    public CameraManager cameraManager;
    public AgentController agentController;

    private Agent[] agents;
    private int[] randomIndexes;

    private int[] kills;

    public Shield shield;

    private Coroutine[] decisionCoroutines;
    private float decisionTimer = 0;

    private bool hasStarted = false;
    private bool hasFinished = false;

    public GameObject winPanel;
    public TextMeshProUGUI winGameText;

    private void Awake()
    {
        agents = new Agent[Const.NUM_AGENTS];
        decisionCoroutines = new Coroutine[Const.NUM_AGENTS];
        randomIndexes = Utils.ShuffledArray(Const.NUM_AGENTS);
        kills = new int[Const.NUM_AGENTS];
        for (int i = 0; i < Const.NUM_AGENTS; i++)
            kills[i] = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Time.timeScale = 1.1f - Time.timeScale;
        if (hasStarted && !hasFinished)
        {
            decisionTimer += Time.deltaTime;
            if (decisionTimer >= Const.DECISION_TIME)
            {
                FinishDeciding(); // Finish decision epoch
                ExecuteActions();
                CheckAgentsEnergy(); // Check if any agent has died
                CheckAgentsPosition(); 
                StartDeciding(); // Action for next epoch

                randomIndexes = Utils.ShuffledArray(Const.NUM_AGENTS);

                decisionTimer = 0;// -= Const.DECISION_TIME;
            }
        }
        else if (AllAgentsSpawned()) // Finished spawning agents
        {
            StartDeciding(); // Action for first epoch
            hasStarted = true;
        }
    }

    public void AddAgent(Agent agent)
    {
        agents[agent.index - 1] = agent;
    }

    public void RemoveAgent(int index)
    {
        agents[index - 1] = null;
    }

    public Agent GetAgent(int index)
    {
        return agents[index];
    }

    public IEnumerable<Agent> GetAllAgents()
    {
        return agents;
    }

    public int GetNumAliveAgents()
    {
        return agents.Count((agent) => agent != null);
    }

    private bool AllAgentsSpawned()
    {
        foreach (Agent agent in agents)
            if (agent == null)
                return false;
        return true;
    }

    private void StartDeciding()
    {
        foreach (int r in randomIndexes)
            if (agents[r] != null)
            {
                Agent.Perception perception = agents[r].See();
                agents[r].ClearAction();
                decisionCoroutines[r] = StartCoroutine(agents[r].Decide(perception));
            }
    }

    private void FinishDeciding()
    {
        foreach (int r in randomIndexes)
            if (decisionCoroutines[r] != null)
                StopCoroutine(decisionCoroutines[r]);
    }

    private void ExecuteActions()
    {
        foreach (int r in randomIndexes)
            if (agents[r] != null)
            {
                agents[r].BeforeAction();
                agents[r].ExecuteAction();
            }
    }

    private void CheckAgentsEnergy()
    {
        foreach (int r in randomIndexes)
            if (agents[r] != null && agents[r].energy == 0)
            {
                DestroyAgent(r);
                if (GetNumAliveAgents() == 1) // Winner found
                {
                    Debug.Log("Finished");
                    FinishGame();
                    return;
                }
            }
    }
    
    private void CheckAgentsPosition()
    {
        foreach (int r in randomIndexes){
            if (agents[r] != null && !agents[r].inShieldBounds)
                agents[r].shieldTimer ++;
            if (agents[r] != null && agents[r].shieldTimer == agents[r].MAX_SHIELD_TIMER)
            {
                agents[r].shieldTimer = 0;
                agents[r].LoseEnergy(1);
            }
        }
    }

    private void DestroyAgent(int index)
    {
        Agent agent = agents[index];
        agent.SetRanking(GetNumAliveAgents());
        
        agent.DropChest();

        if (agentController.IsActiveAgent(agent))
        {
            cameraManager.EnableCamera();
            agentController.Disable();
        }

        Destroy(agent.gameObject);

        if (agent.lastAttackerIndex != 0)
        {
            Debug.Log("Agent " + (index + 1) + " was killed by agent " + agent.lastAttackerIndex);
            kills[agent.lastAttackerIndex] ++;        
        }
        else
            Debug.Log("Agent " + (index + 1) + " died");

        agents[index] = null;
        shield.UpdateTargetScale(GetNumAliveAgents());
        
    }

    private void FinishGame()
    {
        foreach (Agent agent in agents)
            if (agent != null)
            {
                agent.SetRanking(1);
                winGameText.text = "Agent " + agent.index + " won the Hunger Games!";
                winGameText.color = agent.bodyMaterial.color;
                winGameText.gameObject.SetActive(true);

                winPanel.SetActive(true);

                agentController.SetAgent(agent);

                hasFinished = true;

                Debug.Log("Kills:");
                for (int i = 0; i < Const.NUM_AGENTS; i ++)
                {
                    Debug.Log("Agent " + (i + 1) + ": " + kills[i]);
                }
            }
    }
}
