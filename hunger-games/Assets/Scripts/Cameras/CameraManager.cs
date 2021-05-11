using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public UnityEngine.Camera floorCamera;
    public UnityEngine.Camera flexibleCamera;
    public UnityEngine.Camera highViewCamera;
    // Start is called before the first frame update

    int cameraIndex = 0;

    private AgentController agentController;

    private void Awake()
    {
        agentController = FindObjectOfType<AgentController>();
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.Tab)) {
            cameraIndex = (cameraIndex + 1) % 3;

            floorCamera.gameObject.SetActive(cameraIndex == 0);
            flexibleCamera.gameObject.SetActive(cameraIndex == 1);
            highViewCamera.gameObject.SetActive(cameraIndex == 2);
            agentController.DisableAllCameras();
        }
    }

    public void DisableAll()
    {
        floorCamera.gameObject.SetActive(false);
        flexibleCamera.gameObject.SetActive(false);
        highViewCamera.gameObject.SetActive(false);
        
    }
}
