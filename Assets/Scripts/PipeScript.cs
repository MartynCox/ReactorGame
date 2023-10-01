using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PipeScript : MonoBehaviour
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
            if (p.GetComponent<PipeScript>() == null) { continue; }
            
            p.GetComponent<PipeScript>().SetWater(isWater);
        }
    }
}
