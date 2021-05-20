using UnityEngine;

public class Arrow : Entity
{
    private Agent owner;

    public float MOVE_DISTANCE; // Distance to move in one epoch
    private float MOVE_SPEED;

    public float RANGE;
    private float LIFETIME;

    private float timer = 0;

    private int damage;

    private Rigidbody myRigidbody;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        MOVE_SPEED = MOVE_DISTANCE / Const.DECISION_TIME;
        LIFETIME = RANGE / MOVE_SPEED;
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
        if (collider.transform.CompareTag("Collider"))
            return;
        Agent agent = collider.GetComponentInParent<Agent>();
        if (agent != null)
        {
            if (agent == owner)
                return;
            agent.LoseEnergy(damage);
        }

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

    public override EntityData GetData()
    {
        return new ArrowData()
        {
            position = transform.position,
            rotation = transform.rotation.eulerAngles.y
        };
    }
}

public class ArrowData : EntityData
{
    public float rotation;
    public ArrowData()
    {
        type = Entity.Type.ARROW;
    }
}
