using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveScript : Valve
{
    public override void TurnValve()
    {
        if (!IsInteractable) { return; }

        // Toggle the valve state
        transform.Rotate(0, 0, 45);
        SetFlowRate((GetFlowRate() + 1) % 2);

        UpdateAppearance();
    }
}
