using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Reactor : Tank
{
    [SerializeField] private List<Pipe> _outflowPipes;
    [SerializeField] private Slider _temperatureSlider;
    [SerializeField] private float _tempAnimSpeed = 2.5f;
    [SerializeField] private GameObject _targetMarker;
    [SerializeField] private float _currentTemperature;
    [SerializeField] private Color[] _colours;

    void Start()
    {
        // Set up the slider
        float minTemp = 0;
        float maxTemp = GameController.Instance.Settings.FlowTemperatures[GetMinFlowRate()];
        _temperatureSlider.minValue = minTemp;
        _temperatureSlider.maxValue = maxTemp;
        if (minTemp == maxTemp){ return; }

        // Set the target marker
        float targetTemp = GameController.Instance.Settings.TargetTemperature;
        _currentTemperature = targetTemp;
        float sliderHeight = _temperatureSlider.GetComponent<RectTransform>().rect.height;
        Debug.Log("slider height : " + sliderHeight);
        RectTransform markerTransform = _targetMarker.GetComponent<RectTransform>();
        markerTransform.localPosition = new Vector3(
            markerTransform.localPosition.x,
            sliderHeight * (targetTemp - minTemp) / (maxTemp - minTemp),
            markerTransform.localPosition.z
        );

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
        if (targetValue <= GameController.Instance.Settings.TargetTemperature)
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
        float flow = GetFlow();
        _currentTemperature = GetTemperature(flow);

        // Add degree symbol to temperature and update display
        _temperatureSlider.GetComponentInChildren<TMP_Text>().text = _currentTemperature.ToString() + "�";
    }

    private float GetFlow()
    {
        // Count the total flow rate to the reactor
        float totalFlowRate = 0;
        foreach (Valve valve in GetInputValves())
        {
            totalFlowRate += valve.GetWaterAvailable();
        }
        foreach (Valve valve in GetInputValves())
        {
            valve.ResetMockCapacity();
        }

        foreach (Pipe pipe in _outflowPipes)
        {
            pipe.SetWater(totalFlowRate > 0);
        }
        return totalFlowRate;
    }

    private float GetTemperature(float flow)
    {
        int upperFlow = Mathf.CeilToInt(flow);
        int lowerFlow = Mathf.FloorToInt(flow);

        Dictionary<int, int> flowTemperatures = GameController.Instance.Settings.FlowTemperatures;

        // Find max and min flow rates
        int maxFlow = GetMaxFlowRate();
        int minFlow = GetMinFlowRate();

        // Clamp the flow rate to the max and min
        upperFlow = Mathf.Clamp(upperFlow, minFlow, maxFlow);
        lowerFlow = Mathf.Clamp(lowerFlow, minFlow, maxFlow);

        int upperTemp = GameController.Instance.Settings.FlowTemperatures[upperFlow];
        int lowerTemp = GameController.Instance.Settings.FlowTemperatures[lowerFlow];

        // If the flow rate is exactly one of the temperatures, return that temperature
        if (upperFlow == lowerFlow) { return upperTemp; }

        // Otherwise interpolate between the two temperatures
        float t = (flow - lowerFlow) / (upperFlow - lowerFlow);
        return Mathf.Lerp(lowerTemp, upperTemp, t);
    }

    private int GetMinFlowRate()
    {
        int minFlow = 0;
        Dictionary<int, int> flowTemperatures = GameController.Instance.Settings.FlowTemperatures;
        foreach (int flowRate in flowTemperatures.Keys)
        {
            if (flowRate < minFlow) { minFlow = flowRate; }
        }
        return minFlow;
    }

    private int GetMaxFlowRate()
    {
        int maxFlow = 0;
        Dictionary<int, int> flowTemperatures = GameController.Instance.Settings.FlowTemperatures;
        foreach (int flowRate in flowTemperatures.Keys)
        {
            if (flowRate > maxFlow) { maxFlow = flowRate; }
        }
        return maxFlow;
    }

    public void RecordValue()
    {
        float flow = GetFlow();
        UpdateState();
    }
}
