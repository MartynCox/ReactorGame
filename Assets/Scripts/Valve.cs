using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class Valve : MonoBehaviour
{
    [SerializeField] private ValveState _state = ValveState.Closed;
    [SerializeField] protected int FlowRate = 1;
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

    public abstract void TurnValve();

    public virtual void UpdateAppearance()
    {
        // Set state
        if (IsInteractable){
            _state = FlowRate > 0 ? ValveState.Open : ValveState.Closed;
        }

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
    }

    protected ValveState GetState()
    {
        return _state;
    }

    protected enum ValveState
    {
        Open,
        Closed,
        Disabled,
        Broken
    }
}