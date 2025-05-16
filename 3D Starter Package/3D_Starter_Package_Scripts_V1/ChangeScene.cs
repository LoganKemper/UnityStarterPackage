// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Change the current scene by index or by name.
    /// </summary>
    public class ChangeScene : MonoBehaviour
    {
        // Call one of these methods via a UnityEvent to change the current scene

        public void LoadSceneByName(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void LoadSceneByIndex(int index)
        {
            SceneManager.LoadScene(index);
        }

        public void LoadNextScene()
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
            }
            else
            {
                Debug.LogWarning("No more scenes to load. Already at the last scene.");
            }
        }

        public void LoadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}