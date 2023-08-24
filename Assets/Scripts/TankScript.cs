using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    private RectTransform _water;
    [SerializeField] private int _maxCapacity = 8;
    [SerializeField] private int _capacity = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        _water = transform.GetChild(0).GetComponent<RectTransform>();
        UpdateWaterDisplay();
    }

    public int AddWater(int flowAmount)
    {
        int lastCapacity = _capacity;
        _capacity = Mathf.Clamp(_capacity + flowAmount, 0, _maxCapacity);
        UpdateWaterDisplay();
        return _capacity;
    }

    private void UpdateWaterDisplay()
    {
        _water.anchorMax = new Vector2(1, (float)_capacity / _maxCapacity - 0.001f);
    }

    public int GetCapacity()
    {
        return _capacity;
    }
}
