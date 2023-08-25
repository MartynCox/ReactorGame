using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class ValveFlow : Valve, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color _openColor = Color.green;
    [SerializeField] private Color _closedColor = Color.red;
    [SerializeField] private Color _disabledColor = Color.gray;

    [SerializeField]
    private int _closedAngle = 135;
    [SerializeField]
    private int _maxAngle = -45;

    [SerializeField] private int _maximumFlowRate = 4;
    private RectTransform _handle;
    private TMP_Text _flowRateText;

    private Vector3 _lastMousePosition;
    private int _dragStartFlowRate = 0;
    private bool _isDragging = false;
    [SerializeField] private float _dragSensitivity = 0.3f;
    [SerializeField] private Texture2D _dragPointer;


    private void Start()
    {
        _handle = transform.GetChild(0).GetComponent<RectTransform>();
        _flowRateText = transform.GetChild(1).GetComponent<TMP_Text>();
        
        // Set the rotation to point left (closed)
        _handle.rotation = Quaternion.Euler(0, 0, _closedAngle);
        _handle.GetComponent<UnityEngine.UI.Image>().color = _closedColor;
        IsOpen = false;
        FlowRate = 0;

        UpdateAppearance();
    }

    public void Drag(){
        // Get the mouse position
        Vector3 mousePosition = Input.mousePosition;

        // Get the horizontal distance the mouse has moved
        float mouseDelta = (mousePosition.x - _lastMousePosition.x) * _dragSensitivity;

        // Set the flow rate based on the mouse movement
        FlowRate = _dragStartFlowRate + (int) Mathf.Round(
            mouseDelta / (_closedAngle - _maxAngle) * _maximumFlowRate);
        FlowRate = Mathf.Clamp(FlowRate, 0, _maximumFlowRate);

        IsOpen = FlowRate > 0;

        UpdateAppearance();
        SetPipeAppearance();
    }

    public override void TurnValve()
    {
        if (_isDragging) { return; }

        // Modify the flow rate
        FlowRate = (FlowRate + 1) % (_maximumFlowRate + 1);
        IsOpen = FlowRate > 0;

        UpdateAppearance();
        SetPipeAppearance();
    }

    private void UpdateAppearance(){
        // Set rotation and color
        _handle.rotation = Quaternion.Euler(0, 0, 
            _closedAngle + (_maxAngle - _closedAngle) * FlowRate / _maximumFlowRate);
        _handle.GetComponent<UnityEngine.UI.Image>().color = IsOpen ? _openColor : _closedColor;

        _flowRateText.text = FlowRate.ToString();
    }

    // When the mouse presses on the valve grab and begin to turn
    // public void OnMouseDown()
    // {
    //     _isDragging = true;
    //     _lastMousePosition = Input.mousePosition;
    // }

    public void BeginDrag()
    {
        _isDragging = true;
        _lastMousePosition = Input.mousePosition;
        _dragStartFlowRate = FlowRate;
        Cursor.SetCursor(_dragPointer, new Vector2(23f, 3f), CursorMode.Auto);
    }

    public void EndDrag()
    {
        _isDragging = false;
        Cursor.SetCursor(null, new Vector2(0f, 0f), CursorMode.Auto);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(_dragPointer, new Vector2(23f, 3f), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isDragging){
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
