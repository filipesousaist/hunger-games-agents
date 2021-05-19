using UnityEngine;

public class Arrow : Entity
{
    private Agent owner;
    public class ArrowData : Data
    {
        public float rotation;
        public ArrowData()
        {
            type = Type.ROCK;
        }
    }

    public float MOVE_DISTANCE; // Distance to move in one epoch
    private float MOVE_SPEED;

    public float LIFETIME_IN_EPOCHS;
    private float LIFETIME;

    private float timer = 0;

    private int damage;

    private Environment environment;
    private Rigidbody myRigidbody;

    private void Awake()
    {
        environment = FindObjectOfType<Environment>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        MOVE_SPEED = MOVE_DISTANCE / environment.DECISION_TIME;
        LIFETIME = LIFETIME_IN_EPOCHS * environment.DECISION_TIME;
    }

    private void Update()
    {
        myRigidbody.velocity = transform.forward * MOVE_SPEED;

        timer += Time.deltaTime;
        if (timer >= LIFETIME)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Agent agent = collider.GetComponentInParent<Agent>();
        if (agent != null)
        {
            if (agent == owner)
                return;
            agent.LoseEnergy(damage);
        }
        Debug.Log("Arrow has hit " + collider.gameObject.name);
        Destroy(gameObject);
    }

    public void SetOwner(Agent agent)
    {
        owner = agent;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public override Data GetData()
    {
        return new ArrowData()
        {
            position = transform.position,
            rotation = transform.rotation.eulerAngles.y
        };
    }
}
