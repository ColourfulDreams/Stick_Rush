using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColorChangeController : MonoBehaviour
{
    public List<Color> _colors;

    [HideInInspector]
    public Color CurrentColor => _colors[CurrentColorId];

    [HideInInspector]
    public UnityAction<Color> ColorChanged;

    private int _currentColorId;

    public int CurrentColorId
    {
        get
        {
            return _currentColorId;
        }
        set
        {
            _currentColorId = value;
            ColorChanged?.Invoke(CurrentColor);
        }
    }

    [SerializeField]
    private Camera main;

    public void OnColorChanged(Color col)
    {
        main.backgroundColor = col;
    }
}
