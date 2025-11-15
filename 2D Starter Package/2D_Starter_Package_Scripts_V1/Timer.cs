// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Generic script for counting down time. Can be used to display a timer on the UI, or just to delay an action by a specified amount of time.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        private enum TimerDirection : byte
        {
            CountUp,
            CountDown
        }

        [Tooltip("How many seconds the timer lasts for.")]
        [SerializeField] private float timerSeconds = 5f;

        [Tooltip("Optional: Assign a text component to display the timer on the UI.")]
        [SerializeField] private TMP_Text timerText;

        [Tooltip("Optional: Prefix written before the timer text.")]
        [SerializeField] private string timerTextPrefix = "Timer: ";

        [Tooltip("How many numbers after the decimal place on the timer text.")]
        [SerializeField] private int decimalPlaces = 2;

        [Tooltip("Choose whether the timer should count up or down.")]
        [SerializeField] private TimerDirection timerDirection = TimerDirection.CountDown;

        [Tooltip("Choose whether the timer should begin right away (in the component's Start method).")]
        [SerializeField] private bool startTimerOnStart = false;

        [Space(20)]
        [SerializeField] private UnityEvent onTimerFinished;

        private float timer;
        private bool timerInProgress;
        private bool isPaused;

        private void Start()
        {
            if (startTimerOnStart)
            {
                StartTimer();
            }
        }

        // Call from a UnityEvent to begin the timer
        [ContextMenu("Start Timer")]
        public void StartTimer()
        {
            timerInProgress = true;
            isPaused = false;

            if (timerDirection == TimerDirection.CountDown)
            {
                timer = timerSeconds;
            }
            else if (timerDirection == TimerDirection.CountUp)
            {
                timer = 0f;
            }

            UpdateTimerDisplay();
        }

        // Call from a UnityEvent to stop the timer early
        [ContextMenu("Stop Timer")]
        public void StopTimer()
        {
            timerInProgress = false;
            isPaused = false;

            if (timerDirection == TimerDirection.CountDown)
            {
                timer = 0f;
            }
            else if (timerDirection == TimerDirection.CountUp)
            {
                timer = timerSeconds;
            }

            UpdateTimerDisplay();
        }

        // Call from a UnityEvent to pause the timer without resetting it
        [ContextMenu("Pause Timer")]
        public void PauseTimer()
        {
            if (timerInProgress && !isPaused)
            {
                isPaused = true;
            }
        }

        // Call from a UnityEvent to resume the paused timer
        [ContextMenu("Resume Timer")]
        public void ResumeTimer()
        {
            if (timerInProgress && isPaused)
            {
                isPaused = false;
            }
        }

        private void Update()
        {
            if (!timerInProgress || isPaused)
            {
                return;
            }

            if (timerDirection == TimerDirection.CountDown)
            {
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    timer = 0f;
                    UpdateTimerDisplay();
                    onTimerFinished.Invoke();
                    StopTimer();
                    return;
                }
            }
            else if (timerDirection == TimerDirection.CountUp)
            {
                timer += Time.deltaTime;

                if (timer >= timerSeconds)
                {
                    timer = timerSeconds;
                    UpdateTimerDisplay();
                    onTimerFinished.Invoke();
                    StopTimer();
                    return;
                }
            }

            UpdateTimerDisplay();
        }

        public void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                timerText.text = timerTextPrefix + FormatTime(timer, decimalPlaces);
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
                    : $"{minutes:00}:{Mathf.FloorToInt(seconds):00}." +
                      $"{(seconds % 1).ToString($"F{decimalPlaces}")}".Replace("0.", "");
            }
        }

        private void OnValidate()
        {
            // Enforce minimum values
            timerSeconds = Mathf.Max(0f, timerSeconds);
            decimalPlaces = Mathf.Max(0, decimalPlaces);
        }
    }
}