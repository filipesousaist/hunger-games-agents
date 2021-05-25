using UnityEngine;
using TMPro;
using System.Linq;
public class SelectArchitectureDropdown : MonoBehaviour
{
    public int index;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI architectureText;

    private SelectArchitecturesManager selectArchitecturesManager;
    private void Awake()
    {
        selectArchitecturesManager = FindObjectOfType<SelectArchitecturesManager>();
    }

    private void Start()
    {
        GetComponent<TMP_Dropdown>().AddOptions(selectArchitecturesManager.namesToArchitectures.Keys.ToList());
        OnValueChange();
    }

    public void OnValueChange()
    {
        Decider decider = selectArchitecturesManager.namesToArchitectures[architectureText.text];
        nameText.text = Utils.GenerateAgentName(decider, index);
        Global.architectures[index - 1] = decider; 
    }
}
