using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Used for keeping track of a score value and displaying it on the UI. Can be used for the number of items collected, number of enemies defeated, etc.
/// </summary>
public class Score : MonoBehaviour
{
    [Tooltip("Optional: Drag in a TextMeshPro text element to display the score.")]
    [SerializeField] private TMP_Text scoreText;

    [Tooltip("Optional: Prefix before the score text. Could be something like \"Score: \" or \"Death Count: \"")]
    [SerializeField] private string scorePrefix = "Score: ";

    [Tooltip("The number of digits in the score number. 2 = 00, 4 = 0000, etc. Set to 0 to ignore.")]
    [SerializeField] private int scoreDigits = 0;

    [Tooltip("The minimum allowed score value.")]
    [SerializeField] private int minimumScore = 0;

    [Tooltip("The maximum allowed score value.")]
    [SerializeField] private int maximumScore = 1000;

    [Tooltip("If the score is persistent, it will keep its value even when the scene changes. Important: The GameObject's name is used to keep track of which score value belongs to which Score component.")]
    [SerializeField] private bool persistentScore = false;

    [Space(20)]
    [SerializeField] private UnityEvent<int> onScoreChanged;

    private string scoreID;
    private int score = 0;
    private static Dictionary<string, int> instanceScores = new();

    private void Awake()
    {
        // Handle the persistent score functionality
        if (persistentScore)
        {
            scoreID = gameObject.name;

            if (!instanceScores.ContainsKey(scoreID))
            {
                // Initialize static score for this score's ID
                instanceScores[scoreID] = 0;
            }
        }
    }

    private void Start()
    {
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        // Return out of this method early if no score text has been assigned
        if (scoreText == null)
        {
            return;
        }

        // Format the score and display it on the score text
        if (scoreDigits > 0)
        {
            scoreText.text = scorePrefix + GetScore().ToString("D" + scoreDigits);
        }
        else
        {
            scoreText.text = scorePrefix + GetScore().ToString();
        }
    }

    public int GetScore()
    {
        if (persistentScore)
        {
            return instanceScores.ContainsKey(scoreID) ? instanceScores[scoreID] : 0;
        }
        else
        {
            return score;
        }
    }

    // Call this method to directly change the score to a new value
    public void SetScore(int newScore)
    {
        newScore = Mathf.Clamp(newScore, minimumScore, maximumScore);

        if (persistentScore)
        {
            instanceScores[scoreID] = newScore;
        }
        else
        {
            score = newScore;
        }

        UpdateScoreText();
        onScoreChanged.Invoke(newScore);
    }

    // Call this method to increase or decrease the score
    public void AdjustScore(int adjustment)
    {
        SetScore(GetScore() + adjustment);
    }

    [RuntimeInitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        // instanceScores will be reset when the game first starts
        instanceScores = new();
    }
}
