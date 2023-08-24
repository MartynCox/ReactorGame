using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveScript : MonoBehaviour
{
    private bool _isOpen = false;
    [SerializeField] private int _flowRate = 1;
    [SerializeField] private List<PipeScript> _connectedPipes;
    [SerializeField] private List<TankScript> _inflowTanks;
    [SerializeField] private List<TankScript> _outflowTanks;

    private void Start()
    {
        _isOpen = false; 
    }

    public void turnValve()
    {
        transform.Rotate(0, 0, 45);
        _isOpen = !_isOpen;

        setPipeAppearance();
    }

    public void setPipeAppearance()
    {
        // Set each pipe to be full or not full
        bool waterAvailable = checkWaterAvailable();
        foreach (PipeScript p in _connectedPipes)
        {
            p.SetWater(_isOpen && waterAvailable);
        }
    }

    public void flow()
    {
        // Do nothing if the valve is closed
        if (!_isOpen) { return; }

        // Transfer water
        foreach(TankScript t in _inflowTanks)
        {
            t.AddWater(-_flowRate);
        }
        setPipeAppearance();

        foreach(TankScript t in _outflowTanks)
        {
            t.AddWater(_flowRate);
        }
    }

    private bool checkWaterAvailable()
    {
        // If there are no inflow tanks, assume unlimited water
        if (_inflowTanks.Count == 0) { return true; }

        foreach (TankScript t in _inflowTanks)
        {
            if (t.GetCapacity() > 0)
            {
                return true;
            }
        }

        return false;
    }
}
