using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Reactor : Tank
{
    [SerializeField] private List<Pipe> _outflowPipes;
    [SerializeField] private Slider _temperatureSlider;
    [SerializeField] private float _tempAnimSpeed = 2.5f;
    [SerializeField] private GameObject _targetMarker;
    [SerializeField] private float _currentTemperature;
    [SerializeField] private Color[] _colours;
    [SerializeField] private ReactorGraph _graph;
    private float _targetTemperature = 120f;

    void Start()
    {
        // Set up the slider
        float minTemp = 0;
        float maxTemp = 300;
        int totalCycles = 10;
        if (ScenarioController.Instance.HasSettings())
        {
            var sortedFlows = ScenarioController.Instance.Settings.FlowTemperatures.Keys.OrderBy(k => k).ToList();
            int minFlow = sortedFlows[0];
            maxTemp = ScenarioController.Instance.Settings.FlowTemperatures[minFlow];
            _targetTemperature = ScenarioController.Instance.Settings.TargetTemperature;
            totalCycles = ScenarioController.Instance.Settings.TotalCycles;
        }
        
        _temperatureSlider.minValue = minTemp;
        _temperatureSlider.maxValue = maxTemp;
        if (minTemp == maxTemp){ return; }

        // Set the target marker
        _currentTemperature = _targetTemperature;
        float sliderHeight = _temperatureSlider.GetComponent<RectTransform>().rect.height;
        RectTransform markerTransform = _targetMarker.GetComponent<RectTransform>();
        markerTransform.localPosition = new Vector3(
            markerTransform.localPosition.x,
            sliderHeight * (_targetTemperature - minTemp) / (maxTemp - minTemp),
            markerTransform.localPosition.z
        );

        // Set up the graph
        _graph.SetTemps(_targetTemperature, minTemp, maxTemp, totalCycles);

        // Update the state
        UpdateState();
    }

    public override void Update()
    {
        // Interpolate towards the current temperature
        float targetValue = _currentTemperature;
        float newValue = Mathf.Lerp(_temperatureSlider.value, targetValue, Time.deltaTime * _tempAnimSpeed);
        _temperatureSlider.value = newValue;

        // Set the colour of the fill area
        if (targetValue <= _targetTemperature)
        {
            _temperatureSlider.fillRect.GetComponent<Image>().color = _colours[0];
        }
        else
        {
            _temperatureSlider.fillRect.GetComponent<Image>().color = _colours[1];
        }
    }

    public override float AddWater(float flowAmount)
    {
        // Reactor can't receive water
        return flowAmount;
    }

    public override void UpdateState()
    {
        float flow = CalculateFlow();
        _currentTemperature = FindTemperatureByFlow(flow);

        // Add degree symbol to temperature and update display
        _temperatureSlider.GetComponentInChildren<TMP_Text>().text = _currentTemperature.ToString() + "°";
    }

    private float CalculateFlow()
    {
        // Count the total flow rate to the reactor
        float totalFlowRate = 0;
        foreach (Valve valve in GetInputValves())
        {
            valve.ResetMockCapacity();
        }
        foreach (Valve valve in GetInputValves())
        {
            totalFlowRate += valve.GetWaterAvailable();
        }

        foreach (Pipe pipe in _outflowPipes)
        {
            pipe.SetWater(totalFlowRate > 0);
        }
        return totalFlowRate;
    }

    private float FindTemperatureByFlow(float flow)
    {
        if (!ScenarioController.Instance.HasSettings()) { return 0; }

        Dictionary<int, int> flowTemperatures = ScenarioController.Instance.Settings.FlowTemperatures;

        // Sort the keys to and then find the two temperatures that the flow rate is between
        var sortedKeys = flowTemperatures.Keys.OrderBy(k => k).ToList();

        int lowerFlow = sortedKeys[0];
        int upperFlow = sortedKeys[sortedKeys.Count - 1];

        for (int i = 0; i < sortedKeys.Count; i++)
        {
            if (sortedKeys[i] <= flow)
            {
                lowerFlow = sortedKeys[i];
            }
            if (sortedKeys[i] >= flow && i > 0)
            {
                upperFlow = sortedKeys[i];
                break;
            }
        }

        // If the flow rate is exactly one of the temperatures, return that temperature
        if (upperFlow == lowerFlow)
        {
            return flowTemperatures[lowerFlow];
        }

        // Otherwise, interpolate between the two temperatures
        int upperTemp = flowTemperatures[upperFlow];
        int lowerTemp = flowTemperatures[lowerFlow];
        float t = (flow - lowerFlow) / (float)(upperFlow - lowerFlow);
        return Mathf.Lerp(lowerTemp, upperTemp, t);
    }


    private int GetMinFlowRate()
    {
        if (!ScenarioController.Instance.HasSettings()) { return 0; }

        int minFlow = 0;
        Dictionary<int, int> flowTemperatures = ScenarioController.Instance.Settings.FlowTemperatures;
        foreach (int flowRate in flowTemperatures.Keys)
        {
            if (flowRate < minFlow) { minFlow = flowRate; }
        }
        return minFlow;
    }

    public float RecordTemperature()
    {
        UpdateState();
        _graph.AddMarker(_currentTemperature);
        return _currentTemperature;
    }
}
