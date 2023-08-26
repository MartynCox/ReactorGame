using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    private RectTransform _water;
    [SerializeField] private int _maxCapacity = 8;
    [SerializeField] private int _capacity = 0;

    [SerializeField] private float _lineThickness = 0.02f;
    [SerializeField] private int _fillingValves = 0;
    [SerializeField] private int _drainingValves = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        _water = transform.GetChild(0).GetComponent<RectTransform>();
        UpdateWaterDisplay();

        // Create the line markings
        for (int i = 1; i < _maxCapacity; i++)
        {
            GameObject line = new GameObject("Line");
            line.transform.SetParent(transform);
            RectTransform lineTransform = line.AddComponent<RectTransform>();
            lineTransform.anchorMin = new Vector2(0, (float)i / _maxCapacity - _lineThickness/2f);
            lineTransform.anchorMax = new Vector2(0.2f, (float)i / _maxCapacity + _lineThickness/2f);
            lineTransform.offsetMin = new Vector2(0, 0);
            lineTransform.offsetMax = new Vector2(0, 0);
            lineTransform.pivot = new Vector2(0.5f, 0.5f);
            lineTransform.localScale = new Vector3(1, 1, 1);
            lineTransform.anchoredPosition = new Vector2(0, 0);
            line.AddComponent<UnityEngine.UI.Image>().color = Color.black;
        }
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

    public bool CanFill(){
        return _drainingValves == 0;
    }

    public bool CanDrain(){
        return _fillingValves == 0;
    }

    public int AddFillingValve()
    {
        _fillingValves++;
        return _fillingValves;
    }

    public int RemoveFillingValve()
    {
        _fillingValves--;
        return _fillingValves;
    }

    public int AddDrainingValve()
    {
        _drainingValves++;
        return _drainingValves;
    }

    public int RemoveDrainingValve()
    {
        _drainingValves--;
        return _drainingValves;
    }
}
