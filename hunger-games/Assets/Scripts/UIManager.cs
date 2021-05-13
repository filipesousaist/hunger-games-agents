using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Text nameText;
    public Text architectureText;
    public Text energyText;
    public Text attackText;
    public Text hideUIText;

    public void UpdateAgentInfo(Agent agent)
    {
        nameText.text = agent.name;
        architectureText.text = agent.GetArchitectureName() + " agent";
        energyText.text = "Energy: " + agent.energy;
        attackText.text = "Attack: " + agent.attack;
    }
    public void RemoveAgentInfo()
    {
        nameText.text = architectureText.text = energyText.text = attackText.text = "";
    }
}
