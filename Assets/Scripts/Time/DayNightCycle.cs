using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 24f)]
    public float timeOfDay;

    public Transform sun;

    public Slider slider;
    public Toggle toggle;
    public TextMeshProUGUI timeLabel;

    public enum TimeCategory
    {
        Morning,
        Noon,
        Night
    }

    public float morningEnd;
    public float noonEnd;
    public float nightEnd;

    public static TimeCategory? currentTimeCategory = null;
    private TimeCategory? previousTimeCategory = null;

    public static UnityEvent onTimeCategoryChanged = new UnityEvent();
    public static UnityEvent preTimeCategoryChanged = new UnityEvent();

    private bool advanceTime = true;

    public void Start()
    {
        timeOfDay = Random.Range(0f, 24f);
        UpdateTime();
        slider.value = timeOfDay / 24f;
        slider.onValueChanged.AddListener(SetTimeFromSlider);
        toggle.onValueChanged.AddListener(SetAdvanceTime);
    }

    // Updates sun and time category when time is changed
    public void UpdateTime()
    {
        UpdateSun();
        UpdateTimeCategory();
    }

    public void Update()
    {
        AdvanceTime();
        UpdateTime();
    }

    public void AdvanceTime()
    {
        // Advance time of day gradually
        if (advanceTime)
        {
            timeOfDay += Time.deltaTime * 0.5f;

            // Wrap time
            if (timeOfDay > 24f)
            {
                timeOfDay -= 24f;
            }

            // Set on slider
            slider.value = timeOfDay / 24f;
        }
    }

    // Set time of day when slider is changed
    public void SetTimeFromSlider(float normalizedTime)
    {
        timeOfDay = normalizedTime * 24f;
    }

    // Set advance when toggle is pressed
    public void SetAdvanceTime(bool advanceTime)
    {
        this.advanceTime = advanceTime;
    }

    // Run events when time category changed
    private void OnTimeCategoryChanged()
    {
        preTimeCategoryChanged.Invoke();
        onTimeCategoryChanged.Invoke();
        timeLabel.text = "Time: " + currentTimeCategory;
    }

    // Check if time category has changed
    private void UpdateTimeCategory()
    {
        previousTimeCategory = currentTimeCategory;
        currentTimeCategory = GetCurrentTimeCategory();

        if (previousTimeCategory != currentTimeCategory)
        {
            OnTimeCategoryChanged();
        }
    }

    // Get the current time category based on time of day
    private TimeCategory GetCurrentTimeCategory()
    {
        if (timeOfDay >= nightEnd && timeOfDay < morningEnd)
        {
            return TimeCategory.Morning;
        }
        else if (timeOfDay >= morningEnd && timeOfDay < noonEnd)
        {
            return TimeCategory.Noon;
        }
        return TimeCategory.Night;
    }

    // Update sun position based on time
    private void UpdateSun()
    {
        float normalizedTime = timeOfDay / 24f;
        float sunAngle = (normalizedTime * 360f) - 90f;

        sun.rotation = Quaternion.Euler(sunAngle, 0f, 0f);
    }

    public void OnValidate()
    {
        UpdateTime();
    }
}
