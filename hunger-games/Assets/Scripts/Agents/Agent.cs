using System.Collections;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public enum Action
    {
        IDLE, WALK, ROTATE_LEFT, ROTATE_RIGHT, USE_CHEST, EAT_BERRIES, ATTACK, TRAIN
    }

    public Camera cam;

    public AgentInteractionCollider interactionCollider;

    [ReadOnly] public int index;

    public int BASE_ATTACK;
    public int MAX_ATTACK;
    public int MAX_ENERGY;

    [ReadOnly] public int attack;
    [ReadOnly] public int energy;
    private Weapon weapon;

    private bool training = false;
    private int trainTimer; // Epochs
    private float trainAnimationTimer;

    public int TRAIN_DURATION; // Epochs
    public int TRAIN_ATTACK_GAIN;
    public int TRAIN_ENERGY_LOSS;

    public float TRAIN_JUMP_HEIGHT;
    public float TRAIN_NUM_JUMPS; // Number of jumps during train

    private float ROPE_ANGULAR_FREQ;
    public float ROPE_OFFSET_Y;

    public GameObject ropePrefab;
    private GameObject rope;

    private float TRAIN_JUMP_DURATION; // Seconds
    private float TRAIN_JUMP_INITIAL_SPEED;
    private float TRAIN_GRAVITY_ACC;

    public GameObject chestPrefab;

    [ReadOnly] public Material bodyMaterial; 

    public Vector3 SWORD_POSITION;
    public Vector3 SWORD_ROTATION;
    public Vector3 BOW_POSITION;
    public Vector3 BOW_ROTATION;

    public float WALK_DISTANCE;
    public float ROTATE_ANGLE;

    private float WALK_SPEED;
    private float ROTATE_SPEED;

    private Rigidbody myRigidbody;
    private Decider decider;

    private Environment environment;
    private UIManager uIManager;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        decider = GetComponent<Decider>();
        environment = FindObjectOfType<Environment>();
        uIManager = FindObjectOfType<UIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        WALK_SPEED = WALK_DISTANCE / environment.DECISION_TIME;
        ROTATE_SPEED = ROTATE_ANGLE / environment.DECISION_TIME;

        TRAIN_JUMP_DURATION = TRAIN_DURATION * environment.DECISION_TIME / TRAIN_NUM_JUMPS;
        TRAIN_JUMP_INITIAL_SPEED = 4 * TRAIN_JUMP_HEIGHT / TRAIN_JUMP_DURATION;
        TRAIN_GRAVITY_ACC = TRAIN_JUMP_INITIAL_SPEED / TRAIN_JUMP_DURATION;

        ROPE_ANGULAR_FREQ = 360 / TRAIN_JUMP_DURATION;

        attack = BASE_ATTACK;
        energy = MAX_ENERGY;
        weapon = null;

        environment.AddAgent(this);
    }

    void Update()
    {
        if (training)
        {
            trainAnimationTimer += Time.deltaTime;

            float t = trainAnimationTimer % TRAIN_JUMP_DURATION;
            float y = TRAIN_JUMP_INITIAL_SPEED * t - TRAIN_GRAVITY_ACC * t * t;

            transform.position += transform.up * (y - transform.position.y);

            rope.transform.Rotate(new Vector3(0, 0, ROPE_ANGULAR_FREQ * Time.deltaTime), Space.Self);
        }
    }

    // TODO: Sensors

    // Actions
    public void BeforeAction()
    {

        if (training)
        {
            trainTimer ++;
            if (trainTimer == TRAIN_DURATION)
                FinishTraining();
        }
        else
        {
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void ExecuteAction(Action action)
    {
        if (!training)
            switch (action)
            {
                case Action.IDLE:           Idle();         break;
                case Action.WALK:           Walk();         break;
                case Action.ROTATE_LEFT:    RotateLeft();   break;
                case Action.ROTATE_RIGHT:   RotateRight();  break;
                case Action.USE_CHEST:      UseChest();     break;
                case Action.EAT_BERRIES:    EatBerries();   break;
                case Action.ATTACK:         Attack();       break;
                case Action.TRAIN:          Train();        break;
            }
    }

    protected void ChooseAction(Action action)
    {
        environment.actions[index - 1] = action;
    }

    private void Idle() { }

    private void Walk()
    {
        myRigidbody.velocity = transform.forward * WALK_SPEED;
    }

    private void RotateLeft()
    {
        myRigidbody.angularVelocity = - transform.up * ROTATE_SPEED;
    }

    private void RotateRight()
    {
        myRigidbody.angularVelocity = transform.up * ROTATE_SPEED;
    }

    private void UseChest()
    {
        Chest chest = interactionCollider.GetNearestChest(transform.position);
        if (chest != null)
            chest.Interact(this);
    }

    private void EatBerries()
    {
        Bush bush = interactionCollider.GetNearestBush(transform.position);
        if (bush != null && bush.hasBerries)
            bush.Interact(this);
    }

    private void Attack()
    {

    }

    private void Train()
    {
        training = true;

        CreateRope();
    }

    public IEnumerator Decide()
    {
        ChooseAction(decider.Decide());
        yield return null;
    }

    public void SetControllable(bool controllable)
    { 
        decider.SetControllable(controllable);
    }

    public string GetArchitectureName()
    {
        return decider.GetArchitectureName();
    }

    private void ResetWeaponPosition()
    {
        weapon.transform.localPosition = weapon.type == Weapon.Type.SWORD ? SWORD_POSITION : BOW_POSITION;
        weapon.transform.localRotation = Quaternion.Euler(
            weapon.type == Weapon.Type.SWORD ? SWORD_ROTATION : BOW_ROTATION);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
        if (weapon != null)
        {
            attack += weapon.attack;
            uIManager.UpdateAgentInfo(this);

            weapon.transform.parent = transform;
            ResetWeaponPosition();
        }
    }

    public Weapon UnequipWeapon()
    {
        if (weapon != null)
        {
            attack -= weapon.attack;
            uIManager.UpdateAgentInfo(this);
        }

        Weapon oldWeapon = weapon;
        weapon = null;
        return oldWeapon;
    }

    private void CreateRope()
    {
        rope = Instantiate(ropePrefab);
        rope.transform.parent = transform;
        rope.transform.localPosition = transform.up * ROPE_OFFSET_Y;
        rope.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
    }

    private void FinishTraining()
    {
        attack = Mathf.Min(attack + TRAIN_ATTACK_GAIN, MAX_ATTACK);
        LoseEnergy(TRAIN_ENERGY_LOSS);

        training = false;
        trainTimer = 0;
        trainAnimationTimer = 0;

        Destroy(rope.gameObject);
    }

    public void GainEnergy(int amount)
    {
        energy = Mathf.Min(energy + amount, MAX_ENERGY);
        uIManager.UpdateAgentInfo(this);
    }

    public void LoseEnergy(int amount)
    {
        energy = Mathf.Max(energy - amount, 0);

        uIManager.UpdateAgentInfo(this);
    }

    public void DropChest()
    {
        Chest newChest = Instantiate(chestPrefab).GetComponent<Chest>();
        newChest.RemoveWeapon();
        newChest.ChangeWeapon(this, true);

        newChest.SetMaterial(bodyMaterial);

        newChest.transform.position = new Vector3(
            Mathf.RoundToInt(transform.position.x),
            0,
            Mathf.RoundToInt(transform.position.z)
        );
        newChest.transform.rotation = Quaternion.Euler(
            0,
            Mathf.RoundToInt(transform.rotation.eulerAngles.y / 90) * 90,
            0
        );
    }
}
