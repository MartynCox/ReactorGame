using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public class TableManager : MonoBehaviour
{
    private GameObject _flowColumn;
    private GameObject _temperatureColumn;
    [SerializeField] private GameObject _tableRowPrefab;
    [SerializeField] private float _rowHeight = 28f;
    [SerializeField] private Color[] _colours = {
        Color.grey, Color.green, Color.red};

    void Start()
    {
        _flowColumn = transform.GetChild(0).gameObject;
        _temperatureColumn = transform.GetChild(1).gameObject;

        Color header = _colours[0];
        Color good = _colours[1];
        Color bad = _colours[2];

        AddTableValue("Flow", "Temp", header);

        // Read the data from the settings
        Dictionary<int, int> flowTemperatures = GameController.Instance.Settings.FlowTemperatures;
        int targetTemp = GameController.Instance.Settings.TargetTemperature;

        // Sort the data by flow rate descending
        List<int> flows = new List<int>(flowTemperatures.Keys);
        flows.Sort();
        flows.Reverse();

        // Add the data to the table
        bool isFirst = true;
        foreach (int flow in flows)
        {
            int temperature = flowTemperatures[flow];
            String flowString = flow.ToString();
            if (isFirst){
                isFirst = false;
                flowString += "+";
            }

            if (temperature <= targetTemp)
            {
                AddTableValue(flowString, temperature.ToString(), good);
            }
            else
            {
                AddTableValue(flowString, temperature.ToString(), bad);
            }
        }
    }

    void AddTableValue(string flow, string temperature, Color? color = null)
    {
        // Create the new rows
        GameObject flowText = Instantiate(_tableRowPrefab, _flowColumn.transform);
        flowText.GetComponentInChildren<TMP_Text>().text = flow;
        GameObject temperatureText = Instantiate(_tableRowPrefab, _temperatureColumn.transform);
        temperatureText.GetComponentInChildren<TMP_Text>().text = temperature;

        // Move the new text to the correct position
        flowText.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(0, -_rowHeight * (_flowColumn.transform.childCount - 1));
        temperatureText.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(0, -_rowHeight * (_temperatureColumn.transform.childCount - 1));

        // Set the color if it is given
        if (color != null)
        {
            flowText.GetComponentInChildren<Image>().color = (Color)color;
            temperatureText.GetComponentInChildren<Image>().color = (Color)color;
        }
    }
}
