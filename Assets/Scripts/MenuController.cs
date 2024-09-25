using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts.Settings;
using System;
using UnityEngine.Video;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TMP_Text _promptText;
    [SerializeField] private TMP_Text _completionText;
    [SerializeField] private UnityEngine.UI.Button _nextButton;
    [SerializeField] private UnityEngine.UI.Button _watchVideoButton;
    [SerializeField] private UnityEngine.UI.Button _skipVideoButton;
    [SerializeField] private Transform _video;
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
            // Show the video
            StartCoroutine(WaitForVideo());
        }
    }

    private IEnumerator WaitForVideo()
    {
        _video.gameObject.SetActive(true);
        VideoPlayer videoPlayer = _video.GetComponentInChildren<VideoPlayer>();
        string url = ScenarioController.Instance.GetRandomVideoUrl();

        // Disable skip button if skipping is not allowed
        if (!ScenarioController.Instance.GetAllowVideoSkipping())
        {
            _skipVideoButton.gameObject.SetActive(false);
        }

        // Disable the video button
        _watchVideoButton.interactable = false;
        _watchVideoButton.GetComponentInChildren<TMP_Text>().text = "Loading...";

        while(url == null)
        {
            yield return new WaitForSeconds(0.2f);
            url = ScenarioController.Instance.GetRandomVideoUrl();
        }

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
}
