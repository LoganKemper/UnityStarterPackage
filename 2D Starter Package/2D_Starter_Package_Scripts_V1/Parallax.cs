// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Add to a GameObject in the background or foreground to create the illusion of depth with parallaxing movement.
    /// </summary>
    public class Parallax : MonoBehaviour
    {
        [Tooltip("Drag in the main camera. If left empty, the script will try to find the main camera in the scene.")]
        [SerializeField] private Transform mainCamera;

        [Tooltip("Drag in the SpriteRenderer.")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Tooltip("The higher the strength, the more this GameObject will move with the camera. Use values between 0 and 1 for objects in the background, and values above 1 for objects in the foreground.")]
        [SerializeField] private float parallaxStrength = 0.5f;

        [Tooltip("Add a continuous scrolling effect to the x-axis or y-axis. This could be used for clouds slowly passing by, or an endless runner game with the background constantly moving.")]
        [SerializeField] private Vector2 continuousScrolling = Vector2.zero;

        [Tooltip("Select how the parallax should loop, if at all.")]
        [SerializeField] private LoopMode loopMode = LoopMode.None;

        public enum LoopMode
        {
            None,       // No looping, just parallaxing
            Horizontal, // Loop horizontally
            Vertical,   // Loop vertically
            Both        // Loop horizontally and vertically
        }

        private Vector3 startPos;
        private float spriteHeight, spriteWidth;

        private void Start()
        {
            // Cache the starting position of this GameObject
            startPos = transform.position;

            // If the camera transform has not been manually assigned, get the main active camera in the scene
            if (mainCamera == null)
            {
                mainCamera = Camera.main.transform;
            }

            // Get the sprite's height and width
            spriteHeight = spriteRenderer.bounds.size.x;
            spriteWidth = spriteRenderer.bounds.size.y;
        }

        private void LateUpdate()
        {
            // Apply continuous scrolling
            startPos.x += continuousScrolling.x * Time.deltaTime;
            startPos.y += continuousScrolling.y * Time.deltaTime;

            float distanceX = mainCamera.position.x * (1 - parallaxStrength);
            float distanceY = mainCamera.position.y * (1 - parallaxStrength);

            // Apply parallax effect
            Vector3 targetPosition = new(startPos.x + distanceX, startPos.y + distanceY, transform.position.z);

            transform.position = targetPosition;

            // If no looping is selected, return early
            if (loopMode == LoopMode.None)
            {
                return;
            }

            float cameraX = mainCamera.position.x * parallaxStrength;
            float cameraY = mainCamera.position.y * parallaxStrength;

            // Move when the camera reaches half of the sprite size
            float offsetX = spriteWidth * 0.5f;
            float offsetY = spriteHeight * 0.5f;

            // Handle horizontal looping
            if (loopMode == LoopMode.Horizontal || loopMode == LoopMode.Both)
            {
                if (cameraX > startPos.x + offsetX)
                {
                    startPos.x += spriteWidth;
                }
                else if (cameraX < startPos.x - offsetX)
                {
                    startPos.x -= spriteWidth;
                }
            }

            // Handle vertical looping
            if (loopMode == LoopMode.Vertical || loopMode == LoopMode.Both)
            {
                if (cameraY > startPos.y + offsetY)
                {
                    startPos.y += spriteHeight;
                }
                else if (cameraY < startPos.y - offsetY)
                {
                    startPos.y -= spriteHeight;
                }
            }
        }

        private void OnValidate()
        {
            // Prevent a negative parallaxStrength from being set in the inspector
            if (parallaxStrength < 0)
            {
                parallaxStrength = 0;
            }
        }
    }
}