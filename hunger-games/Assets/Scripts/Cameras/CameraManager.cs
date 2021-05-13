using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public AgentController agentController;
    public UIManager uIManager;

    public Camera floorCamera;
    public Camera flexibleCamera;
    public Camera highViewCamera;

    public Text nameText;

    private int cameraIndex = -1;
    private Camera[] cameras;

    private void Awake()
    {
        cameras = new Camera[] { floorCamera, flexibleCamera, highViewCamera };
    }

    private void Start()
    {
        SwitchToNextCamera();
        UpdateCameraInfo();
    }

    // Update is called once per frame
    private void Update() {

        if (Input.GetKeyDown(KeyCode.Tab))
            SwitchToNextCamera();
    }

    public void EnableCamera()
    {
        cameras[cameraIndex].gameObject.SetActive(true);
        UpdateCameraInfo();
    }

    private void SwitchToNextCamera()
    {
        cameraIndex = (cameraIndex + 1) % 3;

        for (int i = 0; i < cameras.Length; i++)
            cameras[i].gameObject.SetActive(i == cameraIndex);

        UpdateCameraInfo();

        agentController.DisableAllCameras();
        agentController.ToggleAgentControl(false);
    }

    public void UpdateCameraInfo()
    {
        uIManager.RemoveAgentInfo();
        nameText.text = cameras[cameraIndex].name;
    }

    public void DisableAll()
    {
        floorCamera.gameObject.SetActive(false);
        flexibleCamera.gameObject.SetActive(false);
        highViewCamera.gameObject.SetActive(false);
    }
}
