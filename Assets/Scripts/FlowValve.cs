using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class FlowValve : Valve, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string _name;
    [SerializeField] private Color[] _colours = {
        Color.green, Color.red, Color.grey, Color.black };

    [SerializeField] private int _closedAngle = 135;
    [SerializeField] private int _maxAngle = -45;
    [SerializeField] private int _minimumAngleDifferent = 45;
    [SerializeField] private int _minFlowRate = 0;
    [SerializeField] private int _maxFlowRate = 4;
    [SerializeField] private int _flowInc = 1;
    private int _flowRateDiff;
    private RectTransform _handle;
    private TMP_Text _flowRateText;

    private Vector3 _lastMousePosition;
    private int _dragStartFlowRate = 0;
    private bool _isDragging = false;
    [SerializeField] private float _dragSensitivity = 1.2f;
    [SerializeField] private Texture2D _dragPointer;

    [SerializeField] private Sprite _tickSprite;
    [SerializeField] protected Color TickColour;

    protected override void Start()
    {
        base.Start();

        // Get values from settings if available
        if (GameController.Instance.Settings != null
            && GameController.Instance.Settings.Valves.ContainsKey(_name))
        {
            ValveSettings settings = GameController.Instance.Settings.Valves[_name];
            _maxFlowRate = settings.MaxFlowDisplay;
            _minFlowRate = 0;
            _flowInc = settings.FlowStepSize;
            SetFlowRateThrottle(settings.FlowRatePerStep);

            if (settings.IsBroken)
            {
                SetState(ValveState.Broken);
            }
        }

        // Get the handle and flow rate text
        _handle = transform.GetChild(1).GetComponent<RectTransform>();
        _flowRateText = transform.GetChild(2).GetComponent<TMP_Text>();
        
        // Set the rotation to point left (closed)
        _handle.rotation = Quaternion.Euler(0, 0, _closedAngle);
        _handle.GetComponent<UnityEngine.UI.Image>().color = _colours[(int) ValveState.Closed];

        // Set the max angle for the valve to be open all the way
        _flowRateDiff = _maxFlowRate - _minFlowRate;
        if (_closedAngle - _maxAngle > _minimumAngleDifferent * (_flowRateDiff))
        {
            _maxAngle = _closedAngle - _minimumAngleDifferent * (_flowRateDiff);
        }

        if (_flowRateDiff <= 0)
        {
            _flowRateDiff = 1;
        }

        CreateTickLines();

        // Set flow and update appearance
        SetFlowRate(0, true);
    }

    private void CreateTickLines()
    {
        // Create tick lines
        Transform tickBucket = transform.GetChild(0);

        for (int i = 0; i <= _flowRateDiff; i++)
        {
            // Check if the tick should be written according te the increment
            if (i % _flowInc != 0) { continue; }

            GameObject tick = new GameObject("Tick " + i);
            // Add as child of the valve but underneath the handle
            tick.transform.SetParent(tickBucket);
            tick.transform.localPosition = Vector3.zero;
            tick.transform.localScale = new Vector3(1, 1, 1);
            // -45 is to account for the different starting angle
            tick.transform.rotation = Quaternion.Euler(
                0, 0, -45 + _closedAngle + (_maxAngle - _closedAngle) * i / _flowRateDiff);
            tick.AddComponent<UnityEngine.UI.Image>().color = TickColour;
            tick.GetComponent<UnityEngine.UI.Image>().sprite = _tickSprite;

            // Make anchor stretch in both directions and the offset 0
            RectTransform rectTransform = tick.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0f);
            rectTransform.anchorMax = new Vector2(1, 1f);
            rectTransform.offsetMin = new Vector2(0, 0);
            rectTransform.offsetMax = new Vector2(0, 0);
        }
    }

    public void Drag(){
        if (!_isDragging) { return; }

        // Get the horizontal distance the mouse has moved
        Vector3 mousePosition = Input.mousePosition;

        float mouseDelta = (mousePosition.x - _lastMousePosition.x) * _dragSensitivity;

        // int newRate = _dragStartFlowRate + (int) Mathf.Round(
            // mouseDelta / (_closedAngle - _maxAngle) * _flowRateDiff);
        
        // Set the flow rate based on the mouse movement, rounding to the nearest increment
        int newRate = _dragStartFlowRate + (int) Mathf.Round(
            mouseDelta / (_closedAngle - _maxAngle) * _flowRateDiff / _flowInc) * _flowInc;
        
        // Clamp the flow rate
        newRate += _minFlowRate;
        newRate = Mathf.Clamp(newRate, _minFlowRate, _maxFlowRate);
        SetFlowRate(newRate, false);
    }

    public override void TurnValve()
    {
        if (!IsInteractable) { return; }
        if (_isDragging) { return; }

        // Modify the flow rate
        int newRate = GetFlowRate() + _flowInc;
        if (newRate > _maxFlowRate) { newRate = _minFlowRate; }
        SetFlowRate(newRate, false);
    }

    public override void UpdateAppearance(){
        base.UpdateAppearance();

        // Set rotation and color
        _handle.rotation = Quaternion.Euler(0, 0, 
            _closedAngle + (_maxAngle - _closedAngle) * (GetFlowRate() - _minFlowRate) / _flowRateDiff);
        _handle.GetComponent<UnityEngine.UI.Image>().color = _colours[(int) GetState()];

        // Only display the absolute value of the flow rate
        _flowRateText.text = Mathf.Abs(GetFlowRate()).ToString();
    }

    public void BeginDrag()
    {
        // If the valve is disabled or broken, don't drag
        if (!IsInteractable) { return; }

        _isDragging = true;
        _lastMousePosition = Input.mousePosition;
        _dragStartFlowRate = GetFlowRate() - _minFlowRate;
        Cursor.SetCursor(_dragPointer, new Vector2(11f, 1.5f), CursorMode.Auto);
    }

    public void EndDrag()
    {
        _isDragging = false;
        Cursor.SetCursor(null, new Vector2(0f, 0f), CursorMode.Auto);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If the valve is disabled or broken, don't change the cursor
        if (!IsInteractable) { return; }

        Cursor.SetCursor(_dragPointer, new Vector2(11f, 1.5f), CursorMode.Auto);
        // Make the handle darker but with full alpha
        _handle.GetComponent<UnityEngine.UI.Image>().color = 
            _colours[(int) GetState()] * 0.8f + new Color(0, 0, 0, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isDragging){
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        // Reset the handle color
        _handle.GetComponent<UnityEngine.UI.Image>().color = _colours[(int) GetState()];
    }
}
