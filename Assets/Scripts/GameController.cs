using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel.Design;
using Assets.Scripts;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform _advanceBar;
    [SerializeField] private float _stepTime = 15f;
    [SerializeField] private TMP_Text _stepText;
    [SerializeField] private Image _overlay;
    [SerializeField] private float _fadeOutTime = 3f;
    private float _timeUntilAdvance;
    private int _currentStep = 0;
    private int _totalSteps = 0;

    [SerializeField]
    private List<Valve> _allValves;

    private ResultOutput _resultOutput;

    private bool _isPaused = false;
    private bool _isFinished = false;

    private void Start()
    {
        if (ScenarioController.Instance.Settings != null)
        {
            _stepTime = ScenarioController.Instance.Settings.CycleDuration;
            _totalSteps = ScenarioController.Instance.Settings.TotalCycles;
        }

        _timeUntilAdvance = _stepTime;
        _currentStep = 0;
        _resultOutput = new ResultOutput();
        _isPaused = false;
        _isFinished = false;
    }

    private void Update()
    {
        CheckShouldFadeOut();

        if (_isPaused) { return; }

        _timeUntilAdvance -= Time.deltaTime;

        _advanceBar.GetComponent<RectTransform>().anchorMax = 
            new Vector2(1f - _timeUntilAdvance / _stepTime, 1f);

        _stepText.text = "Next cycle in "
            + Mathf.CeilToInt(_timeUntilAdvance).ToString() + " s";

        if (_timeUntilAdvance <= 0f)
        {
            Advance();
        }
    }

    private void CheckShouldFadeOut()
    {
        if (!_isFinished) { return; }

        Color c = _overlay.color;
        c.a += Time.deltaTime / _fadeOutTime;
        _overlay.color = c;
    }

    public void Advance()
    {
        if (_isPaused) { return; }

        _currentStep++;
        _timeUntilAdvance = _stepTime;

        // Find and record water flow to reactor
        Reactor reactor = FindObjectOfType<Reactor>();
        if (reactor != null)
        {
            reactor.RecordValue();
        }
        
        // Update all valves
        foreach (Valve v in _allValves)
        {
            v.Flow();
        }

        // After all valves have been updated, update the pipes
        foreach (Valve v in _allValves)
        {
            v.UpdateAppearance();
        }

        // Finally update the reactor
        if (reactor != null)
        {
            reactor.UpdateState();
        }

        // After all components have been updated, record the results
        RecordResults(reactor);

        // Check if we're done
        if (_currentStep >= _totalSteps)
        {
            _isPaused = true;
            _isFinished = true;
            ScenarioController.Instance.EndScenario(_resultOutput.ToCSV());
        }
    }

    private void RecordResults(Reactor reactor)
    {
        ResultRecord record = new ResultRecord();

        record.SetTemperature(reactor.GetTemperature());
        record.SetTime(_currentStep);

        List<int> flows = new List<int>();
        foreach (Valve v in _allValves)
        {
            flows.Add(v.GetFlowRate());
        }

        record.SetFlows(flows);
        _resultOutput.AddRecord(record);
    }
}
