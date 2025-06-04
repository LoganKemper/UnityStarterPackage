// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Provides functionality for quitting or pausing the game.
    /// </summary>
    public class ApplicationController : MonoBehaviour
    {
        [Tooltip("If true, the standalone application will close when the Esc key is pressed.")]
        [SerializeField] private bool quitGameOnEscape = true;

        // This script makes use of "conditional compilation".
        // That means that you can write code that only compiles into the game only if certain conditions are met.
        // In this example, different code executes whether the game is running in the editor or as a standalone application.

        // For more information, look up conditional compilation (and more broadly, "preprocessor directives") in the Unity docs!

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && quitGameOnEscape)
            {
#if !UNITY_EDITOR
            QuitGame();
#endif
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

        // This boolean can be checked anywhere else in the code with ApplicationController.GameIsPaused
        public static bool GameIsPaused { get; private set; } = false;

        // Call this from a UnityEvent to pause or resume the game
        public void PauseGame(bool paused)
        {
            GameIsPaused = paused;

            // Changing Time.timeScale affects the speed at which in-game time passes.
            // When timeScale is 0, physics, animations, and more will be frozen.

            if (paused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
}