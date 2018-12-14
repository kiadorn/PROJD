using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour {

    [SerializeField]
    private FloatVariable value;
    [SerializeField]
    private float minValue = 0f;
    [SerializeField]
    private float maxValue = 1f;

    [SerializeField]
    private TextMeshProUGUI valueText;

    private Slider slider;

    public void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = value.Value;
        valueText.text = Math.Round(ConvertSliderValueToUIValue(), 1).ToString();
    }

    public void SetValue()
    {
        value.Value = slider.value;
        valueText.text = Math.Round(ConvertSliderValueToUIValue(), 1).ToString();
    }

    private float ConvertSliderValueToUIValue() {
        if (slider.value <= -39.9f)
        {
            value.Value = -80f;
        }
        return (minValue + (((slider.value - slider.minValue) / (slider.maxValue - slider.minValue)) * (maxValue - minValue)));
    }
}
