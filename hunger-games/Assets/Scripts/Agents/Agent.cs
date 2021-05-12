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
    public int MAX_ENERGY;

    [ReadOnly] public int attack;
    [ReadOnly] public int energy;
    private Weapon weapon;

    private bool training;
    private int trainTime;

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
    private AgentController agentController;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        decider = GetComponent<Decider>();
        environment = FindObjectOfType<Environment>();
        agentController = FindObjectOfType<AgentController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        WALK_SPEED = WALK_DISTANCE / environment.DECISION_TIME;
        ROTATE_SPEED = ROTATE_ANGLE / environment.DECISION_TIME;
        attack = BASE_ATTACK;
        energy = MAX_ENERGY;
        weapon = null;

        environment.AddAgent(this);
        agentController.AddAgent(this);
    }

    // TODO: Sensors

    // Actions
    public void BeforeAction()
    {
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
    }

    public void ExecuteAction(Action action)
    {
        switch (action)
        {
            case Action.IDLE: Idle(); break;
            case Action.WALK: Walk(); break;
            case Action.ROTATE_LEFT: RotateLeft(); break;
            case Action.ROTATE_RIGHT: RotateRight(); break;
            case Action.USE_CHEST: UseChest(); break;
            case Action.EAT_BERRIES: EatBerries(); break;
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

    }

    public IEnumerator Decide()
    {
        ChooseAction(decider.Decide());
        yield return null;
    }

    // Method to control agents
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
            weapon.transform.parent = transform;
            ResetWeaponPosition();
        }
    }

    public Weapon UnequipWeapon()
    {
        Weapon oldWeapon = weapon;
        weapon = null;
        return oldWeapon;
    }

    public void UpdateInfo()
    {
        agentController.UpdateInfo();
    }
}
