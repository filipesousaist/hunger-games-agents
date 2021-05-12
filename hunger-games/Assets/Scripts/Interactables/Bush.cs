using UnityEngine;

public abstract class Bush : Interactable
{
    public GameObject otherPrefab;
    public bool hasBerries;

    public float GROW_TIME;
    private float timer;

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

    public override void Interact(Agent agent)
    {
        if (hasBerries)
        {
            EatBerries(agent);
            ChangeBushType();
        }
    }

    private void ChangeBushType()
    {
        GameObject newBush = Instantiate(otherPrefab);
        newBush.transform.position = transform.position;
        foreach (Agent agent in environment.GetAllAgents())
            if (agent != null)
                agent.interactionCollider.RemoveBush(this);
        Destroy(gameObject);
    }

    protected abstract void EatBerries(Agent agent);
}
