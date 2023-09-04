using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveScript : Valve
{
    [SerializeField]
    private Color[] _colours = {
        Color.green, Color.red, Color.grey, Color.black };

    public override void TurnValve()
    {
        if (!IsInteractable) { return; }

        // Toggle the valve state
        transform.Rotate(0, 0, 45);
        SetFlowRate((GetFlowRate() + 1) % 2);
    }

    public override void UpdateAppearance()
    {
        base.UpdateAppearance();

        // Set rotation and color
        GetComponent<UnityEngine.UI.Image>().color = _colours[(int)GetState()];

    }

}
