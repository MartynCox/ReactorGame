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
            _video.gameObject.SetActive(true);
            VideoPlayer videoPlayer = _video.GetComponentInChildren<VideoPlayer>();
            videoPlayer.loopPointReached += EndVideoReached;
            _hidenWhenVideo.gameObject.SetActive(false);
        }
    }

    private void EndVideoReached(VideoPlayer vp)
    {
        Debug.Log("stop video");
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
