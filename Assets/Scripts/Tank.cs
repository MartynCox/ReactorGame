using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tank : MonoBehaviour
{
    private RectTransform _water;
    [SerializeField] private GameObject _warningSign;
    private bool _isOverflowed = false;
    [SerializeField] private string _name;
    [SerializeField] private float _maxCapacity = 8;
    [SerializeField] private float _capacity = 0;
    private float _mockCapacity = 0;
    private float _waterLevel = 0;
    [SerializeField] private float _animationSpeed = 2.5f;

    [SerializeField] private float _lineThickness = 0.02f;
    [SerializeField] private bool _hasLineMarkings = true;

    private List<Valve> _inputValves = new List<Valve>();
    private List<Valve> _outputValves = new List<Valve>();

    void Start()
    {
        // Get capacity from settings if it exists
        if (ScenarioController.Instance.HasSettings()
            && ScenarioController.Instance.Settings.Tanks.ContainsKey(_name))
        {
            _maxCapacity = ScenarioController.Instance.Settings.Tanks[_name].Capacity;
            _capacity = ScenarioController.Instance.Settings.Tanks[_name].StartLevel;
        }

        // Get the water display
        _water = transform.GetChild(0).GetComponent<RectTransform>();
        _waterLevel = _capacity;

        if (_hasLineMarkings)
        {
            CreateLineMarkings();
        }

        _warningSign.SetActive(false);
    }

    private void CreateLineMarkings()
    {
        // Create the line markings
        for (int i = 1; i < _maxCapacity; i++)
        {
            GameObject line = new GameObject("Line");
            line.transform.SetParent(transform);
            RectTransform lineTransform = line.AddComponent<RectTransform>();
            lineTransform.anchorMin = new Vector2(0, (float)i / _maxCapacity - _lineThickness / 2f);
            lineTransform.anchorMax = new Vector2(0.2f, (float)i / _maxCapacity + _lineThickness / 2f);
            lineTransform.offsetMin = new Vector2(0, 0);
            lineTransform.offsetMax = new Vector2(0, 0);
            lineTransform.pivot = new Vector2(0.5f, 0.5f);
            lineTransform.localScale = new Vector3(1, 1, 1);
            lineTransform.anchoredPosition = new Vector2(0, 0);
            line.AddComponent<UnityEngine.UI.Image>().color = Color.black;
        }
    }

    public virtual void Update()
    {
        // Move the water toward the correct value by the namiation speed
        _waterLevel = Mathf.Lerp(_waterLevel, _capacity, _animationSpeed * Time.deltaTime);
        UpdateWaterDisplay();
    }

    public virtual float AddWater(float flowAmount)
    {
        float lastCapacity = _capacity;
        _capacity = Mathf.Clamp(_capacity + flowAmount, 0, _maxCapacity);

        // Check if the tank has overflowed
        if (ScenarioController.Instance.Settings.BreakTankOnOverflow && lastCapacity + flowAmount > _maxCapacity)
        {
            _waterLevel = _capacity;
            foreach (Valve valve in _inputValves)
            {
                valve.Break();
                _isOverflowed = true;
            }
                
        }

        UpdateWarningSign();

        // Return how much water was added
        return _capacity - lastCapacity;
    }

    private void UpdateWaterDisplay()
    {
        _water.anchorMax = new Vector2(1, _waterLevel / _maxCapacity - 0.001f);
    }

    public float GetCapacity()
    {
        return _capacity;
    }
    
    public void AddInputValve(Valve valve)
    {
        _inputValves.Add(valve);
    }

    public void AddOutputValve(Valve valve)
    {
        _outputValves.Add(valve);
    }

    protected List<Valve> GetInputValves()
    {
        return _inputValves;
    }

    public void ResetMockCapacity()
    {
        _mockCapacity = _capacity;
    }

    public void MockDrain(float amount)
    {
        _mockCapacity -= amount;
        _mockCapacity = Mathf.Clamp(_mockCapacity, 0, _maxCapacity);
    }

    public float GetMockCapacity()
    {
        return _mockCapacity;
    }

    public virtual void UpdateState()
    {
        // Check if the tank is being filled or drained
        bool isFilling = false;
        bool isDraining = false;
        foreach (var valve in _inputValves)
        {
            if (valve.IsOpen)
            {
                isFilling = true;
                break;
            }
        }
        foreach (var valve in _outputValves)
        {
            if (valve.IsOpen)
            {
                isDraining = true;
                break;
            }
        }

        // Update the valves
        if (isFilling)
        {
            DisableValves(true);
            EnableValves(true);
        }
        else if (isDraining)
        {
            DisableValves(false);
            EnableValves(false);
        }
        else
        {
            EnableValves(true);
            EnableValves(false);
        }

        UpdateWarningSign();
    }

    private void UpdateWarningSign()
    {
        // Check whether the tank is about to overflow
        _warningSign.SetActive(false);
        if (_isOverflowed) { return; }
        
        float flowAmount = 0;
        foreach (Valve valve in _inputValves)
        {
            if (valve.IsOpen && valve.IsWaterAvailable())
            {
                flowAmount += valve.GetFlowRate();
            }
        }

        if (_capacity + flowAmount > _maxCapacity)
        {
            _warningSign.SetActive(true);
        }
    }
        
    public void DisableValves(bool isFilling)
    {
        List<Valve> valves = isFilling ? _outputValves : _inputValves;
        foreach (Valve valve in valves)
        {
            valve.SetEnabled(false, this.name);
        }
    }

    public void EnableValves(bool isFilling)
    {
        List<Valve> valvesToEnable = isFilling ? _inputValves : _outputValves;
        List<Valve> valvesToCheck = isFilling ? _outputValves : _inputValves;

        // Make sure all valves that are not in the opposite direction are closed
        foreach(Valve valve in valvesToCheck)
        {
            if (valve.IsOpen) { return; }
        }

        // Enable all valves in the correct direction
        foreach (Valve valve in valvesToEnable)
        {
            valve.SetEnabled(true, this.name);
        }
    }

    public void ReverseDirection(Valve valve, bool isNowInput)
    {
        // Remove the valve from the old list
        List<Valve> oldList = isNowInput ? _inputValves : _outputValves;
        if (oldList.Contains(valve))
        {
            oldList.Remove(valve);
        } else
        {
            Debug.LogError("Valve " + valve.name + " not found in " + this.name + " " + (isNowInput ? "output" : "input") + " valves");
        }

        // Add the valve to the new list
        List<Valve> newList = isNowInput ? _outputValves : _inputValves;
        newList.Add(valve);
    }
}
