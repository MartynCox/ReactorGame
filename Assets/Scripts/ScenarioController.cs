using Assets.Scripts.Settings;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

[Preserve]
public class ScenarioController : MonoBehaviour
{
    [SerializeField] private string _settingsUrl = "https://reactorgame.azurewebsites.net/Settings?handler=json";
    [SerializeField] private string _codesUrl = "https://reactorgame.azurewebsites.net/Codes?handler=getrandom";
    [SerializeField] private string _resultsUrl = "https://reactorgame.azurewebsites.net/Results";
    [SerializeField] private float _sceneDelay = 3;
    [SerializeField] private Texture2D _defaultPointer;

    public static ScenarioController Instance { get; private set; }

    private ScenarioSet _settings;
    private GameResult _results;

    private int _currentScenarioIndex = 0;

    public GameScenario Settings
    {
        get
        {
            if (_settings == null || _currentScenarioIndex >= _settings.Scenarios.Count)
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

        // Initialise things
        _results = new GameResult();
        SceneManager.activeSceneChanged += ChangedScene;

        // Load the settings
        StartCoroutine(GetSettings());
        _currentScenarioIndex = 0;
        FindAnyObjectByType<MenuController>().ReadyScenario(false);
    }

    public void EndScenario(ScenarioResult result)
    {
        // Add the result to the list
        _results.ResultList.Add(result);

        // Move to the next scenario
        _currentScenarioIndex++;
        if (_currentScenarioIndex >= _settings.Scenarios.Count)
        {
            StartCoroutine(EndGame());
            return;
        }

        // Return to the menu
        StartCoroutine(LoadSceneWithDelay("Menu", _sceneDelay));
    }

    private IEnumerator EndGame()
    {
        // End the game
        StartCoroutine(LoadSceneWithDelay("End", _sceneDelay));
        _results.EndTimestamp = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        
        // Get the completion code
        yield return StartCoroutine(RequestCompletionCode());
        
        // Convert the results to JSON and send to the server
        string json = JsonConvert.SerializeObject(_results);
        yield return StartCoroutine(SendResult(json));
    }

    private IEnumerator LoadSceneWithDelay(string scene, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        SceneManager.LoadScene(scene);
    }

    public bool HasSettings()
    {
        return Settings != null;
    }

    private IEnumerator GetSettings()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(_settingsUrl);

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
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            FindAnyObjectByType<MenuController>().ReadyScenario(true);
        }

        yield break;
    }

    private class CompletionCode
    {
        public string Code { get; set; }
     }


    private IEnumerator RequestCompletionCode()
    {
        _results.CompletionCode = "";
        using UnityWebRequest webRequest = UnityWebRequest.Get(_codesUrl);

        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                string text = webRequest.downloadHandler.text;
                _results.CompletionCode = JsonConvert.DeserializeObject<CompletionCode>(text).Code;
                break;
        }
    }

    private IEnumerator SendResult(string json)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using UnityWebRequest webRequest = new UnityWebRequest(_resultsUrl, UnityWebRequest.kHttpVerbPOST);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log("Results sent successfully");
                break;
        }
    }

    public void SetUserId(string userId)
    {
        _results.UserId = userId;
    }

    public int GetScenarioIndex()
    {
        return _currentScenarioIndex;
    }

    public int GetScenarioCount()
    {
        return _settings.Scenarios.Count;
    }

    public string GetCompletionCode()
    {
        return _results.CompletionCode;
    }

    public string GetRandomVideoUrl()
    {
        if (_settings == null)
        {
            return null;
        }
        // Get a random video URL
        string url = _settings.VideoUrls[Random.Range(0, _settings.VideoUrls.Count)];
        _results.VideoShown = url.Substring(url.LastIndexOf('/') + 1);
        return url;
    }

    public bool GetAllowVideoSkipping()
    {
        if (_settings == null)
        {
            return false;
        }
        return _settings.AllowVideoSkipping;
    }

    private void ChangedScene(Scene current, Scene next)
    {
        ResetCursor();
    }

    public void ResetCursor()
    {
        // Set cursor to default
        Cursor.SetCursor(_defaultPointer, new Vector2(12f, 0f), CursorMode.Auto);
    }
}
