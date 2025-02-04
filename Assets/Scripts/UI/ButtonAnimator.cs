using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq; // For list.Concat()
using UnityEngine.EventSystems;
using DG.Tweening;

using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
    [SerializeField] private float minButtonXPos = -150f;
    [SerializeField] private float buttonXDiff = 100f; // X staircase offset for buttons
    [SerializeField] private float buttonYDiff = 140f; // Y spacing between buttons
    [SerializeField] private float delayBetweenButtons = 0.1f;
    [SerializeField] private float buttonLerpDuration = 0.25f; 
    [SerializeField] private float moveLeftOffset = 10f; // Hovered button moves left
    [SerializeField] private float moveRightOffset = 5f; // Other buttons move right
    [SerializeField] private float expandFactor = 1.1f; // Scale factor for hovered button

    [SerializeField] private List<Button> titleButtonList = new List<Button>();
    [SerializeField] private List<Button> gameModeButtonList = new List<Button>();
    [SerializeField] private List<Button> inGameButtonList = new List<Button>();
    [SerializeField] private List<Button> settingsButtonList = new List<Button>();

    private List<Button> allButtons = new List<Button>();

    void Start()
    {
        allButtons = titleButtonList.Concat(gameModeButtonList)
                                    .Concat(inGameButtonList)
                                    .Concat(settingsButtonList)
                                    .ToList();
        //foreach (Button button in allButtons)
        //{
        //    EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        //    // OnPointerEnter (hover start)
        //    EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        //    entryEnter.callback.AddListener((data) => { OnButtonHover(button); });
        //    trigger.triggers.Add(entryEnter);

        //    // OnPointerExit (hover end)
        //    EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        //    entryExit.callback.AddListener((data) => { OnButtonExit(button); });
        //    trigger.triggers.Add(entryExit);
        //}
    }

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

    private void OnButtonHover(Button hoveredButton)
    {
        RectTransform hoveredRect = hoveredButton.GetComponent<RectTransform>();

        // Move hovered button left & expand
        hoveredRect.DOAnchorPos(hoveredRect.anchoredPosition + new Vector2(-moveLeftOffset, 0), buttonLerpDuration).SetEase(Ease.OutQuad);
        hoveredRect.DOScale(Vector3.one * expandFactor, buttonLerpDuration).SetEase(Ease.OutQuad);

        // Move other buttons right
        foreach (var button in allButtons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect != hoveredRect)
            {
                rect.DOAnchorPos(rect.anchoredPosition + new Vector2(moveRightOffset, 0), buttonLerpDuration).SetEase(Ease.OutQuad);
            }
        }
    }

    private void OnButtonExit(Button hoveredButton)
    {
        RectTransform hoveredRect = hoveredButton.GetComponent<RectTransform>();

        // Reset hovered button position & size dynamically
        hoveredRect.DOAnchorPos(hoveredRect.anchoredPosition + new Vector2(moveLeftOffset, 0), buttonLerpDuration).SetEase(Ease.OutQuad);
        hoveredRect.DOScale(Vector3.one, buttonLerpDuration).SetEase(Ease.OutQuad);

        // Reset other buttons
        foreach (var button in allButtons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect != hoveredRect)
            {
                rect.DOAnchorPos(rect.anchoredPosition + new Vector2(-moveRightOffset, 0), buttonLerpDuration).SetEase(Ease.OutQuad);
            }
        }
    }

    private void ResetButtonPositions()
    {
        foreach (var button in allButtons)
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
