using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    enum Hazard
    {
        FIRE, FOG, RAIN, RADIATION
    }
    public Vector3 SWING_START_POSITION;
    public Vector3 SWING_START_ROTATION;
    public Vector3 SWING_END_POSITION;
    public Vector3 SWING_END_ROTATION;

    public float SWING_DURATION; // Epochs

    private Vector3 SWING_POSITION_DIFFERENCE;
    private Vector3 SWING_ROTATION_DIFFERENCE;

    private void Awake()
    {
        SWING_POSITION_DIFFERENCE = SWING_END_POSITION - SWING_START_POSITION;
        SWING_ROTATION_DIFFERENCE = SWING_END_ROTATION - SWING_START_ROTATION;
    }

    public override void Attack(Agent owner)
    {
        StartCoroutine(SwingCo());
        foreach (Agent agent in owner.meleeCollider.GetCollidingAgents())
            agent.LoseEnergy(owner.attack);
    }

    public IEnumerator SwingCo()
    {
        transform.localPosition = SWING_START_POSITION;
        transform.localEulerAngles = SWING_START_ROTATION;

        float t = 0;

        yield return null;

        while (t < 1)
        {
            t = Mathf.Min(t + Time.deltaTime / SWING_DURATION, 1);
            transform.localPosition = SWING_START_POSITION + t * SWING_POSITION_DIFFERENCE;
            transform.localEulerAngles = SWING_START_ROTATION + t * SWING_ROTATION_DIFFERENCE;
            yield return null;
        }

        transform.localPosition = POSITION_IN_AGENT;
        transform.localEulerAngles = ROTATION_IN_AGENT;
    }

    public override Type GetType()
    {
        return Type.SWORD;
    }
}
