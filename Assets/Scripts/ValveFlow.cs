using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ValveFlow : Valve
{
    [SerializeField] private Color _openColor = Color.green;
    [SerializeField] private Color _closedColor = Color.red;
    [SerializeField] private Color _disabledColor = Color.gray;

    [SerializeField]
    private int _closedAngle = 135;
    [SerializeField]
    private int _openAngle = -45;

    [SerializeField] private int _maximumFlowRate = 4;
    private RectTransform _handle;

    private void Start()
    {
        IsOpen = false;
        FlowRate = 0;
        _handle = transform.GetChild(0).GetComponent<RectTransform>();
        // Set the rotation to point left (closed)
        _handle.rotation = Quaternion.Euler(0, 0, _closedAngle);
        _handle.GetComponent<UnityEngine.UI.Image>().color = _closedColor;
    }

    public override void TurnValve()
    {
        // Toggle flow rate
        FlowRate = (FlowRate + 1) % (_maximumFlowRate + 1);
        IsOpen = FlowRate > 0;

        // Set rotation and color
        _handle.rotation = Quaternion.Euler(0, 0, 
            _closedAngle + (_openAngle - _closedAngle) * FlowRate / _maximumFlowRate);
        _handle.GetComponent<UnityEngine.UI.Image>().color = IsOpen ? _openColor : _closedColor;

        SetPipeAppearance();
    }
}
