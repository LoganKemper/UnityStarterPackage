// Unity Starter Package - Version 3
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Provides functionality for quitting and pausing the game.
    /// </summary>
    public class ApplicationController : MonoBehaviour
    {
        [System.Serializable]
        public class Events
        {
            [Space(20)]
            public UnityEvent onPaused, onUnpaused;
        }

        [Header("Settings")]
        [Tooltip("If true, the standalone application will close when the quit action is triggered.")]
        [SerializeField] private bool quitGameOnEscape = true;

        [Tooltip("The input action used for quitting the game. Set to the escape key by default.")]
        [SerializeField] private InputAction quitAction = new("Quit", InputActionType.Button, "<Keyboard>/escape");

        [Tooltip("The input action used for pausing the game. Set to the P key by default.")]
        [SerializeField] private InputAction pauseAction = new("Pause", InputActionType.Button, "<Keyboard>/p");

        [Header("UnityEvents")]
        [SerializeField] private Events events;

        // This static boolean can be checked anywhere else in the code with ApplicationController.GameIsPaused
        public static bool GameIsPaused { get; private set; } = false;

        private void OnEnable()
        {
            quitAction.Enable();
            pauseAction.Enable();
        }

        private void OnDisable()
        {
            quitAction.Disable();
            pauseAction.Disable();
        }

        // This script makes use of "conditional compilation".
        // That means that you can write code that only compiles into the game only if certain conditions are met.
        // In this example, different code executes whether the game is running in the editor or as a standalone application.

        // For more information, look up conditional compilation (and more broadly, "preprocessor directives") in the Unity docs!

        private void Update()
        {
            if (quitAction.WasPressedThisFrame() && quitGameOnEscape)
            {
#if !UNITY_EDITOR && !UNITY_WEBGL
                QuitGame();
#endif
            }

            if (pauseAction.WasPressedThisFrame())
            {
                TogglePause();
            }
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Game quit! Exiting play mode...");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Log("Game quit! Closing application...");
            Application.Quit();
#endif
        }

        [ContextMenu("Toggle Pause")]
        public void TogglePause()
        {
            PauseGame(!GameIsPaused);
        }

        public void PauseGame(bool paused)
        {
            GameIsPaused = paused;

            // Changing Time.timeScale affects the speed at which in-game time passes.
            // When timeScale is 0, physics, animations, and more will be frozen.

            if (paused)
            {
                Time.timeScale = 0f;
                events.onPaused.Invoke();
            }
            else
            {
                Time.timeScale = 1f;
                events.onUnpaused.Invoke();
            }
        }
    }
}
