using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactorGraph : MonoBehaviour
{
    [SerializeField] private GameObject _graphMarker;
    [SerializeField] private Color[] _colours;
    private float _targetTemperature = 120f;
    private float _minTemperature = 0f;
    private float _maxTemperature = 300f;
    private int _totalCycles = 10;
    private List<float> _temperatures = new List<float>();

    public void AddMarker(float temperature)
    {
        _temperatures.Add(temperature);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        // Destroy all children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Add a marker for each temperature
        float spacing_x = GetComponent<RectTransform>().rect.width / (_totalCycles - 1);
        float height = GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < _temperatures.Count; i++)
        {
            Console.WriteLine("Adding marker");
            GameObject marker = Instantiate(_graphMarker, transform);
            RectTransform markerTransform = marker.GetComponent<RectTransform>();
            markerTransform.localPosition = new Vector3(
                    spacing_x * i,
                    height * (_temperatures[i] - _minTemperature) / (_maxTemperature - _minTemperature),
                    markerTransform.localPosition.z
                );
            marker.GetComponent<Image>().color = _temperatures[i] <= _targetTemperature ? _colours[0] : _colours[1];
        }

    }

    public void SetTemps(float targetTemperature, float minTemp, float maxTemp, int totalCycles)
    {
        _targetTemperature = targetTemperature;
        _minTemperature = minTemp;
        _maxTemperature = maxTemp;
        _totalCycles = totalCycles;
    }
}
