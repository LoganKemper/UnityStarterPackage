// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Add to a Light2D to flicker its intensity up and down.
    /// </summary>
    public class FlickeringLight2D : MonoBehaviour
    {
        [Tooltip("Drag in the target Light2D component.")]
        [SerializeField] private Light2D light2D;

        [Tooltip("Set a minimum intensity for the light.")]
        [SerializeField] private float minIntensity = 0.1f;

        [Tooltip("Set a maximum intensity for the light.")]
        [SerializeField] private float maxIntensity = 2f;

        [Tooltip("How frequently the light flickers.")]
        [SerializeField] private float frequency = 1f;

        private float baseIntensity;

        public void SetMinIntensity(float minIntensity)
        {
            this.minIntensity = minIntensity;
        }

        public void SetMaxIntensity(float maxIntensity)
        {
            this.maxIntensity = maxIntensity;
        }

        public void SetFrequency(float frequency)
        {
            this.frequency = frequency;
        }

        public void SetBaseIntensity(float baseIntensity)
        {
            this.baseIntensity = baseIntensity;
        }

        private void Start()
        {
            if (light2D == null)
            {
                return;
            }

            // Cache the original intensity
            baseIntensity = light2D.intensity;
        }

        private void Update()
        {
            if (light2D == null)
            {
                return;
            }

            // Perlin noise can be used to efficiently generate pseudo-random patterns of numbers
            float flicker = Mathf.PerlinNoise(Time.time * frequency, 0f);
            float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, flicker);

            light2D.intensity = targetIntensity * baseIntensity;
        }
    }
}