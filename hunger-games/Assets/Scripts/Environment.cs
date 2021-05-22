using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Environment : MonoBehaviour
{
    public CameraManager cameraManager;
    public AgentController agentController;

    private Agent[] agents;
    private int[] randomIndexes;

    public GameObject Shield;
    public float shieldDecreaseFactor;
    public float shieldDecreaseTimer;

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
                StartCoroutine(Scale());
            }
    }
    
    private void CheckAgentsPosition()
    {
        float coordinate_x;
        float coordinate_z;
        
        foreach (int r in randomIndexes)
        {
            if (agents[r] != null)
            {
                var agentPosition = agents[r].transform.position;
                coordinate_x = agentPosition.x;
                coordinate_z = agentPosition.z;
                
                if (!(coordinate_x<=250*Shield.transform.localScale.x && coordinate_x>=-250*Shield.transform.localScale.x && coordinate_z<=250*Shield.transform.localScale.z && coordinate_z>=-250*Shield.transform.localScale.z))
                {
                    agents[r].LoseEnergy(1);
                }
            }
        }
    }

    private void DestroyAgent(int index)
    {
        Agent agent = agents[index];
        agents[index] = null;

        agent.DropChest();

        if (agentController.IsActiveAgent(agent))
        {
            cameraManager.EnableCamera();
            agentController.Disable();
        }

        Destroy(agent.gameObject);
    }
    
    private IEnumerator Scale()
    {
        float timer = 0;
    
        while (timer < shieldDecreaseTimer) 
        {
            timer += Time.deltaTime;
            Shield.transform.localScale -= new Vector3(1, 1, 1) * (Time.deltaTime * shieldDecreaseFactor);
            yield return null;
        }
    }
}
