// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.DialogueSystem
{
    /// <summary>
    /// Feeds dialogue text into a DialogueManager.
    /// Can be triggered by a trigger collision, key press, or events.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        [Serializable]
        public class Events
        {
            [Space(20)]
            public UnityEvent onDialogueBegan, onDialogueEnded;
        }

        public enum TriggerType : byte
        {
            TriggerCollision,
            KeyPress,
            EventOnly
        }

        [Header("Setup")]
        [Tooltip("Reference to the DialogueManager in the scene.")]
        [SerializeField] private DialogueManager dialogueManager;

        [Tooltip("Text file containing the dialogue for this trigger.")]
        [SerializeField] private TextAsset dialogueTextFile;

        [Tooltip("How this trigger starts the dialogue.")]
        [SerializeField] private TriggerType triggerType = TriggerType.TriggerCollision;

        [Tooltip("Enter the tag name that should register collisions.")]
        [SerializeField] private string tagName = "Player";

        [Tooltip("If true, this trigger can only be used once.")]
        [SerializeField] private bool singleUse = false;

        [Tooltip("If true, dialogue will abruptly end when the player exits the trigger collider.")]
        [SerializeField] private bool endDialogueOnTriggerExit = false;

        [Header("Events")]
        [SerializeField] private Events dialogueEvents;

        private bool playerInside;
        private bool hasBeenUsed;

        // Call from other scripts or UnityEvents to manually start this dialogue
        public void StartDialogue()
        {
            TryStartDialogue();
        }

        public void DialogueBegan()
        {
            dialogueEvents.onDialogueBegan.Invoke();
        }

        public void DialogueEnded()
        {
            dialogueEvents.onDialogueEnded.Invoke();
        }

        public void SetSingleUse(bool singleUse)
        {
            this.singleUse = singleUse;
        }

        public void SetEndDialogueOnTriggerExit(bool endDialogueOnTriggerExit)
        {
            this.endDialogueOnTriggerExit = endDialogueOnTriggerExit;
        }

        private void Start()
        {
            if (dialogueManager == null)
            {
                dialogueManager = FindAnyObjectByType<DialogueManager>();
            }
        }

        private void Update()
        {
            if (triggerType != TriggerType.KeyPress)
            {
                return;
            }

            if (!playerInside)
            {
                return;
            }

            if (dialogueManager == null)
            {
                return;
            }

            if (dialogueManager.IsDialogueRunning)
            {
                return;
            }

            if (singleUse && hasBeenUsed)
            {
                return;
            }

            // Use the same advance key as the DialogueManager
            if (Input.GetKeyDown(dialogueManager.AdvanceKey))
            {
                TryStartDialogue();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleEnter(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            HandleExit(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleEnter(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            HandleExit(other.gameObject);
        }

        private void HandleEnter(GameObject other)
        {
            if (!other.CompareTag(tagName))
            {
                return;
            }

            playerInside = true;

            if (triggerType == TriggerType.TriggerCollision)
            {
                TryStartDialogue();
            }
        }

        private void HandleExit(GameObject other)
        {
            if (!other.CompareTag(tagName))
            {
                return;
            }

            playerInside = false;

            if (endDialogueOnTriggerExit && dialogueManager != null)
            {
                dialogueManager.EndDialogue();
            }
        }

        private void TryStartDialogue()
        {
            if (dialogueManager == null)
            {
                Debug.LogError($"[{nameof(DialogueTrigger)}] No {nameof(dialogueManager)} assigned on '{gameObject.name}'.", this);
                return;
            }

            if (dialogueTextFile == null)
            {
                Debug.LogError($"[{nameof(DialogueTrigger)}] No dialogue text asset assigned on '{gameObject.name}'.", this);
                return;
            }

            if (singleUse && hasBeenUsed)
            {
                return;
            }

            if (dialogueManager.IsDialogueRunning)
            {
                return;
            }

            hasBeenUsed = true;
            dialogueManager.StartDialogue(dialogueTextFile, this);
        }
    }
}