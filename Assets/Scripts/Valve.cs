using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class Valve : MonoBehaviour
{
    [SerializeField] private ValveState _state = ValveState.Closed;
    [SerializeField] private int FlowRate = 1;
    [SerializeField] private List<PipeScript> _connectedPipes;
    [SerializeField] private List<TankScript> _inflowTanks;
    [SerializeField] private List<TankScript> _outflowTanks;

    public bool IsOpen {
        get { return _state == ValveState.Open; }
    }

    public bool IsInteractable {
        get { return (
            _state != ValveState.Disabled
            && _state != ValveState.Broken
        );}
    }

    protected virtual void Start()
    {
        // Add this valve to the inflow and outflow tanks
        foreach (TankScript t in _inflowTanks)
        {
            t.AddOutputValve(this);
        }

        foreach (TankScript t in _outflowTanks)
        {
            t.AddInputValve(this);
        }
    }

    public abstract void TurnValve();

    public virtual void UpdateAppearance()
    {
        // Set each pipe to be full or not full
        bool waterAvailable = CheckWaterAvailable();
        foreach (PipeScript p in _connectedPipes)
        {
            p.SetWater(IsOpen && waterAvailable);
        }
    }

    public void Flow()
    {
        // Do nothing if the valve is closed
        if (!IsOpen) { return; }

        // Transfer water
        foreach(TankScript t in _inflowTanks)
        {
            t.AddWater(-FlowRate);
        }

        foreach(TankScript t in _outflowTanks)
        {
            t.AddWater(FlowRate);
        }
    }

    private bool CheckWaterAvailable()
    {
        // If there are no inflow tanks, assume unlimited water
        if (_inflowTanks.Count == 0) { return true; }

        foreach (TankScript t in _inflowTanks)
        {
            if (t.GetCapacity() == 0) { return false; }
        }

        return true;
    }

    public void SetFlowRate(int flowRate)
    {
        FlowRate = flowRate;
           
        ValveState oldState = _state;
        // Set state to open or closed
        if (IsInteractable)
        {
            _state = FlowRate > 0 ? ValveState.Open : ValveState.Closed;
        }

        if (oldState == _state) { return; }
        
        // Disable the tank filling or draining if the valve state changed
        if (_state == ValveState.Open)
        {
            foreach (TankScript t in _inflowTanks)
            {
                t.DisableFilling();
            }
            foreach (TankScript t in _outflowTanks)
            {
                t.DisableDraining();
            }
        }
        else if (_state == ValveState.Closed)
        {
            foreach (TankScript t in _inflowTanks)
            {
                t.EnableFilling();
            }
            foreach (TankScript t in _outflowTanks)
            {
                t.EnableDraining();
            }
        }
    }

    public int GetFlowRate()
    {
        return FlowRate;
    }

    protected ValveState GetState()
    {
        return _state;
    }
    public void SetEnabled(bool isEnabled)
    {
        // Don't do anything if the valve is broken
        if (_state == ValveState.Broken) { return; }

        if (isEnabled)
        {
            // If the valve is disabled, set it to closed otherwise do nothing
            if (_state == ValveState.Disabled)
            {
                _state = ValveState.Closed;
            }
        }
        else
        {
            _state = ValveState.Disabled;
        }

        UpdateAppearance();
    }

    protected enum ValveState
    {
        Open,
        Closed,
        Disabled,
        Broken
    }
}