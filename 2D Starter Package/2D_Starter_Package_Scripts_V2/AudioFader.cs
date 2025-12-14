// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// A simple component for gradually fading audio in or out.
    /// </summary>
    public class AudioFader : MonoBehaviour
    {
        [Tooltip("Drag in the target AudioSource component.")]
        [SerializeField] private AudioSource audioSource;

        [Tooltip("The duration of the fade effect in seconds.")]
        [SerializeField] private float fadeDuration = 1f;

        [Tooltip("The volume that the AudioSource will fade to.")]
        [Range(0f, 1f), SerializeField] private float targetVolume = 1f;

        private Coroutine fadeCoroutine;

        public void SetFadeDuration(float fadeDuration)
        {
            this.fadeDuration = Mathf.Max(0, fadeDuration);
        }

        public void SetTargetVolume(float targetVolume)
        {
            this.targetVolume = Mathf.Clamp(targetVolume, 0f, 1f);
        }

        [ContextMenu("Fade Audio")]
        public void FadeAudio()
        {
            if (audioSource == null)
            {
                Debug.LogWarning(nameof(AudioFader) + ": The AudioSource has not been assigned!");
                return;
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeCoroutine());
        }

        private IEnumerator FadeCoroutine()
        {
            float initialVolume = audioSource.volume;

            float timeElapsed = 0;
            while (timeElapsed < fadeDuration)
            {
                float t = timeElapsed / fadeDuration;
                audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, t);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            fadeCoroutine = null;
        }

        private void OnValidate()
        {
            fadeDuration = Mathf.Max(0, fadeDuration);
            targetVolume = Mathf.Clamp(targetVolume, 0f, 1f);
        }
    }
}