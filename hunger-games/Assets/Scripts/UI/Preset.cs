using UnityEngine;
using TMPro;

public class Preset : MonoBehaviour
{
    private TMP_Dropdown[] dropdowns;
    private SelectArchitecturesManager selectArchitecturesManager;

    public Decider[] deciders;

    private void Awake()
    {
        selectArchitecturesManager = FindObjectOfType<SelectArchitecturesManager>();
        dropdowns = FindObjectsOfType<TMP_Dropdown>();
    }

    public void Apply()
    {
        for (int i = 0; i < deciders.Length; i ++)
        {
            dropdowns[i].value = selectArchitecturesManager.namesToIndices[deciders[i].GetArchitectureName()];
        }
    }
}