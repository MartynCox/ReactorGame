using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BreakablePipe : Pipe
{
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _brokenSprite;

    void Start()
    {
        SetBroken(false);
    }

    public override void SetBroken(bool isBroken)
    {
        GetComponent<Image>().sprite = isBroken ? _brokenSprite : _normalSprite;

        base.SetBroken(isBroken);

        // Start particles
        if (isBroken)
        {
            GetComponent<ParticleSystem>().Play();
        }
    }
}
