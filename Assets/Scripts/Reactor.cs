using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : Tank
{
    public override int AddWater(int flowAmount)
    {
        // Reactor can't receive water
        return base.AddWater(flowAmount);
    }

    public override void UpdateState()
    {
        // Count the total flow rate to the reactor
        int totalFlowRate = 0;
        foreach (Valve valve in GetInputValves())
        {
            totalFlowRate += valve.GetWaterAvailable();
        }
    }
}
