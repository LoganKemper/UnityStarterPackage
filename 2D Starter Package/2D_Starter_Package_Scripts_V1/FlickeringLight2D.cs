// Unity Starter Package - Version 1
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
        [SerializeField] private Light2D light2D;
        [SerializeField] private float minIntensity = 0.1f;
        [SerializeField] private float maxIntensity = 1f;
        [SerializeField] private float frequency = 1f;

        private float randomSeed;

        private void Start()
        {
            // Generate a random number for use in the Perlin noise generator
            randomSeed = Random.Range(0f, 65535f);
        }

        private void Update()
        {
            // Return early if the light has not been assigned
            if (light2D == null)
            {
                return;
            }

            // Perlin noise can be used to efficiently generate pseudo-random patterns of numbers
            float noise = Mathf.PerlinNoise(randomSeed, Time.time * frequency);

            // Flicker the intensity using the noise value
            light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }
}