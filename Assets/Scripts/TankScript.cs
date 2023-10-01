using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    private RectTransform _water;
    [SerializeField] private int _maxCapacity = 8;
    [SerializeField] private int _capacity = 0;
    [SerializeField] private float _waterLevel = 0;
    [SerializeField] private float _animationSpeed = 1f;

    [SerializeField] private float _lineThickness = 0.02f;

    [SerializeField] private List<Valve> _inputValves;
    [SerializeField] private List<Valve> _outputValves;

    // Start is called before the first frame update
    void Start()
    {
        _water = transform.GetChild(0).GetComponent<RectTransform>();
        _waterLevel = _capacity;
        //UpdateWaterDisplay();

        // Create the line markings
        for (int i = 1; i < _maxCapacity; i++)
        {
            GameObject line = new GameObject("Line");
            line.transform.SetParent(transform);
            RectTransform lineTransform = line.AddComponent<RectTransform>();
            lineTransform.anchorMin = new Vector2(0, (float)i / _maxCapacity - _lineThickness/2f);
            lineTransform.anchorMax = new Vector2(0.2f, (float)i / _maxCapacity + _lineThickness/2f);
            lineTransform.offsetMin = new Vector2(0, 0);
            lineTransform.offsetMax = new Vector2(0, 0);
            lineTransform.pivot = new Vector2(0.5f, 0.5f);
            lineTransform.localScale = new Vector3(1, 1, 1);
            lineTransform.anchoredPosition = new Vector2(0, 0);
            line.AddComponent<UnityEngine.UI.Image>().color = Color.black;
        }
    }

    public void Update()
    {
        // Move the water toward the correct value by the namiation speed
        _waterLevel = Mathf.Lerp(_waterLevel, _capacity, _animationSpeed * Time.deltaTime);
        UpdateWaterDisplay();
    }

    public int AddWater(int flowAmount)
    {
        int lastCapacity = _capacity;
        _capacity = Mathf.Clamp(_capacity + flowAmount, 0, _maxCapacity);

        // Check if the tank has overflowed
        if (lastCapacity + flowAmount > _maxCapacity)
        {
            foreach (Valve valve in _inputValves)
            {
                valve.Break();
            }
                
        }

        // Return how much water was added
        return _capacity - lastCapacity;
    }

    private void UpdateWaterDisplay()
    {
        _water.anchorMax = new Vector2(1, _waterLevel / _maxCapacity - 0.001f);
    }

    public int GetCapacity()
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

    public void UpdateState()
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

        if (isFilling && isDraining)
        {
            Debug.LogError("Tank " + this.name + " is both filling and draining");
        }

        // Update the valves
        if (isFilling)
        {
            DisableValves(true);
            EnableValves(true);
        } else if (isDraining)
        {
            DisableValves(false);
            EnableValves(false);
        } else
        {
            EnableValves(true);
            EnableValves(false);
        }   
    }
        
    public void DisableValves(bool isFilling)
    {
        List<Valve> valves = isFilling ? _outputValves : _inputValves;
        Debug.Log("Tank: " + this.name + " Disabling " + valves.Count + " valves");
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
        Debug.Log("Tank: " + this.name + " Enabling " + valvesToEnable.Count + " valves");
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
