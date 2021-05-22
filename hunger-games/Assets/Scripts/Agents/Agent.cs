using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Agent : Entity
{
    public enum Action
    {
        IDLE, WALK, ROTATE_LEFT, ROTATE_RIGHT, USE_CHEST, EAT_BERRIES, ATTACK, TRAIN
    }

    public struct Perception
    {
        // Agent's own data
        public AgentData myData;
        public IEnumerable<EntityData> visionData;
        public ChestData nearestChestData;
        public BushData nearestBushData;
    }

    public Camera cam;
    public Camera faceCam;
    public RenderTexture faceCamTexturePrefab;

    private TextMeshProUGUI energyText;
    private TextMeshProUGUI attackText;

    public VisionCollider visionCollider;
    public InteractionCollider interactionCollider;
    public MeleeCollider meleeCollider;

    public GameObject body;
    public GameObject head;
    public GameObject torso;

    [ReadOnly] public int index;

    public int BASE_ATTACK;
    public int MAX_ATTACK;
    public int MIN_ATTACK;
    public int MAX_ENERGY;

    [ReadOnly] public int attack;
    [ReadOnly] public int energy;
    private Weapon weapon;

    public float ATTACK_WAIT_TIME;
    private float attackWaitTimer = 0;

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

    public float WALK_DISTANCE; // Distance to walk in one epoch
    public float ROTATE_ANGLE; // Angle to rotate in one epoch

    private float WALK_SPEED;
    private float ROTATE_SPEED;

    private Rigidbody myRigidbody;
    private Decider decider;

    private Environment environment;
    private AgentController agentController;
    private UIManager uIManager;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        decider = GetComponent<Decider>();
        environment = FindObjectOfType<Environment>();
        agentController = FindObjectOfType<AgentController>();
        uIManager = FindObjectOfType<UIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        WALK_SPEED = WALK_DISTANCE / Const.DECISION_TIME;
        ROTATE_SPEED = ROTATE_ANGLE / Const.DECISION_TIME;

        TRAIN_JUMP_DURATION = TRAIN_DURATION * Const.DECISION_TIME / TRAIN_NUM_JUMPS;
        TRAIN_JUMP_INITIAL_SPEED = 4 * TRAIN_JUMP_HEIGHT / TRAIN_JUMP_DURATION;
        TRAIN_GRAVITY_ACC = TRAIN_JUMP_INITIAL_SPEED / TRAIN_JUMP_DURATION;

        ROPE_ANGULAR_FREQ = 360 / TRAIN_JUMP_DURATION;

        attack = BASE_ATTACK;
        energy = MAX_ENERGY;
        weapon = null;

        environment.AddAgent(this);

        UpdateInfo();
    }

    public void InitInfoPanel()
    {
        faceCam.cullingMask = 1 << LayerMask.NameToLayer("Agent " + index);

        Transform panelContainerTransform = FindObjectOfType<Canvas>().transform.Find("_AgentPanelContainer");
        Transform newPanelTransform = Instantiate(panelContainerTransform.Find("_AgentPanel"));
        newPanelTransform.gameObject.SetActive(true);
        
        newPanelTransform.SetParent(panelContainerTransform);
        newPanelTransform.GetComponent<RectTransform>().anchorMin = new Vector2(0.25f, 1 - index / 8f);
        newPanelTransform.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1 - (index - 1) / 8f);
        newPanelTransform.GetComponent<RectTransform>().offsetMin =
        newPanelTransform.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        newPanelTransform.localScale = Vector3.one;

        newPanelTransform.Find("_NamePanel").Find("_NameText").GetComponent<TextMeshProUGUI>().text = name;
        energyText = newPanelTransform.Find("_EnergyPanel").Find("_EnergyText").GetComponent<TextMeshProUGUI>();
        attackText = newPanelTransform.Find("_AttackPanel").Find("_AttackText").GetComponent<TextMeshProUGUI>();

        RenderTexture faceCamTexture = Instantiate(faceCamTexturePrefab);
        faceCam.targetTexture = faceCamTexture;
        newPanelTransform.Find("_PortraitImage").GetComponent<RawImage>().texture = faceCamTexture;
    }

    void Update()
    {
        if (training)
            TrainUpdate();
    }

    private void TrainUpdate()
    {
        trainAnimationTimer += Time.deltaTime;

        float t = trainAnimationTimer % TRAIN_JUMP_DURATION;
        float y = (TRAIN_JUMP_INITIAL_SPEED - TRAIN_GRAVITY_ACC * t) * t;

        transform.position += transform.up * (y - transform.position.y);

        rope.transform.Rotate(new Vector3(0, 0, ROPE_ANGULAR_FREQ * Time.deltaTime), Space.Self);
    }

    public Perception See()
    {
        Chest nearestChest = interactionCollider.GetNearestChest(transform.position);
        Bush nearestBush = interactionCollider.GetNearestBush(transform.position);

        return new Perception()
        {
            myData = (AgentData) GetData(),
            visionData = visionCollider.GetCollidingEntitiesData(),
            nearestChestData = nearestChest != null ? (ChestData) nearestChest.GetData() : null,
            nearestBushData = nearestBush != null ? (BushData) nearestBush.GetData() : null
        };
    }

    // Actions
    public void BeforeAction()
    {
        attackWaitTimer = Mathf.Max(attackWaitTimer - 1, 0);
        if (weapon != null && weapon.GetType() == Weapon.Type.BOW && attackWaitTimer == 0)
            ((Bow) weapon).ShowFixedArrow();

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

    public void ExecuteAction()
    {
        if (!training)
            switch (decider.nextAction)
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

    public void ClearAction()
    {
        decider.nextAction = Action.IDLE;
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
        if (attackWaitTimer == 0)
        {
            if (weapon != null)
                weapon.Attack(this);
            else
                Punch();
            attackWaitTimer = ATTACK_WAIT_TIME;
        }
    }

    private void Train()
    {
        training = true;

        CreateRope();
    }

    public IEnumerator Decide(Perception perception)
    {
        decider.Decide(perception);
        yield return null;
    }

    public void SetControllable(bool controllable)
    { 
        decider.SetControllable(controllable);
    }

    public void UpdateInfo()
    {
        energyText.text = energy.ToString();
        attackText.text = (attack + GetWeaponAttack()).ToString();

        if (agentController.IsActiveAgent(this))
            uIManager.UpdateAgentInfo(this);
    }

    public string GetArchitectureName()
    {
        return decider.GetArchitectureName();
    }

    private void ResetWeaponPosition()
    {
        weapon.transform.localPosition = weapon.POSITION_IN_AGENT;
        weapon.transform.localRotation = Quaternion.Euler(weapon.ROTATION_IN_AGENT);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
        if (weapon != null)
        {
            weapon.transform.parent = transform;
            ResetWeaponPosition();
        }
        UpdateInfo();
    }

    public Weapon UnequipWeapon()
    {
        Weapon oldWeapon = weapon;
        weapon = null;

        UpdateInfo();

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

        transform.Translate(-transform.up * transform.position.y);
    }

    private void Punch()
    {
        foreach (Agent agent in meleeCollider.GetCollidingAgents())
            agent.LoseEnergy(attack);
    }

    public void GainEnergy(int amount)
    {
        energy = Mathf.Min(energy + amount, MAX_ENERGY);
        UpdateInfo();
    }

    public void LoseEnergy(int amount)
    {
        energy = Mathf.Max(energy - amount, 0);
        UpdateInfo();
    }

    public void DropChest()
    {
        Chest newChest = Instantiate(chestPrefab).GetComponent<Chest>();
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

    public int GetWeaponAttack()
    {
        return weapon != null ? weapon.attack : 0;
    }

    public void CorrodeWeapon(int attackLoss)
    {
        if (weapon != null)
        {
            weapon.Corrode(attackLoss);
            UpdateInfo();
        }
    }

    public override EntityData GetData()
    {
        return new AgentData()
        {
            position = transform.position,
            index = index,
            rotation = transform.rotation.eulerAngles.y,
            energy = energy,
            attack = attack,
            weaponType = weapon != null ? weapon.GetType() : Weapon.Type.NONE,
            weaponAttack = GetWeaponAttack()
        };
    }
}

public class AgentData : EntityData
{
    public int index;
    public float rotation; // Degrees (rotation.y)
    public int energy;
    public int attack;
    public Weapon.Type weaponType;
    public int weaponAttack;

    public AgentData()
    {
        type = Entity.Type.AGENT;
    }
}
