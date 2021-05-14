using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Text nameText;
    public Text architectureText;
    public Text energyText;
    public Text attackText;
    public Text toggleUIText;

    public AgentController agentController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            ToggleUI();
    }

    public void UpdateAgentInfo(Agent agent)
    {
        if (agent != null)
        {
            nameText.text = agent.name;
            architectureText.text = agent.GetArchitectureName() + " agent";
            energyText.text = "Energy: " + agent.energy;
            attackText.text = "Attack: " + agent.attack;
        }
    }
    public void RemoveAgentInfo()
    {
        nameText.text = architectureText.text = energyText.text = attackText.text = "";
    }

    public void ToggleUI()
    {
        bool newValue = !nameText.gameObject.activeSelf;
        nameText.gameObject.SetActive(newValue);
        architectureText.gameObject.SetActive(newValue);
        energyText.gameObject.SetActive(newValue);
        attackText.gameObject.SetActive(newValue);
        toggleUIText.gameObject.SetActive(newValue);
    }
}
