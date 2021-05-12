using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public enum State
    {
        OPEN, CLOSED, OPENING, CLOSING
    }

    public float DISPLAY_WEAPON_SPEED;
    public float DISPLAY_WEAPON_HEIGHT;

    public float ANGULAR_VELOCITY;

    public Transform lid;

    public GameObject sword;
    public GameObject bow;

    private Weapon currentWeapon;
    private float weaponOriginalHeight;

    private Coroutine openCo;
    private Coroutine closeCo;
    private Coroutine displayWeaponCo;
    private Coroutine hideWeaponCo;

    [ReadOnly] public State state; 

    private List<AgentInteractionCollider> interactionColliders;

    private readonly System.Random random = new System.Random();

    private void Awake()
    {
        interactionColliders = new List<AgentInteractionCollider>();
    }

    private void Start()
    {
        state = State.CLOSED;
        GameObject prefab = random.Next(10) < 5 ? sword : bow;
        GameObject newWeapon = Instantiate(prefab, transform.position + Vector3.up * 0.3f, Quaternion.Euler(new Vector3(0, -45, 90)));
        newWeapon.transform.Rotate(new Vector3(0, 0, 45), Space.Self);

        SetWeapon(newWeapon.GetComponent<Weapon>());
    }


    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        weaponOriginalHeight = weapon.transform.position.y;
    }

    public void Open()
    {
        openCo = StartCoroutine(OpenCo());
        if (currentWeapon != null)
            displayWeaponCo = StartCoroutine(DisplayWeaponCo());
    }

    public void Close()
    {
        closeCo = StartCoroutine(CloseCo());
        if (currentWeapon != null)
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

        Transform weaponTransform = currentWeapon.transform;
        while (weaponTransform.position.y < DISPLAY_WEAPON_HEIGHT)
        {
            float distance = Mathf.Min(DISPLAY_WEAPON_SPEED * Time.deltaTime, DISPLAY_WEAPON_HEIGHT - weaponTransform.position.y);
            weaponTransform.position += Vector3.up * distance;
            yield return null;
        }
    }

    public IEnumerator HideWeaponCo()
    {
        if (displayWeaponCo != null)
            StopCoroutine(displayWeaponCo);

        Transform weaponTransform = currentWeapon.transform;
        while (weaponTransform.position.y > weaponOriginalHeight)
        {
            float distance = Mathf.Min(DISPLAY_WEAPON_SPEED * Time.deltaTime, weaponTransform.position.y - weaponOriginalHeight);
            weaponTransform.position += Vector3.down * distance;
            yield return null;
        }
    }

    public override void Interact(Agent agent)
    {
        switch (state)
        {
            case State.CLOSING:
            case State.CLOSED:
                Open(); break;
            case State.OPEN:
                ChangeWeapon(agent); break;
        }
    }

    public void AddInteractionCollider(AgentInteractionCollider interactionCollider)
    {
        if (!interactionColliders.Contains(interactionCollider))
            interactionColliders.Add(interactionCollider);
    }

    public void RemoveInteractionCollider(AgentInteractionCollider interactionCollider)
    {
        interactionColliders.Remove(interactionCollider);
        if (interactionColliders.Count == 0 && (state == State.OPEN || state == State.OPENING))
            Close();
    }

    public void ChangeWeapon(Agent agent)
    {
        Weapon agentWeapon = agent.UnequipWeapon();
        agent.EquipWeapon(currentWeapon);
        currentWeapon = agentWeapon;
    }
}
