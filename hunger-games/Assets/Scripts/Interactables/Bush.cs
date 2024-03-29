using UnityEngine;

public abstract class Bush : Entity, IInteractable
{
    public GameObject otherPrefab;
    public bool hasBerries;

    public float GROW_TIME;
    [ReadOnly] public float timer;

    private Environment environment;

    private void Awake()
    {
        environment = FindObjectOfType<Environment>();
    }

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        if (!hasBerries)
        {
            timer += Time.deltaTime;
            if (timer >= GROW_TIME)
                ChangeBushType();
        }
    }

    public void Interact(Agent agent)
    {
        if (hasBerries)
        {
            EatBerries(agent);
            ChangeBushType();
        }
    }

    public void ChangeBushType()
    {
        GameObject newBush = Instantiate(otherPrefab);
        newBush.transform.position = transform.position;
        foreach (Agent agent in environment.GetAllAgents())
            if (agent != null)
                agent.interactionCollider.RemoveBush(this);
        Destroy(gameObject);
    }

    protected abstract void EatBerries(Agent agent);

    protected abstract bool IsPoisonous();

    public override EntityData GetData()
    {
        return new BushData()
        {
            position = transform.position,
            hasBerries = hasBerries,
            poisonous = IsPoisonous()
        };
    }
}

public class BushData : EntityData
{
    public bool hasBerries;
    public bool poisonous;

    public BushData()
    {
        type = Entity.Type.BUSH;
    }
}
