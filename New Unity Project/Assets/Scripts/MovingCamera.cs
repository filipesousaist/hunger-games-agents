using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    public float SPEED;

    public float ROTATE_SPEED;
    
    private Camera camera;
    // Start is called before the first frame update
    void Start() {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = gameObject.transform.position.x;
        float z = gameObject.transform.position.z;
        float rotation = transform.rotation.y;
        
        // Movement
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(0, 0, SPEED * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(0, 0, -SPEED * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(-SPEED * Time.deltaTime, 0, 0);
        }
        
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(SPEED * Time.deltaTime, 0, 0);
        }
        
        // Rotation
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.Rotate(Vector3.left, ROTATE_SPEED * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.Rotate(Vector3.right, ROTATE_SPEED * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.down, ROTATE_SPEED * Time.deltaTime, Space.World);
        }
        
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(Vector3.up, ROTATE_SPEED * Time.deltaTime, Space.World);
        }
    }
}
