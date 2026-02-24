using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI genaText;

    void Start()
    {
        if (scoreText == null || genaText == null)
        {
            Debug.LogWarning("ScoreDisplay: scoreText is not assigned. Assign a TextMeshProUGUI element in the Inspector.");
            return;
        }

        UpdateScore();
    }

    void Update()
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (GameController.Instance != null && scoreText != null)
        {
            scoreText.text = "Score: " + GameController.Instance.score;
        }

        if (GameController.Instance != null && genaText != null)
        {
            genaText.text = "Score: " + GameController.Instance.score;
        }
    }
}
