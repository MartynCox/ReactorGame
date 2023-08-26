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

    [SerializeField] private int _closedAngle = 135;
    [SerializeField] private int _maxAngle = -45;
    [SerializeField] private int _minimumAngleDifferent = 45;
    [SerializeField] private int _maximumFlowRate = 4;
    private RectTransform _handle;
    private TMP_Text _flowRateText;

    private Vector3 _lastMousePosition;
    private int _dragStartFlowRate = 0;
    private bool _isDragging = false;
    [SerializeField] private float _dragSensitivity = 1.2f;
    [SerializeField] private Texture2D _dragPointer;

    [SerializeField] private Sprite _tickSprite;
    [SerializeField] private Color _tickColour;

    private void Start()
    {
        Transform tickBucket = transform.GetChild(0);
        _handle = transform.GetChild(1).GetComponent<RectTransform>();
        _flowRateText = transform.GetChild(2).GetComponent<TMP_Text>();
        
        // Set the rotation to point left (closed)
        _handle.rotation = Quaternion.Euler(0, 0, _closedAngle);
        _handle.GetComponent<UnityEngine.UI.Image>().color = _closedColor;
        IsOpen = false;
        FlowRate = 0;

        // Set the max angle for the valve to be open all the way
        if (_closedAngle - _maxAngle > _minimumAngleDifferent * (_maximumFlowRate))
        {
            _maxAngle = _closedAngle - _minimumAngleDifferent * (_maximumFlowRate);
        }

        // Create tick lines
        for (int i = 0; i <= _maximumFlowRate; i++)
        {
            GameObject tick = new GameObject("Tick " + i);
            // Add as child of the valve but underneath the handle
            tick.transform.SetParent(tickBucket);
            tick.transform.localPosition = Vector3.zero;
            tick.transform.localScale = new Vector3(1, 1, 1);
            // -45 is to account for the different starting angle
            tick.transform.rotation = Quaternion.Euler(
                0, 0, -45 + _closedAngle + (_maxAngle - _closedAngle) * i / _maximumFlowRate);
            tick.AddComponent<UnityEngine.UI.Image>().color = _tickColour;
            tick.GetComponent<UnityEngine.UI.Image>().sprite = _tickSprite;

            // Make anchor stretch in both directions and the offset 0
            RectTransform rectTransform = tick.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0f);
            rectTransform.anchorMax = new Vector2(1, 1f);
            rectTransform.offsetMin = new Vector2(0, 0);
            rectTransform.offsetMax = new Vector2(0, 0);
        }

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
    }

    public override void TurnValve()
    {
        if (_isDragging) { return; }

        // Modify the flow rate
        FlowRate = (FlowRate + 1) % (_maximumFlowRate + 1);
        IsOpen = FlowRate > 0;

        UpdateAppearance();
    }

    private void UpdateAppearance(){
        // Set rotation and color
        _handle.rotation = Quaternion.Euler(0, 0, 
            _closedAngle + (_maxAngle - _closedAngle) * FlowRate / _maximumFlowRate);
        _handle.GetComponent<UnityEngine.UI.Image>().color = IsOpen ? _openColor : _closedColor;

        _flowRateText.text = FlowRate.ToString();

        SetPipeAppearance();
    }

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
