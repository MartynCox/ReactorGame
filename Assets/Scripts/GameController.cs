using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform _advanceBar;
    [SerializeField] private float _stepTime = 15f;
    [SerializeField] private TMP_Text _stepText;
    private float _timeUntilAdvance;

    [SerializeField]
    private List<Valve> _allValves;

    private void Start()
    {
        if (ScenarioController.Instance.Settings != null)
        {
            _stepTime = ScenarioController.Instance.Settings.CycleDuration;
        }

        _timeUntilAdvance = _stepTime;
    }

    private void Update()
    {
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

    public void Advance()
    {
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
    }
}
