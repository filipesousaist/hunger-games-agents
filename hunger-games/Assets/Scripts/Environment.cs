using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public CameraManager cameraManager;
    public AgentController agentController;

    [ReadOnly] public Agent.Action[] actions;

    private Agent[] agents;

    private Coroutine[] decisionCoroutines;

    public float DECISION_TIME;

    private float decisionTimer = 0;

    private bool hasStarted = false;

    private int NUM_AGENTS;


    private void Awake()
    {
        NUM_AGENTS = FindObjectOfType<AgentSpawner>().AGENT_AMOUNT;
        actions = new Agent.Action[NUM_AGENTS];
        agents = new Agent[NUM_AGENTS];
        decisionCoroutines = new Coroutine[NUM_AGENTS];
    }

    // Update is called once per frame
    private void Update()
    {
        if (hasStarted)
        {
            decisionTimer += Time.deltaTime;
            if (decisionTimer >= DECISION_TIME)
            {
                FinishDeciding(); // Finish decision epoch
                ExecuteActions();
                CheckAgentsEnergy(); // Check if any agent has died
                StartDeciding(); // Action for next epoch

                decisionTimer -= DECISION_TIME;
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

    private bool AllAgentsSpawned()
    {
        foreach (Agent agent in agents)
            if (agent == null)
                return false;
        return true;
    }

    private void StartDeciding()
    {
        // Reset actions vector
        for (int i = 0; i < NUM_AGENTS; i ++)
            actions[i] = Agent.Action.IDLE;

        // Start deciding
        for (int i = 0; i < NUM_AGENTS; i ++)
            if (agents[i] != null)
                decisionCoroutines[i] = StartCoroutine(agents[i].Decide());
    }

    private void FinishDeciding()
    {
        for (int i = 0; i < NUM_AGENTS; i ++)
            if (decisionCoroutines[i] != null)
                StopCoroutine(decisionCoroutines[i]);
    }

    private void ExecuteActions()
    {
        for (int i = 0; i < NUM_AGENTS; i ++)
            if (agents[i] != null)
            {
                agents[i].BeforeAction();
                agents[i].ExecuteAction(actions[i]);
            }
    }

    private void CheckAgentsEnergy()
    {
        for (int i = 0; i < NUM_AGENTS; i++)
            if (agents[i] != null && agents[i].energy == 0)
                DestroyAgent(i);
    }

    private void DestroyAgent(int index)
    {
        Agent agent = agents[index];
        agents[index] = null;

        agent.DropChest();
        Destroy(agent.gameObject);

        cameraManager.EnableCamera();
        agentController.Disable();
    }
}
