using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimulationController : MonoBehaviour
{
    [SerializeField] private Transform _advanceBar;
    [SerializeField] private float _stepTime = 15f;
    [SerializeField] private TMP_Text _stepText;
    private float _timeUntilAdvance;

    [SerializeField]
    private List<Valve> _allValves;

    private void Start()
    {
        _timeUntilAdvance = _stepTime;
    }

    private void Update()
    {
        _timeUntilAdvance -= Time.deltaTime;

        _advanceBar.GetComponent<RectTransform>().anchorMax = 
            new Vector2(1f - _timeUntilAdvance / _stepTime, 1f);

        _stepText.text = "Next cycle in "
            + Mathf.CeilToInt(_timeUntilAdvance).ToString()
            + " s";

        if (_timeUntilAdvance <= 0f)
        {
            Advance();
        }
    }

    public void Advance()
    {
        _timeUntilAdvance = _stepTime;
        // Find all valves
        // ValveScript[] valves = GameObject.FindObjectsOfType<ValveScript>();
        foreach (Valve v in _allValves)
        {
            v.Flow();
        }

        // After all valves have been updated, update the pipes
        foreach (Valve v in _allValves)
        {
            v.UpdateAppearance();
        }


        Debug.Log("advance simulation");
    }
}
