using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreen : MonoBehaviour
{
    void Start()
    {
        // Get text component
        TMP_Text text = GetComponent<TMP_Text>();
        string code = ScenarioController.Instance.GetCompletionCode();
        text.text = text.text.Replace("{Code}", code);
    }
}
