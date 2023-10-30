using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts.Settings;
using System;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text _promptText;
    [SerializeField]
    private TMPro.TMP_Text _completionText;
    [SerializeField]
    private UnityEngine.UI.Button _nextButton;

    private void Start()
    {
        //SetScenario();
    }

    public void SetScenario()
    {
        int index = ScenarioController.Instance.GetScenarioIndex();
        int total = ScenarioController.Instance.GetScenarioCount();

        if (index != 0)
        {
            _completionText.text = "Scenario complete!";
            _promptText.text = "Next scenario is " + (index + 1) + " of " + total;
        } else
        {
            _completionText.text = "This Scenario " + (index + 1) + " of " + total;
            _promptText.text = "Press the button to start the scenario";
        }
    }

    public void ReadyScenario(bool isReady)
    {
        _nextButton.interactable = isReady;
        if (isReady)
        {
            SetScenario();
        }
    }

    public void NextScenario()
    {
        // Go to the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
