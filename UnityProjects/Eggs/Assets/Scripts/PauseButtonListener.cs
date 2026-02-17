using UnityEngine;
using UnityEngine.UI;

public class PauseButtonListener : MonoBehaviour
{
    private Button[] buttons;

    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(OnResumeClicked);
        }
        Debug.Log("PauseButtonListener инициализирован, найдено кнопок: " + buttons.Length);
    }

    void OnResumeClicked()
    {
        Debug.Log("Кнопка Resume нажата!");
        if (HeartsSystem.Instance != null)
        {
            HeartsSystem.Instance.Resume();
        }
    }
}
