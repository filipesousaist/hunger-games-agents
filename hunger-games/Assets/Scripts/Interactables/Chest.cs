using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Entity, IInteractable
{
    public enum State
    {
        OPEN, CLOSED, OPENING, CLOSING
    }
    public class ChestData : Data
    {
        public State state;
        public Weapon.Type weaponType;
        public int weaponAttack;

        public ChestData()
        {
            type = Type.CHEST;
        }
    }

    public float WEAPON_MOVE_SPEED;
    public float SHOW_WEAPON_HEIGHT;
    public float HIDE_WEAPON_HEIGHT;

    public float ANGULAR_VELOCITY;

    public bool SPAWN_WEAPON;

    public Transform lid;

    public MeshRenderer[] renderers;

    public GameObject sword;
    public GameObject bow;

    private Weapon weapon;

    private Coroutine openCo;
    private Coroutine closeCo;
    private Coroutine displayWeaponCo;
    private Coroutine hideWeaponCo;

    [ReadOnly] public State state; 

    private List<InteractionCollider> interactionColliders;

    private void Awake()
    {
        interactionColliders = new List<InteractionCollider>();
        if (SPAWN_WEAPON)
        {
            GameObject prefab = Random.Range(0, 2) == 0 ? sword : bow;
            GameObject newWeapon = Instantiate(prefab);
            SetWeapon(newWeapon.GetComponent<Weapon>(), HIDE_WEAPON_HEIGHT);
        }
    }

    private void Start()
    {
        state = State.CLOSED;
    }

    public void SetWeapon(Weapon newWeapon, float height)
    {
        weapon = newWeapon;
        if (weapon != null)
        {
            weapon.transform.parent = transform;
            SetWeaponPosition(height);
        }
    }

    private void SetWeaponPosition(float height)
    {
        weapon.transform.localPosition = Vector3.up * (height - transform.position.y);
        weapon.transform.localRotation = Quaternion.Euler(new Vector3(0, -45, 135));
    }

    public void Open()
    {
        openCo = StartCoroutine(OpenCo());
        if (weapon != null)
            displayWeaponCo = StartCoroutine(DisplayWeaponCo());
    }

    public void Close()
    {
        closeCo = StartCoroutine(CloseCo());
        if (weapon != null)
            hideWeaponCo = StartCoroutine(HideWeaponCo());
    }

    private IEnumerator OpenCo()
    {
        state = State.OPENING;
        if (closeCo != null)
            StopCoroutine(closeCo);

        while (Utils.ClampMod(lid.rotation.eulerAngles.z, -180, 180) < 90)
        {
            lid.Rotate(0, 0, ANGULAR_VELOCITY * Time.deltaTime);
            yield return null;
        }
        lid.Rotate(0, 0, 90 - Utils.ClampMod(lid.rotation.eulerAngles.z, -180, 180));

        openCo = null;
        state = State.OPEN;
    }

    private IEnumerator CloseCo()
    {
        state = State.CLOSING;
        if (openCo != null)
            StopCoroutine(openCo);

        while (Utils.ClampMod(lid.rotation.eulerAngles.z, -180, 180) > 0)
        {
            lid.Rotate(0, 0, -ANGULAR_VELOCITY * Time.deltaTime);
            yield return null;
        }
        lid.Rotate(0, 0, -Utils.ClampMod(lid.rotation.eulerAngles.z, -180, 180));

        closeCo = null;
        state = State.CLOSED;
    }

    private IEnumerator DisplayWeaponCo()
    {
        if (hideWeaponCo != null)
            StopCoroutine(hideWeaponCo);

        Debug.Log(weapon);

        Transform weaponTransform = weapon.transform;
        while (weaponTransform.position.y < SHOW_WEAPON_HEIGHT)
        {
            float distance = Mathf.Min(WEAPON_MOVE_SPEED * Time.deltaTime, SHOW_WEAPON_HEIGHT - weaponTransform.position.y);
            weaponTransform.position += Vector3.up * distance;
            yield return null;
        }
    }

    public IEnumerator HideWeaponCo()
    {
        if (displayWeaponCo != null)
            StopCoroutine(displayWeaponCo);

        Transform weaponTransform = weapon.transform;
        while (weaponTransform.position.y > HIDE_WEAPON_HEIGHT)
        {
            float distance = Mathf.Min(WEAPON_MOVE_SPEED * Time.deltaTime, weaponTransform.position.y - HIDE_WEAPON_HEIGHT);
            weaponTransform.position += Vector3.down * distance;
            yield return null;
        }
    }

    public void Interact(Agent agent)
    {
        switch (state)
        {
            case State.CLOSING:
            case State.CLOSED:
                Open(); break;
            case State.OPEN:
                ChangeWeapon(agent, false); break;
        }
    }

    public void AddInteractionCollider(InteractionCollider interactionCollider)
    {
        if (!interactionColliders.Contains(interactionCollider))
            interactionColliders.Add(interactionCollider);
    }

    public void RemoveInteractionCollider(InteractionCollider interactionCollider)
    {
        interactionColliders.Remove(interactionCollider);
        if (interactionColliders.Count == 0 && (state == State.OPEN || state == State.OPENING))
            Close();
    }

    public void ChangeWeapon(Agent agent, bool hide)
    {
        Weapon agentWeapon = agent.UnequipWeapon();
        agent.EquipWeapon(weapon);
        SetWeapon(agentWeapon, hide ? HIDE_WEAPON_HEIGHT : SHOW_WEAPON_HEIGHT);

        Debug.Log(weapon);
    }

    public void RemoveWeapon()
    {
        Destroy(weapon);
        weapon = null;
    }

    public void SetMaterial(Material material)
    {
        foreach (MeshRenderer renderer in renderers)
            renderer.material = material;
    }

    public override Data GetData()
    {
        return new ChestData()
        {
            position = transform.position,
            state = state,
            weaponType = weapon != null ? weapon.GetType() : Weapon.Type.NONE,
            weaponAttack = weapon != null ? weapon.attack : 0
        };
    }
}
