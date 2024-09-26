using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EndScreen : MonoBehaviour
{
    void Start()
    {
        // Get completion code
        StartCoroutine(WaitForCode());
    }

    private IEnumerator WaitForCode()
    {
        // Wait for the code to be received
        yield return new WaitUntil(() => ScenarioController.Instance.GetCompletionCode() != null);
        // Get text component
        TMP_Text text = GetComponent<TMP_Text>();
        string code = ScenarioController.Instance.GetCompletionCode();
        text.text = text.text.Replace("{Getting code...}", code);
    }
}