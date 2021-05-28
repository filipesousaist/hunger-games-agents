using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    /// <summary>
    /// When an agent dies, the shield's scale will decrease by SHIELD_DECREASE_FACTOR times its original value. 
    /// </summary>
    public float SHIELD_DECREASE_FACTOR;

    /// <summary>
    /// Number of epochs spent to decrase shield by "SHIELD_DECREASE_FACTOR".
    /// </summary>
    public float DECREASE_SHIELD_DURATION_EPOCHS;
    private float DECREASE_SHIELD_SPEED; // fraction of shield per seconds

    /// <summary>
    /// After this number of epochs, shield starts decreasing automatically regardless of agents dying or not.
    /// </summary>
    public float EPOCHS_TO_START_AUTO_DECREASE_SHIELD;
    private float TIME_TO_START_AUTO_DECREASE_SHIELD; // seconds
    
    private float MIN_SHIELD_SCALE;

    private float targetScale;

    private float timer;

    // Start is called before the first frame update
    void Awake()
    {
        targetScale = transform.localScale.y;
        DECREASE_SHIELD_SPEED = SHIELD_DECREASE_FACTOR / (DECREASE_SHIELD_DURATION_EPOCHS * Const.DECISION_TIME);
        TIME_TO_START_AUTO_DECREASE_SHIELD = EPOCHS_TO_START_AUTO_DECREASE_SHIELD * Const.DECISION_TIME;
        MIN_SHIELD_SCALE = transform.localScale.y * (1 - SHIELD_DECREASE_FACTOR * Const.NUM_AGENTS);
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > TIME_TO_START_AUTO_DECREASE_SHIELD)
            targetScale = MIN_SHIELD_SCALE;

        float scale = transform.localScale.y;
        if (scale > targetScale)
        {
            float decreaseAmount = Mathf.Min(Time.deltaTime * DECREASE_SHIELD_SPEED, scale - targetScale);
            transform.localScale -= Vector3.one * decreaseAmount;
        }
    }

    public void UpdateTargetScale(int numAliveAgents)
    {
        targetScale = MIN_SHIELD_SCALE + SHIELD_DECREASE_FACTOR * numAliveAgents;
    }

    public float GetRadius()
    {
        return Const.WORLD_SIZE * transform.localScale.y;
    }

    public bool IsPositionOutside(Vector3 position)
    {
        float x = position.x;
        float z = position.z;

        return x >=  250 * transform.localScale.x
            || x <= -250 * transform.localScale.x
            || z >=  250 * transform.localScale.z
            || z <= -250 * transform.localScale.z;
    }

    private void OnTriggerExit(Collider other)
    {
        Agent agent = other.GetComponentInParent<Agent>();
        if (agent != null)
            agent.inShieldBounds = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Agent agent = other.GetComponentInParent<Agent>();
        if (agent != null)
            agent.inShieldBounds = true;
    }
}
