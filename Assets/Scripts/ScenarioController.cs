using Assets.Scripts.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

[Preserve]
public class ScenarioController : MonoBehaviour
{
    [SerializeField] private string _settingsUri = "https://reactorgame.azurewebsites.net/Settings?handler=json";

    public static ScenarioController Instance { get; private set; }

    private ScenarioSet _settings;

    private int _currentScenarioIndex = 0;
    public GameScenario Settings
    {
        get
        {
            if (_settings == null)
            {
                return null;
            }
            return _settings.Scenarios[_currentScenarioIndex];
        }
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        StartCoroutine(GetSettings());
        _currentScenarioIndex = 0;
        FindAnyObjectByType<MenuController>().ReadyScenario(false);
    }

    public void EndScenario()
    {
        _currentScenarioIndex++;
        if (_currentScenarioIndex >= _settings.Scenarios.Count)
        {
            // End the game
            UnityEngine.SceneManagement.SceneManager.LoadScene("End");
            return;
        }

        // Return to the menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public bool HasSettings()
    {
        return Settings != null;
    }

    private IEnumerator GetSettings()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(_settingsUri);

        yield return webRequest.SendWebRequest();

        ScenarioSet newSettings = null;
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                newSettings = ScenarioSet.LoadSettings(webRequest.downloadHandler.text);
                Debug.Log("Scenario: " + newSettings.Scenarios[0].ScenarioName);
                break;
        }

        _settings = newSettings;

        // Ready the button if we're in the menu
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Menu")
        {
            FindAnyObjectByType<MenuController>().ReadyScenario(true);
        }
        
        yield break;
    }

    public int GetScenarioIndex()
    {
        return _currentScenarioIndex;
    }

    public int GetScenarioCount()
    {
        return _settings.Scenarios.Count;
    }

}
