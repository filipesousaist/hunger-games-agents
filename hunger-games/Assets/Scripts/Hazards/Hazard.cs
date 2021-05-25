using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public abstract class Hazard : MonoBehaviour
{
    public enum Type { FIRE, FOG, RAIN, RADIATION }
    public GameObject prefab;

    public float SPAWN_CHANCE;
    public int NUM_FRAMES_TO_SPAWN;

    public float DURATION_IN_EPOCHS;
    [ReadOnly] public float DURATION;
    public int FREQUENCY; // Max no. of times negative effect occurs during the duration of the hazard
    protected float PERIOD;

    protected bool active = false;
    private float[] agentTimers = new float[Const.NUM_AGENTS];

    protected int index;
    private List<Vector3> region;

    private GameObject[] effects;

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
        effects = new GameObject[region.Count];
    }

    public void Begin()
    {
        for (int i = 0; i < Const.NUM_AGENTS; i ++)
            agentTimers[i] = 0;
        active = true;
        StartCoroutine(SpawnEffectsCo());
    }

    private IEnumerator SpawnEffectsCo()
    {
        int numPositions = region.Count;
        int[] randomIndexes = Utils.ShuffledArray(numPositions);
        int numEffectsPerFrame = effects.Length / NUM_FRAMES_TO_SPAWN;

        int i = 0;
        do
        {
            for (int j = 0; j < numEffectsPerFrame && i < numPositions; j++, i++)
                if (Random.Range(0f, 1f) <= SPAWN_CHANCE)
                    SpawnEffect(i, region[randomIndexes[i]]);
            yield return null;
        }
        while (i < numPositions);
    }

    private void SpawnEffect(int index, Vector3 position)
    {
        GameObject newEffect = Instantiate(prefab);
        newEffect.transform.position = position;
        effects[index] = newEffect;
    }

    protected abstract void Harm(Agent agent);

    public void Stop()
    {
        active = false;
        for (int i = 0; i < effects.Length; i++)
        {
            Destroy(effects[i]);
            effects[i] = null;
        }
    }
}
