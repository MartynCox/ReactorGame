using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveScript : Valve
{
    public override void TurnValve()
    {
        transform.Rotate(0, 0, 45);
        IsOpen = !IsOpen;

        SetPipeAppearance();
    }
}
