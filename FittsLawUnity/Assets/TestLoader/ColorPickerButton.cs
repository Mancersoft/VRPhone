using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerButton : MonoBehaviour
{
    [SerializeField] private CUIColorPicker _colorPicker;

    private Image _background;
    private Color _currentColour;
    public Color CurrentColour
    {
        get { return _currentColour; }
        set
        {
            _currentColour = value;
            _background.color = value;
        }
    }

    void Awake()
    {
        _background = GetComponent<Image>();
        CurrentColour = Color.red;
    }

    public void OnButtonClicked()
    {
        if (_colorPicker.gameObject.activeInHierarchy) return;
        _colorPicker.Color = _currentColour;
        _colorPicker.gameObject.SetActive(true);
    }

    public void OnSubmitClicked()
    {
        CurrentColour = _colorPicker.Color;
        _colorPicker.gameObject.SetActive(false);
    }

    public void OnCancelClicked()
    {
        _colorPicker.gameObject.SetActive(false);
    }
}
