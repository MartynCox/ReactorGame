using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BidirectionalValve : FlowValve
{
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private Color activeColour = Color.white;
    private Color inactiveColour = Color.grey;

    private bool isReversed = true;

    protected override void Start()
    {
        this.inactiveColour = TickColour;
        base.Start();
    }

    public override void UpdateAppearance()
    {
        base.UpdateAppearance();

        // Update the arrow colours
        if (isReversed)
        {
            leftArrow.color = inactiveColour;
            rightArrow.color = activeColour;
        } else
        {
            leftArrow.color = activeColour;
            rightArrow.color = inactiveColour;
        }
    }

    public override void ReverseDirection()
    {
        isReversed = !isReversed;
        base.ReverseDirection();
    }

    public override void Break()
    {
        // Do not break bidirectional valves
        return;
    }
}