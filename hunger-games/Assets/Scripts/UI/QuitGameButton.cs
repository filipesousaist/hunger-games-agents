using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGameButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
