using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq; // For list.Concat()
using DG.Tweening;

using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
    [SerializeField] private float minButtonXPos = -150f;
    [SerializeField] private float buttonXDiff = 100f;
    [SerializeField] private float buttonYDiff = 140f;
    [SerializeField] private float delayBetweenButtons = 0.1f;
    [SerializeField] private float buttonLerpDuration = 0.25f;


    [SerializeField] private List<Button> titleButtonList = new List<Button>();
    [SerializeField] private List<Button> gameModeButtonList = new List<Button>();
    [SerializeField] private List<Button> inGameButtonList = new List<Button>();
    [SerializeField] private List<Button> settingsButtonList = new List<Button>();


    public void LoadButtons(MenuType menuType)
    {
        ResetButtonPositions();
        // Load the corresponding button list
        List<Button> buttonsToLoad = menuType switch
        {
            MenuType.TitleScreen => titleButtonList,
            MenuType.GameModeSelection => gameModeButtonList,
            MenuType.InGame => inGameButtonList,
            MenuType.Settings => settingsButtonList,
            _ => null
        };

        if (buttonsToLoad != null)
        {
            AnimateButtons(buttonsToLoad);
        }
    }

    private void AnimateButtons(List<Button> buttons)
    {
        float startY = 0 + (buttons.Count - 3) * buttonYDiff;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform btnRect = buttons[i].GetComponent<RectTransform>();
            float targetX = minButtonXPos - (buttonXDiff * i);
            float targetY = startY - (buttonYDiff * i);

            btnRect.DOKill(); // Stop any existing tween

            // Immediately set the Y position to target Y
            btnRect.anchoredPosition = new Vector2(btnRect.anchoredPosition.x, targetY);
            btnRect.DOAnchorPosX(targetX, buttonLerpDuration) // Lerp only the X position
                .SetEase(Ease.InOutQuad)
                .SetDelay(i * delayBetweenButtons); // Delay each button animation
        }
    }

    private void ResetButtonPositions()
    {
        foreach (var button in titleButtonList.Concat(gameModeButtonList)
                                              .Concat(inGameButtonList)
                                              .Concat(settingsButtonList))
        {
            if (button != null)
            {
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.DOKill(); // Stop any ongoing animation
                    rectTransform.anchoredPosition = new Vector2(500, 0); // Default hidden button location
                }
            }
        }
    }
}
