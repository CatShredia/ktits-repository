using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int score = 0;

    [SerializeField] private TextMeshProUGUI gameScoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameScoreText.text = "Score: " + score;
    }

    public void BatteryCollected()
    {
        score += 1;
        Debug.Log("Score: " + score);
    }
}
