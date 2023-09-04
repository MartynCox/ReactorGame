using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValveBidirectional : ValveFlow
{
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private Color activeColour = Color.white;
    private Color inactiveColour = Color.grey;

    protected override void Start()
    {
        this.inactiveColour = TickColour;
        base.Start();
    }

    public override void UpdateAppearance()
    {
        base.UpdateAppearance();

        // Update the arrow colours
        if (GetFlowRate() == 0)
        {
            leftArrow.color = inactiveColour;
            rightArrow.color = inactiveColour;
        } else if (GetFlowRate() > 0)
        {
            leftArrow.color = inactiveColour;
            rightArrow.color = activeColour;
        } else
        {
            leftArrow.color = activeColour;
            rightArrow.color = inactiveColour;
        }
    }
}