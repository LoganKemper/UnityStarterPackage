// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DigitalWorlds.DialogueSystem
{
    /// <summary>
    /// Handles the dialogue that is sent by the DialogueTrigger. 
    /// The sent text is navigated by DialogueManager and then displayed on the UI.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        [Serializable]
        public class Events
        {
            [Space(20)]
            public UnityEvent onDialogueBegan, onDialogueEnded;
        }

        [Serializable]
        private class DialogueLine
        {
            public string speakerID;
            public string spriteID;
            public string text;
        }

        [Header("Data")]
        [Tooltip("Speaker library asset with all speakers and their data.")]
        [SerializeField] private SpeakerLibrary speakerLibrary;

        [Header("UI")]
        [Tooltip("Parent GameObject for the dialogue UI. Will be enabled/disabled.")]
        [SerializeField] private GameObject dialogueParent;

        [Tooltip("Main text element where dialogue text is shown.")]
        [SerializeField] private TextMeshProUGUI bodyText;

        [Tooltip("Text element where the speaker's name is shown.")]
        [SerializeField] private TextMeshProUGUI nameText;

        [Tooltip("Image where the speaker's portrait is shown.")]
        [SerializeField] private Image speakerImage;

        [Tooltip("Optional: Image shown when it is possible to advance to the next line.")]
        [SerializeField] private GameObject continueImage;

        [Header("Input Settings")]
        [Tooltip("The button input used for advancing/starting dialogue. Set to the E key by default.")]
        [SerializeField] private KeyCode advanceInput = KeyCode.E;

        [Tooltip("The button input used for skipping dialogue entirely. Set to the X key by default.")]
        [SerializeField] private KeyCode skipInput = KeyCode.X;

        [Header("Typing Settings")]
        [Tooltip("If true, the text will type out character by character. If false, the text will appear instantly.")]
        [SerializeField] private bool typeOutText = true;

        [Tooltip("How many characters per second are revealed when typing.")]
        [SerializeField] private float charactersPerSecond = 40f;

        [Tooltip("Minimum time (in seconds) before a line can be skipped or the dialogue can be skipped.")]
        [SerializeField] private float minTimeBeforeSkip = 0.25f;

        [Tooltip("Optional: Audio source used for text chatter sounds.")]
        [SerializeField] private AudioSource chatterAudioSource;

        [Header("Events")]
        [SerializeField] private Events dialogueEvents;

        public KeyCode AdvanceKey => advanceInput;
        public bool IsDialogueRunning => isDialogueRunning;

        private readonly List<DialogueLine> currentLines = new();
        private DialogueTrigger currentTrigger;
        private Coroutine dialogueCoroutine;
        private Color originalNameColor = Color.black;
        private Color originalTextColor = Color.white;
        private float nextChatterTime;
        private int currentLineIndex;
        private bool advanceRequested;
        private bool isDialogueRunning;
        private bool skipAllRequested;

        private void Awake()
        {
            // Make sure UI starts disabled
            if (dialogueParent != null)
            {
                dialogueParent.SetActive(false);
            }

            if (continueImage != null)
            {
                continueImage.SetActive(false);
            }

            if (nameText != null)
            {
                originalNameColor = nameText.color;
            }

            if (bodyText != null)
            {
                originalTextColor = bodyText.color;
            }
        }

        public void SetCharactersPerSecond(float charactersPerSecond)
        {
            this.charactersPerSecond = charactersPerSecond;
        }

        public void SetTypeOutText(bool typeOutText)
        {
            this.typeOutText = typeOutText;
        }

        public void StartDialogue(TextAsset dialogueTextAsset, DialogueTrigger sourceTrigger)
        {
            if (dialogueTextAsset == null)
            {
                Debug.LogError("[{nameof(DialogueManager)}] Tried to start dialogue with a null text asset.", this);
                return;
            }

            if (isDialogueRunning && dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
            }

            currentTrigger = sourceTrigger;
            currentLines.Clear();
            ParseDialogue(dialogueTextAsset);

            if (currentLines.Count == 0)
            {
                Debug.LogWarning($"[{nameof(DialogueManager)}] Dialogue file '{dialogueTextAsset.name}' did not produce any valid lines.");
                return;
            }

            isDialogueRunning = true;
            skipAllRequested = false;
            currentLineIndex = 0;

            if (dialogueParent != null)
            {
                dialogueParent.SetActive(true);
            }

            if (bodyText != null)
            {
                bodyText.text = string.Empty;
                bodyText.maxVisibleCharacters = 0;
            }

            if (continueImage != null)
                continueImage.SetActive(false);

            // Fire events
            dialogueEvents.onDialogueBegan.Invoke();
            if (currentTrigger != null)
            {
                currentTrigger.DialogueBegan();
            }

            dialogueCoroutine = StartCoroutine(RunDialogueCoroutine());
        }

        public void AdvanceDialogue()
        {
            if (!isDialogueRunning)
            {
                return;
            }

            advanceRequested = true;
        }

        public void EndDialogue()
        {
            isDialogueRunning = false;
            skipAllRequested = false;

            if (dialogueParent != null)
            {
                dialogueParent.SetActive(false);
            }

            if (continueImage != null)
            {
                continueImage.SetActive(false);
            }

            dialogueEvents.onDialogueEnded.Invoke();
            if (currentTrigger != null)
            {
                currentTrigger.DialogueEnded();
                currentTrigger = null;
            }
        }

        private IEnumerator RunDialogueCoroutine()
        {
            while (currentLineIndex < currentLines.Count && !skipAllRequested)
            {
                DialogueLine line = currentLines[currentLineIndex];

                // Update speaker UI
                ApplySpeakerVisuals(line);

                float lineStartTime = Time.unscaledTime;

                // Show the line
                yield return StartCoroutine(ShowLine(line, lineStartTime));
                if (skipAllRequested)
                {
                    break;
                }

                // Wait until player advances to next line
                yield return StartCoroutine(WaitForAdvance(lineStartTime));
                if (skipAllRequested)
                {
                    break;
                }

                currentLineIndex++;
            }

            EndDialogue();
        }

        private void ApplySpeakerVisuals(DialogueLine line)
        {
            if (speakerLibrary == null)
            {
                Debug.LogWarning($"{nameof(DialogueManager)} No {nameof(SpeakerLibrary)} assigned.", this);
                SetFallbackSpeakerVisuals(line.speakerID);
                return;
            }

            Sprite sprite = speakerLibrary.GetSprite(line.speakerID, line.spriteID, out SpeakerLibrary.SpeakerInfo speakerInfo);

            // Name and colors
            string displayName = line.speakerID;
            Color nameColor = originalNameColor;
            Color textColor = originalTextColor;
            bool useCustomColors = false;

            if (speakerInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(speakerInfo.displayName))
                {
                    displayName = speakerInfo.displayName;
                }

                useCustomColors = speakerInfo.advancedOptions.useCustomColors;
                if (useCustomColors)
                {
                    nameColor = speakerInfo.advancedOptions.nameColor;
                    textColor = speakerInfo.advancedOptions.textColor;
                }
            }

            if (nameText != null)
            {
                nameText.text = displayName;
                nameText.color = nameColor;
            }

            if (bodyText != null)
            {
                bodyText.color = textColor;
            }

            if (speakerImage != null)
            {
                speakerImage.sprite = sprite;
                speakerImage.enabled = (sprite != null);
            }
        }

        private void SetFallbackSpeakerVisuals(string speakerId)
        {
            if (nameText != null)
            {
                nameText.text = string.IsNullOrWhiteSpace(speakerId) ? "???" : speakerId;
            }

            if (speakerImage != null)
            {
                speakerImage.sprite = null;
                speakerImage.enabled = false;
            }
        }

        private IEnumerator ShowLine(DialogueLine line, float lineStartTime)
        {
            if (bodyText == null)
            {
                yield break;
            }

            // Assign the full text so TMP can calculate wrapping based on the whole line
            bodyText.text = line.text;
            bodyText.ForceMeshUpdate();
            int totalChars = bodyText.textInfo.characterCount;

            // No typing out: show everything instantly
            if (!typeOutText || charactersPerSecond <= 0f)
            {
                bodyText.maxVisibleCharacters = totalChars;
                yield break;
            }

            bodyText.maxVisibleCharacters = 0;

            // Get speaker info for chatter sounds (if any)
            SpeakerLibrary.SpeakerInfo speakerInfo = null;
            if (speakerLibrary != null)
            {
                speakerInfo = speakerLibrary.GetSpeaker(line.speakerID);
            }

            int visibleCount = 0;
            float charDelay = 1f / charactersPerSecond;

            while (visibleCount < totalChars && !skipAllRequested)
            {
                bool canSkip = Time.unscaledTime - lineStartTime >= minTimeBeforeSkip;

                // Skip entire dialogue
                if (canSkip && Input.GetKeyDown(skipInput))
                {
                    skipAllRequested = true;
                    yield break;
                }

                // Skip typing for this line
                bool advancePressed = canSkip && (Input.GetKeyDown(advanceInput) || advanceRequested);
                if (advancePressed)
                {
                    advanceRequested = false;
                    bodyText.maxVisibleCharacters = totalChars;
                    yield break;
                }

                visibleCount++;
                bodyText.maxVisibleCharacters = visibleCount;
                TryPlayChatter(speakerInfo, visibleCount - 1);

                yield return new WaitForSecondsRealtime(charDelay);
            }
        }

        private IEnumerator WaitForAdvance(float lineStartTime)
        {
            if (continueImage != null)
            {
                continueImage.SetActive(true);
            }

            while (!skipAllRequested)
            {
                bool canSkip = Time.unscaledTime - lineStartTime >= minTimeBeforeSkip;

                if (canSkip && Input.GetKeyDown(skipInput))
                {
                    skipAllRequested = true;
                    break;
                }

                bool advancePressed = canSkip && (Input.GetKeyDown(advanceInput) || advanceRequested);
                if (advancePressed)
                {
                    advanceRequested = false;
                    break;
                }

                yield return null;
            }

            if (continueImage != null)
            {
                continueImage.SetActive(false);
            }
        }

        private void TryPlayChatter(SpeakerLibrary.SpeakerInfo speakerInfo, int charIndex)
        {
            if (chatterAudioSource == null || speakerInfo == null)
            {
                return;
            }

            if (speakerInfo.advancedOptions.chatterClips == null || speakerInfo.advancedOptions.chatterClips.Length == 0)
            {
                return;
            }

            if (bodyText == null || bodyText.textInfo.characterCount == 0)
            {
                return;
            }

            if (charIndex < 0 || charIndex >= bodyText.textInfo.characterCount)
            {
                return;
            }

            var charInfo = bodyText.textInfo.characterInfo[charIndex];
            char c = charInfo.character;
            if (char.IsWhiteSpace(c) || c == '\n' || c == '\r')
            {
                return;
            }

            float interval = speakerInfo.advancedOptions.chatterInterval > 0f ? speakerInfo.advancedOptions.chatterInterval : 0.05f;

            // Throttle chatter slightly so it's not every single character
            if (Time.unscaledTime < nextChatterTime)
            {
                return;
            }

            AudioClip clip = speakerInfo.advancedOptions.chatterClips[UnityEngine.Random.Range(0, speakerInfo.advancedOptions.chatterClips.Length)];
            if (clip != null)
            {
                chatterAudioSource.PlayOneShot(clip);
                nextChatterTime = Time.unscaledTime + interval;
            }
        }

        private void ParseDialogue(TextAsset dialogueTextAsset)
        {
            string[] rawLines = dialogueTextAsset.text.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
                );

            currentLines.Clear();

            for (int i = 0; i < rawLines.Length; i++)
            {
                string rawLine = rawLines[i];
                string line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                Dictionary<string, string> tags = new(StringComparer.OrdinalIgnoreCase);
                int index = 0;

                // Collect all leading [TAG=VALUE] blocks
                while (index < line.Length && line[index] == '[')
                {
                    int closingIndex = line.IndexOf(']', index + 1);
                    if (closingIndex == -1)
                    {
                        Debug.LogError($"[{nameof(DialogueManager)}] Missing closing ']' for tag on line {i + 1} in '{dialogueTextAsset.name}'. Line was: {rawLine}");
                        break;
                    }

                    string inside = line.Substring(index + 1, closingIndex - index - 1);

                    // Allow spaces: [  TAG  =  VALUE  ]
                    string[] parts = inside.Split(new[] { '=' }, 2);
                    if (parts.Length != 2)
                    {
                        Debug.LogError($"[{nameof(DialogueManager)}] Could not parse tag '{inside}' on line {i + 1} in '{dialogueTextAsset.name}'. Tags must look like [NAME=Player] or [SPEAKERSPRITE=Player].");
                    }
                    else
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        if (string.IsNullOrEmpty(key))
                        {
                            Debug.LogError($"[{nameof(DialogueManager)}] Empty tag key on line {i + 1} in '{dialogueTextAsset.name}'.");
                        }
                        else
                        {
                            tags[key] = value;
                        }
                    }

                    index = closingIndex + 1;

                    // Skip whitespace between tags
                    while (index < line.Length && char.IsWhiteSpace(line[index]))
                    {
                        index++;
                    }
                }

                string textPart = line.Substring(index).TrimStart();
                if (string.IsNullOrEmpty(textPart))
                {
                    Debug.LogWarning($"[{nameof(DialogueManager)}] Line {i + 1} in '{dialogueTextAsset.name}' has tags but no text. Skipping.");
                    continue;
                }

                // Determine speaker and sprite
                string speakerID = null;
                string spriteID = null;

                if (tags.TryGetValue("NAME", out string nameValue))
                {
                    speakerID = nameValue;
                }

                if (tags.TryGetValue("SPEAKERSPRITE", out string spriteValue))
                {
                    spriteID = spriteValue;
                }

                if (speakerID == null)
                {
                    Debug.LogWarning($"[{nameof(DialogueManager)}] Line {i + 1} in '{dialogueTextAsset.name}' has no [NAME=...] tag and no previous speaker to fall back to. Using 'UnknownSpeaker'.");
                    speakerID = "UnknownSpeaker";
                }

                currentLines.Add(new DialogueLine
                {
                    speakerID = speakerID,
                    spriteID = spriteID,
                    text = textPart
                });
            }
        }
    }
}