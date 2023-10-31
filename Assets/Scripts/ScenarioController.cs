using Assets.Scripts.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using Newtonsoft;

[Preserve]
public class ScenarioController : MonoBehaviour
{
    [SerializeField] private string _settingsUri = "https://reactorgame.azurewebsites.net/Settings?handler=json";
    [SerializeField] private float _sceneDelay = 3;

    public static ScenarioController Instance { get; private set; }

    private ScenarioSet _settings;
    private GameResults _results;

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

        // Initialise results
        _results = new GameResults();

        // Load the settings
        StartCoroutine(GetSettings());
        _currentScenarioIndex = 0;
        FindAnyObjectByType<MenuController>().ReadyScenario(false);
    }

    public void EndScenario(string result)
    {
        // Add the result to the list
        _results.ResultList.Add(result);

        // Move to the next scenario
        _currentScenarioIndex++;
        if (_currentScenarioIndex >= _settings.Scenarios.Count)
        {
            // End the game
            StartCoroutine(LoadSceneWithDelay("End", _sceneDelay));
            _results.EndTimestamp = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            // Convert the results to JSON
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(_results);
            Debug.Log(json);
            // Copy to clipboard
            GUIUtility.systemCopyBuffer = json;
            return;
        }

        // Return to the menu
        StartCoroutine(LoadSceneWithDelay("Menu", _sceneDelay));
    }

    private IEnumerator LoadSceneWithDelay(string scene, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
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
                break;
        }

        _settings = newSettings;
        _results.GameName = _settings.Name;
        _results.StartTimestamp = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

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
