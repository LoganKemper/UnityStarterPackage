// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

/// <summary>
/// Add to a light to flicker its intensity up and down.
/// </summary>
public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private Light m_light;
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
        if (m_light == null)
        {
            return;
        }

        // Perlin noise can be used to efficiently generate pseudo-random patterns of numbers
        float noise = Mathf.PerlinNoise(randomSeed, Time.time * frequency);

        // Flicker the intensity using the noise value
        m_light.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}
