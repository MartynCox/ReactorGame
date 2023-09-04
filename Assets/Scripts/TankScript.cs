using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    private RectTransform _water;
    [SerializeField] private int _maxCapacity = 8;
    [SerializeField] private int _capacity = 0;

    [SerializeField] private float _lineThickness = 0.02f;

    [SerializeField] private List<Valve> _inputValves;
    [SerializeField] private List<Valve> _outputValves;

    // Start is called before the first frame update
    void Start()
    {
        _water = transform.GetChild(0).GetComponent<RectTransform>();
        UpdateWaterDisplay();

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

    public int AddWater(int flowAmount)
    {
        int lastCapacity = _capacity;
        _capacity = Mathf.Clamp(_capacity + flowAmount, 0, _maxCapacity);
        UpdateWaterDisplay();
        return _capacity;
    }

    private void UpdateWaterDisplay()
    {
        _water.anchorMax = new Vector2(1, (float)_capacity / _maxCapacity - 0.001f);
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
            if (valve.IsOpenForward)
            {
                isFilling = true;
                break;
            } else if (valve.IsOpenBackward)
            {
                isDraining = true;
                break;
            }
        }
        foreach (var valve in _outputValves)
        {
            if (valve.IsOpenForward)
            {
                isDraining = true;
                break;
            } else if (valve.IsOpenBackward)
            {
                isFilling = true;
                break;
            }
        }
        Debug.Log("Tank: " + this.name + "is filling: " + isFilling + ", draining: " + isDraining);

        // Update the valves
        if (isFilling)
        {
            DisableDraining();
            EnableFilling();
        } else if (isDraining)
        {
            DisableFilling();
            EnableDraining();
        } else
        {
            EnableFilling();
            EnableDraining();
        }   
    }

    public void DisableFilling()
    {
        Debug.Log("Disabling filling");
        // Disable all input valves
        foreach (Valve valve in _inputValves)
        {
            valve.SetEnabled(false);
        }
    }

    public void DisableDraining()
    {
        Debug.Log("Disabling draining for " + _outputValves.Count + "valves");
        // Disable all output valves
        foreach (Valve valve in _outputValves)
        {
            valve.SetEnabled(false);
        }
    }

    public void EnableFilling()
    {
        Debug.Log("Enabling filling");
        // Ensure all output valves are closed
        foreach (Valve valve in _outputValves)
        {
            if (valve.IsOpen) { return; }
        }

        // Enable all input valves
        foreach (Valve valve in _inputValves)
        {
            valve.SetEnabled(true);
        }
    }

    public void EnableDraining()
    {
        Debug.Log("Enabling draining");
        // Ensure all input valves are closed
        foreach (Valve valve in _inputValves)
        {
            if (valve.IsOpen) { return; }
        }

        // Enable all output valves
        foreach (Valve valve in _outputValves)
        {
            valve.SetEnabled(true);
        }
    }
}
