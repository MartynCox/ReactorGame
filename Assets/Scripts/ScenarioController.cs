using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScenarioController : MonoBehaviour
{
    [SerializeField] private const string _settingsUri = "https://reactorgame.azurewebsites.net/settings?handler=json";

    public static ScenarioController Instance { get; private set; }
    public GameSettings Settings { get; private set; }
    
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
    }

    public bool HasSettings()
    {
        return Settings != null;
    }

    private IEnumerator GetSettings()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(_settingsUri);

        yield return webRequest.SendWebRequest();

        GameSettings newSettings = null;
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(webRequest.downloadHandler.text);
                newSettings = JsonUtility.FromJson<GameSettings>(webRequest.downloadHandler.text);
                break;
        }

        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        // Set the game controller's settings
        Settings = newSettings;
        yield break;
    }

}
