// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Generic script for counting down time. Can be used to display a timer on the UI, or just to delay an action by a specified amount of time.
/// </summary>
public class Timer : MonoBehaviour
{
    [Tooltip("How many seconds the timer lasts for.")]
    [SerializeField] private float timerSeconds = 5f;

    [Tooltip("Optional: Assign a text component to display the timer on the UI.")]
    [SerializeField] private TMP_Text timerText;

    [Tooltip("How many numbers after the decimal place on the timer text.")]
    [SerializeField] private int decimalPlaces = 2;

    [Space(20)]
    [SerializeField] private UnityEvent onTimerFinished;

    private float timer = 0f;
    private bool timerInProgress;

    // Call from a UnityEvent to begin the timer
    public void StartTimer()
    {
        timer = timerSeconds;
        timerInProgress = true;
        UpdateTimerDisplay();
    }

    // Call from a UnityEvent to stop the timer early
    public void StopTimer()
    {
        timerInProgress = false;
        timer = 0f;
    }

    private void Update()
    {
        if (timerInProgress)
        {
            if (timer >= 0)
            {
                timer -= Time.deltaTime;
                timer = Mathf.Max(0f, timer);
            }
            else
            {
                onTimerFinished.Invoke();
                StopTimer();
            }

            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(timer, decimalPlaces);
        }
    }

    // FormatTime returns a nicely formatted string of the time remaining in minutes, seconds, and specified decimal places
    private string FormatTime(float time, int decimalPlaces)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;

        if (minutes == 0)
        {
            // Hide minutes if time is 60 seconds or less
            return decimalPlaces == 0
                ? $"{Mathf.FloorToInt(seconds):0}"
                : seconds.ToString($"F{decimalPlaces}");
        }
        else
        {
            return decimalPlaces == 0
                ? $"{minutes:00}:{Mathf.FloorToInt(seconds):00}"
                : $"{minutes:00}:{Mathf.FloorToInt(seconds):00}.{(seconds % 1).ToString($"F{decimalPlaces}")}".Replace("0.", "");
        }
    }

    private void OnValidate()
    {
        // Enforce minimum values
        timerSeconds = Mathf.Max(0f, timerSeconds);
        decimalPlaces = Mathf.Max(0, decimalPlaces);
    }
}
