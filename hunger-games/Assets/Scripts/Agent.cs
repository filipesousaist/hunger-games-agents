using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public UnityEngine.Camera cam;

    public int BASE_ATTACK;
    public int MAX_ENERGY;

    public float WALK_DISTANCE;
    public float DECISION_TIME;

    [ReadOnly] public int attack;
    [ReadOnly] public int energy;

    private Rigidbody myRigidbody;

    private float WALK_SPEED;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        WALK_SPEED = WALK_DISTANCE / DECISION_TIME;
        attack = BASE_ATTACK;
        energy = MAX_ENERGY;
    }

    // TODO: Sensors


    // Actions

    public void Walk()
    {
        myRigidbody.velocity = transform.forward * WALK_SPEED;
    }

    public void RotateLeft()
    {
        myRigidbody.angularVelocity = - transform.up * WALK_SPEED;
    }

    public void RotateRight()
    {
        myRigidbody.angularVelocity = transform.up * WALK_SPEED;
    }
}
