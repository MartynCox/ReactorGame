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
        Debug.Log("flow valve with name " + gameObject.name);

        // Transfer as much water as is available, if no input tanks, assume unlimited water
        int flowed = -FlowRate;
        foreach (TankScript t in _inflowTanks)
        {
            flowed = t.AddWater(-FlowRate);
        }

        foreach (TankScript t in _outflowTanks)
        {
            // Only transfer water if there is enough
            t.AddWater(-flowed);
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

    public void SetFlowRate(int flowRate, bool isForcedUpdate)
    {
        FlowRate = flowRate;

        if (!isForcedUpdate && !IsInteractable) { return; };

        // Set state to open or closed
        ValveState oldState = _state;
        _state = FlowRate == 0 ? ValveState.Closed : ValveState.Open;

        UpdateAppearance();

        if (!isForcedUpdate && oldState == _state) { return; }
        UpdateAllTanks();
    }

    private void UpdateAllTanks()
    {
        foreach (TankScript t in _inflowTanks)
        {
            t.UpdateState();
        }
        foreach (TankScript t in _outflowTanks)
        {
            t.UpdateState();
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
    public void SetEnabled(bool isEnabled, string tankName)
    {
        // Don't do anything if the valve is broken
        if (_state == ValveState.Broken) { return; }

        Debug.Log("Valve: " + this.name + "SetEnabled: " + isEnabled);
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
        List<TankScript> tankScripts = new List<TankScript>(_inflowTanks);
        _inflowTanks = new List<TankScript>(_outflowTanks);
        _outflowTanks = new List<TankScript>(tankScripts);

        foreach (TankScript t in _inflowTanks)
        {
            t.ReverseDirection(this, true);
        }

        foreach (TankScript t in _outflowTanks)
        {
            t.ReverseDirection(this, false);
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