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
    [SerializeField] private float moveLeftOffset = 10f; // hovered button moves left
    [SerializeField] private float expandFactor = 1.1f; // hovered button increases in size

    [SerializeField] private List<Button> titleButtonList = new List<Button>();
    [SerializeField] private List<Button> gameModeButtonList = new List<Button>();
    [SerializeField] private List<Button> inGameButtonList = new List<Button>();
    [SerializeField] private List<Button> settingsButtonList = new List<Button>();

    private List<Button> allButtons = new List<Button>();
    private Dictionary<Button, Vector2> originalPositions = new Dictionary<Button, Vector2>(); // Prevent displacement when quickly hovering/unhovering
    private bool areButtonsInAnimation = false;

    void Start()
    {
        allButtons = titleButtonList.Concat(gameModeButtonList)
                                    .Concat(inGameButtonList)
                                    .Concat(settingsButtonList)
                                    .ToList();

        foreach (Button button in allButtons)
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            // OnPointerEnter (hover start)
            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => { OnButtonHover(button); });
            trigger.triggers.Add(entryEnter);

            // OnPointerExit (hover end)
            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((data) => { OnButtonExit(button); });
            trigger.triggers.Add(entryExit);
        }
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
            areButtonsInAnimation = true;
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

            // Immediately set the Y position to target Y
            int currButtonNum = i;
            btnRect.anchoredPosition = new Vector2(btnRect.anchoredPosition.x, targetY);
            btnRect.DOAnchorPosX(targetX, buttonLerpDuration) // Lerp only the X position
                .SetEase(Ease.InOutQuad)
                .SetDelay(i * delayBetweenButtons) // Delay each button animation
                .SetUpdate(true) // Runs even when time is stopped
                .OnComplete(() =>
                {
                    if (currButtonNum == buttons.Count - 1)
                        areButtonsInAnimation = false; // Allow interactions once last button is done
                });

            originalPositions[buttons[i]] = new Vector2(targetX, targetY);
        }
    }

    private void OnButtonHover(Button hoveredButton)
    {
        RectTransform hoveredRect = hoveredButton.GetComponent<RectTransform>();

        Vector2 originalPos = originalPositions[hoveredButton]; // Retrieve original position
        hoveredRect.DOScale(Vector3.one * expandFactor, buttonLerpDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
        hoveredRect.DOAnchorPos(new Vector2(originalPos.x - moveLeftOffset, originalPos.y), buttonLerpDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

    }

    private void OnButtonExit(Button hoveredButton)
    {
        
        RectTransform hoveredRect = hoveredButton.GetComponent<RectTransform>();

        Vector2 originalPos = originalPositions[hoveredButton]; // Retrieve original position
        hoveredRect.DOScale(Vector3.one, buttonLerpDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        if (!areButtonsInAnimation)
        {
            // Only move button if menu is NOT in open/close animation
            hoveredRect.DOAnchorPos(originalPos, buttonLerpDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        }

    }

    private void ResetButtonPositions()
    {
        foreach (var button in allButtons)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.DOKill();
            rectTransform.anchoredPosition = new Vector2(500, 0); // Default hidden button location
        }

    }
}
