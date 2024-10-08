using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class Valve : MonoBehaviour
{
    [SerializeField] private ValveState _state = ValveState.Closed;
    private int flowRate = 0;
    [SerializeField] private List<Pipe> _connectedPipes;
    [SerializeField] private Tank _inflowTank;
    [SerializeField] private Tank _outflowTank;
    [SerializeField] private float _flowRateThrottle = 1.0f;
    private List<string> _disabledBy = new List<string>();

    public bool IsOpen {
        get { return _state == ValveState.Open; }
    }

    public bool IsInteractable {
        get { return (
            _state != ValveState.Disabled
            && _state != ValveState.Broken
        ); }
    }

    protected virtual void Start()
    {
        // Add this valve to the inflow and outflow tanks
        if (_inflowTank != null) { _inflowTank.AddOutputValve(this); }

        if (_outflowTank != null) { _outflowTank.AddInputValve(this); }
    }

    public abstract void TurnValve();

    public virtual void UpdateAppearance()
    {
        // Set each pipe to be full or not full
        bool waterAvailable = IsWaterAvailable();
        foreach (Pipe p in _connectedPipes)
        {
            p.SetWater(IsOpen && waterAvailable);
        }
    }

    public void Flow()
    {
        // Do nothing if the valve is closed
        if (!IsOpen) { return; }

        // Transfer as much water as is available, if no input tanks, assume unlimited water
        float flowed = -flowRate * _flowRateThrottle;
        if (_inflowTank != null)
        {
            flowed = _inflowTank.AddWater(flowed);
        }

        if (_outflowTank != null)
        {
            _outflowTank.AddWater(-flowed);
        }
    }

    public virtual void Break()
    {
        if ( GetState() == ValveState.Broken){ return;}

        // Set the state to broken
        _state = ValveState.Broken;
        SetFlowRate(0, true);

        // Break pipes
        foreach (Pipe p in _connectedPipes)
        {
            p.SetBroken(true);
        }
    }

    public bool IsWaterAvailable()
    {
        if (_inflowTank == null) { return true; }

        return _inflowTank.GetCapacity() > 0;

    }

    public float GetWaterAvailable()
    {
        // If there are no inflow tanks, assume unlimited water
        if (_inflowTank == null) { return float.MaxValue; }

        float waterAvailable = Mathf.Min(_inflowTank.GetMockCapacity(), flowRate * _flowRateThrottle);
        _inflowTank.MockDrain(waterAvailable);
        return waterAvailable;
    }

    public void ResetMockCapacity()
    {
        if (_inflowTank == null) { return; }

        _inflowTank.ResetMockCapacity();
    }

    public void SetFlowRate(int flowRate, bool isForcedUpdate)
    {
        int oldFlowRate = this.flowRate;
        this.flowRate = flowRate;

        if (!isForcedUpdate && !IsInteractable) { return; };

        // Set state to open or closed
        ValveState oldState = _state;
        if (_state != ValveState.Broken) {
            _state = this.flowRate == 0 ? ValveState.Closed : ValveState.Open;
        }

        UpdateAppearance();

        if (!isForcedUpdate && oldState == _state && oldFlowRate == flowRate) { return; }
        UpdateAllTanks();
    }

    private void UpdateAllTanks()
    {
        if (_inflowTank != null)
        {
            _inflowTank.UpdateState();
        }
        if (_outflowTank != null)
        {
            _outflowTank.UpdateState();
        }
    }

    public int GetFlowRate()
    {
        return flowRate;
    }

    protected ValveState GetState()
    {
        return _state;
    }

    protected void SetState(ValveState state)
    {
        _state = state;
    }

    public float GetFlowRateThrottle()
    {
        return _flowRateThrottle;
    }

    protected void SetFlowRateThrottle(float throttle)
    {
        _flowRateThrottle = throttle;
    }

    public void SetEnabled(bool isEnabled, string tankName)
    {
        // Don't do anything if the valve is broken
        if (_state == ValveState.Broken) { return; }

        if (isEnabled)
        {
            _disabledBy.Remove(tankName);
            // Make sure the valve is not being disabled by someone else
            if (_disabledBy.Count > 0) { return; }

            // If the valve is disabled, set it to closed otherwise do nothing
            if (_state == ValveState.Disabled)
            {
                _state = ValveState.Closed;
            }
        }
        else
        {
            _state = ValveState.Disabled;
            if (!_disabledBy.Contains(tankName))
                _disabledBy.Add(tankName);
        }

        UpdateAppearance();
    }

    public virtual void ReverseDirection()
    {
        // Swap the inflow and outflow tanks
        Tank temp = _inflowTank;
        _inflowTank = _outflowTank;
        _outflowTank = temp;

        if (_inflowTank != null)
        {
            _inflowTank.ReverseDirection(this, true);
        }

        if (_outflowTank != null)
        {
            _outflowTank.ReverseDirection(this, false);
        }

        // Set the flow rate to 0
        SetFlowRate(0, true);
    }

    protected enum ValveState
    {
        Open,
        Closed,
        Disabled,
        Broken
    }
}