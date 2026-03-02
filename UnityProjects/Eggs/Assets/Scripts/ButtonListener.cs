using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonListener : MonoBehaviour
{
    private Button[] buttons;
    private Action onClickAction;

    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        Debug.Log("ButtonListener инициализирован, найдено кнопок: " + buttons.Length);
    }

    public void SetOnClickAction(Action action)
    {
        onClickAction = action;
        Debug.Log("SetOnClickAction установлена");
    }

    void OnButtonClicked()
    {
        Debug.Log("Кнопка нажата!");
        onClickAction?.Invoke();
    }
}

