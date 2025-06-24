using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * TimeScaleController Script
 *
 * This script allows the user to control the time scale of the simulation via a UI slider. 
 *
 * Methods:
 * - Start(): Initializes the slider's range, sets the default value, and assigns a listener for value changes.
 * - OnTimeScaleChanged(): Adjusts the time scale based on the slider's value and updates the display text.
 * - UpdateTimeScaleText(): Updates the UI text to reflect the current time scale value.
 */


public class TimeScaleController : MonoBehaviour
{
    public Slider timeScaleSlider;
    public TMP_Text timeScaleText;

    void Start()
    {
        timeScaleSlider.minValue = 0.1f;
        timeScaleSlider.maxValue = 10.0f;
        timeScaleSlider.value = 1.0f; // Set initial value to 1.0
        timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);
        UpdateTimeScaleText(1.0f);
    }

    void OnTimeScaleChanged(float value)
    {
        Time.timeScale = value;
        UpdateTimeScaleText(value);
    }

    void UpdateTimeScaleText(float value)
    {
        timeScaleText.text = $"Time Scale: {value:F1}x";
    }
}
