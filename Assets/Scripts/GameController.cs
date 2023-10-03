using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform _advanceBar;
    [SerializeField] private float _stepTime = 15f;
    [SerializeField] private TMP_Text _stepText;
    private float _timeUntilAdvance;

    [SerializeField]
    private List<Valve> _allValves;

    public static GameController Instance { get; private set; }
    public GameSettings Settings { get; set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Read the settings from a file
        Settings = GameSettings.LoadSettings("Settings/settings.json");
        _stepTime = Settings.CycleDuration;
    }

    void Start()
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
    }
}
