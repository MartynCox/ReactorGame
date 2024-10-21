using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TMP_Text _promptText;
    [SerializeField] private TMP_Text _completionText;
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private UnityEngine.UI.Button _nextButton;
    [SerializeField] private UnityEngine.UI.Button _watchVideoButton;
    [SerializeField] private UnityEngine.UI.Button _skipVideoButton;
    [SerializeField] private UnityEngine.UI.Button _submitWorkerIDButton;
    [SerializeField] private Transform _video;
    [SerializeField] private Transform _idArea;
    [SerializeField] private Transform _hidenWhenVideo;

    private void Start()
    {
        if (ScenarioController.Instance.HasSettings())
        {
            SetScenario();
        }

        if (!ScenarioController.Instance.HasSettings()
            || ScenarioController.Instance.GetScenarioIndex() == 0)
        {
            // First get the mturk worker id
            _video.gameObject.SetActive(false);
            _submitWorkerIDButton.interactable = false;

            // Show the video
            StartCoroutine(WaitForVideo());
        } else
        {
            // Hide all dialogue
            _video.gameObject.SetActive(false);
            _idArea.gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitForVideo()
    {
        // Disable skip button if skipping is not allowed
        if (!ScenarioController.Instance.GetAllowVideoSkipping())
        {
            _skipVideoButton.gameObject.SetActive(false);
        }

        // Disable the video button
        _watchVideoButton.interactable = false;
        _watchVideoButton.GetComponentInChildren<TMP_Text>().text = "Loading...";

        yield return new WaitUntil(() => ScenarioController.Instance.HasSettings());

        VideoPlayer videoPlayer = _video.GetComponentInChildren<VideoPlayer>();
        string url = ScenarioController.Instance.GetRandomVideoUrl();

        // Enable the video button
        _watchVideoButton.interactable = true;
        _watchVideoButton.GetComponentInChildren<TMP_Text>().text = "Play Video";

        videoPlayer.url = url;
        videoPlayer.loopPointReached += EndVideoReached;
        _hidenWhenVideo.gameObject.SetActive(false);
    }

    private void EndVideoReached(VideoPlayer vp)
    {
        // Stop the video
        vp.Stop();
        _video.gameObject.SetActive(false);
        _hidenWhenVideo.gameObject.SetActive(true);
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
            _completionText.text = "This Scenario is " + (index + 1) + " of " + total;
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

    public void VerifyWorkerID()
    {
        _submitWorkerIDButton.interactable = (_idInputField.text.Length > 10);
    }

    public void SetUserID()
    {
        if (ScenarioController.Instance.HasSettings())
        {
            ScenarioController.Instance.SetUserId(_idInputField.text.Trim());
        }
        else
        {
            StartCoroutine(AsyncSetUserID());
        }
    }

    public IEnumerator AsyncSetUserID()
    {
        yield return new WaitUntil(() => ScenarioController.Instance.HasSettings());
        ScenarioController.Instance.SetUserId(_idInputField.text.Trim());
    }
}
