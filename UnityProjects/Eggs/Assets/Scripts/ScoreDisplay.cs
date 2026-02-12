using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("ScoreDisplay: scoreText is not assigned. Assign a TextMeshProUGUI element in the Inspector.");
            return;
        }

        // Обновляем текст при старте
        UpdateScore();
    }

    void Update()
    {
        // Обновляем счёт каждый кадр (или можно использовать события для оптимизации)
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (GameController.Instance != null && scoreText != null)
        {
            scoreText.text = "Score: " + GameController.Instance.score;
        }
    }
}
