// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Provides functionality for closing the game.
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
    }
}