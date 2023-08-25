using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class Valve : MonoBehaviour
{
    protected bool IsOpen = false;
    [SerializeField] protected int FlowRate = 1;
    [SerializeField] private List<PipeScript> _connectedPipes;
    [SerializeField] private List<TankScript> _inflowTanks;
    [SerializeField] private List<TankScript> _outflowTanks;

    public abstract void TurnValve();

    private void Start()
    {
        IsOpen = false;
    }

    public void SetPipeAppearance()
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
    }
}