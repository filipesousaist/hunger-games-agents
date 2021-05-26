using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Environment : MonoBehaviour
{
    public CameraManager cameraManager;
    public AgentController agentController;

    private Agent[] agents;
    private int[] randomIndexes;

    public Shield shield;

    private Coroutine[] decisionCoroutines;
    private float decisionTimer = 0;

    private bool hasStarted = false;

    private void Awake()
    {
        agents = new Agent[Const.NUM_AGENTS];
        decisionCoroutines = new Coroutine[Const.NUM_AGENTS];
        randomIndexes = Utils.ShuffledArray(Const.NUM_AGENTS);
    }

    // Update is called once per frame
    private void Update()
    {
        if (hasStarted)
        {
            decisionTimer += Time.deltaTime;
            if (decisionTimer >= Const.DECISION_TIME)
            {
                FinishDeciding(); // Finish decision epoch
                ExecuteActions();
                CheckAgentsEnergy(); // Check if any agent has died
                CheckAgentsPosition(); 
                StartDeciding(); // Action for next epoch

                if (GetNumAliveAgents() == 0)
                    FinishGame();

                randomIndexes = Utils.ShuffledArray(Const.NUM_AGENTS);

                decisionTimer -= Const.DECISION_TIME;
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

    private int GetNumAliveAgents()
    {
        return agents.Count((agent) => agent != null && agent.energy > 0);
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
                DestroyAgent(r);
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
        agents[index] = null;

        agent.SetRanking(GetNumAliveAgents() - 1);
        agent.DropChest();

        if (agentController.IsActiveAgent(agent))
        {
            cameraManager.EnableCamera();
            agentController.Disable();
        }

        Destroy(agent.gameObject);

        shield.UpdateTargetScale(GetNumAliveAgents());
    }

    private void FinishGame() { }
}
