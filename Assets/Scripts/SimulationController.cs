using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [SerializeField] private Transform _advanceBar;
    private float _stepTime = 8f;
    private float _timeUntilAdvance;

    private void Start()
    {
        _timeUntilAdvance = _stepTime;
    }

    private void Update()
    {
        _timeUntilAdvance -= Time.deltaTime;

        _advanceBar.GetComponent<RectTransform>().anchorMax = 
            new Vector2(1f - _timeUntilAdvance / _stepTime, 1f);

        if (_timeUntilAdvance <= 0f)
        {
            Advance();
        }
    }

    public void Advance()
    {
        _timeUntilAdvance = _stepTime;
        // Find all valves
        ValveScript[] valves = GameObject.FindObjectsOfType<ValveScript>();
        foreach (ValveScript v in valves)
        {
            v.flow();
        }
        

        Debug.Log("advance simulation");
    }
}
