// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Add to a light to flicker its intensity up and down.
    /// </summary>
    public class FlickeringLight3D : MonoBehaviour
    {
        [Tooltip("Drag in the target Light component.")]
        [SerializeField] private Light _light;

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
            if (_light == null)
            {
                return;
            }

            // Cache the original intensity
            baseIntensity = _light.intensity;
        }

        private void Update()
        {
            if (_light == null)
            {
                return;
            }

            // Perlin noise can be used to efficiently generate pseudo-random patterns of numbers
            float flicker = Mathf.PerlinNoise(Time.time * frequency, 0f);
            float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, flicker);

            _light.intensity = targetIntensity * baseIntensity;
        }
    }
}