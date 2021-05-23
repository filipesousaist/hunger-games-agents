using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    public enum Type { FIRE, FOG, RAIN, RADIATION }
    public GameObject prefab;

    public float SPAWN_CHANCE;

    public float DURATION_IN_EPOCHS;
    [ReadOnly] public float DURATION;
    public int FREQUENCY; // Max no. of times negative effect occurs during the duration of the hazard
    protected float PERIOD;

    protected bool active = false;
    private float[] agentTimers = new float[Const.NUM_AGENTS];

    protected int index;
    private List<Vector3> region;

    private readonly List<GameObject> effects = new List<GameObject>();

    private Environment environment;
    protected HazardsManager hazardManager;

    private void Awake()
    {
        environment = FindObjectOfType<Environment>();
        hazardManager = FindObjectOfType<HazardsManager>();
        DURATION = DURATION_IN_EPOCHS * Const.DECISION_TIME;
        PERIOD = FREQUENCY > 0 ? DURATION / FREQUENCY : float.MaxValue;
    }

    private void Update()
    {
        if (active)
        {
            IEnumerable<Agent> agents = environment.GetAllAgents();
            for (int i = 0; i < Const.NUM_AGENTS; i ++)
            {
                Agent agent = agents.ElementAt(i);
                if (agent != null && hazardManager.GetRegion(agent.transform.position) == index)
                {
                    agentTimers[i] += Time.deltaTime;
                    if (agentTimers[i] >= PERIOD)
                    {
                        agentTimers[i] -= PERIOD;
                        Harm(agent);
                    }
                }
            }    
        }
        OnUpdate();
    }

    protected virtual void OnUpdate() { }

    public void SetRegion(int index, List<Vector3> region)
    {
        this.index = index;
        this.region = region;
    }

    public void Begin()
    {
        for (int i = 0; i < Const.NUM_AGENTS; i ++)
            agentTimers[i] = 0;
        active = true;
        foreach (Vector3 position in region)
            if (Random.Range(0f, 1f) <= SPAWN_CHANCE)
            {
                GameObject effect = Instantiate(prefab);
                effect.transform.position = position;
                effects.Add(effect);
            }
    }

    protected abstract void Harm(Agent agent); 

    public void Stop()
    {
        active = false;
        foreach (GameObject effect in effects)
            Destroy(effect);
        effects.Clear();
        
    }
}
