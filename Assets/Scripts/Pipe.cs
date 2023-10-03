using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pipe : MonoBehaviour
{
    [SerializeField] private Color _pipeColour;
    [SerializeField] private Color _waterColour;
    [SerializeField] private bool _isWater;

    private void Start()
    {
        SetWater(_isWater);
    }

    public void SetWater(bool isWater)
    {
        _isWater = isWater;
        transform.GetComponent<Image>().color = isWater ? _waterColour : _pipeColour;
        // Set children pipes
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform p = transform.GetChild(i);
            if (p.GetComponent<Pipe>() == null) { continue; }
            
            p.GetComponent<Pipe>().SetWater(isWater);
        }
    }

    public virtual void SetBroken(bool isBroken)
    {
        // Do nothing but break all children pipes
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform p = transform.GetChild(i);
            if (p.GetComponent<Pipe>() == null) { continue; }
            
            p.GetComponent<Pipe>().SetBroken(isBroken);
        }
    }
}
