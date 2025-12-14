// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalWorlds.DialogueSystem
{
    /// <summary>
    /// A ScriptableObject that stores all possible speakers for the dialogue system.
    /// </summary>
    [CreateAssetMenu(menuName = "Dialogue System/Speaker Library")]
    public class SpeakerLibrary : ScriptableObject
    {
        [Serializable]
        public class SpeakerSpriteEntry
        {
            [Tooltip("ID used in the [SPEAKERSPRITE=...] tag.")]
            public string spriteID;

            [Tooltip("Sprite shown when this spriteID is active.")]
            public Sprite sprite;
        }

        [Serializable]
        public class SpeakerInfo
        {
            [Tooltip("Speaker ID used in [NAME=...] tags. Should be unique (e.g. Player, NPC).")]
            public string ID = "Speaker";

            [Tooltip("Display name shown in the UI. If empty, the ID is used.")]
            public string displayName;

            [Tooltip("Default sprite used when [SPEAKERSPRITE=...] is omitted.")]
            public Sprite defaultSprite;

            [Tooltip("More options for the advanced dialogue system user.")]
            public AdvancedSpeakerOptions advancedOptions;
        }

        [Serializable]
        public class AdvancedSpeakerOptions
        {
            [Tooltip("Extra sprites that can be referenced by [SPEAKERSPRITE=...] tags.")]
            public List<SpeakerSpriteEntry> extraSprites = new();

            [Tooltip("If true, these custom colors will be used for the dialogue text.")]
            public bool useCustomColors = false;

            [Tooltip("Color of the speaker's name in the UI.")]
            [ColorUsage(false)]
            public Color nameColor = Color.black;

            [Tooltip("Color of the dialogue text in the UI.")]
            [ColorUsage(false)]
            public Color textColor = Color.white;

            [Tooltip("Optional: Random chatter sounds that can play while this speaker talks.")]
            public AudioClip[] chatterClips;

            [Tooltip("Minimum time (in seconds) between chatter sounds for this speaker.")]
            public float chatterInterval = 0.05f;
        }

        [Tooltip("List of all speakers that can appear in dialogue.")]
        [SerializeField] private List<SpeakerInfo> speakers = new();

        public SpeakerInfo GetSpeaker(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            for (int i = 0; i < speakers.Count; i++)
            {
                if (string.Equals(speakers[i].ID, id, StringComparison.OrdinalIgnoreCase))
                {
                    return speakers[i];
                }
            }

            Debug.LogWarning($"[{nameof(SpeakerLibrary)}] No {nameof(SpeakerInfo)} found with ID '{id}'. Check your {nameof(SpeakerLibrary)} or your [NAME=...] tag.");
            return null;
        }

        public Sprite GetSprite(string speakerID, string spriteID, out SpeakerInfo speakerInfo)
        {
            speakerInfo = GetSpeaker(speakerID);
            if (speakerInfo == null)
            {
                return null;
            }

            // No spriteID given, use default sprite
            if (string.IsNullOrWhiteSpace(spriteID))
            {
                return speakerInfo.defaultSprite;
            }

            // Try to find matching extra sprite
            for (int i = 0; i < speakerInfo.advancedOptions.extraSprites.Count; i++)
            {
                var entry = speakerInfo.advancedOptions.extraSprites[i];
                if (string.Equals(entry.spriteID, spriteID, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.sprite;
                }
            }

            Debug.LogWarning($"[{nameof(SpeakerLibrary)}] Speaker '{speakerInfo.ID}' does not have a sprite with ID '{spriteID}'. Using default sprite instead.");
            return speakerInfo.defaultSprite;
        }
    }
}