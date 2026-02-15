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
    }

    public void SetOnClickAction(Action action)
    {
        onClickAction = action;
    }

    void OnButtonClicked()
    {
        onClickAction?.Invoke();
    }
}
