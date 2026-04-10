using UnityEngine;
using TMPro;

public class UserUIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        UpdateScoreText(GameManager.Instance.Score);
        GameManager.Instance.OnScoreChanged += UpdateScoreText;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnScoreChanged -= UpdateScoreText;
    }

    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
}
