using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaBar : MonoBehaviour
{
    private float maxStamina = 100f;
    public float currentStamina { get; private set; }
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private Gradient colorGradient;

    // Start is called before the first frame update
    void Start()
    {
        currentStamina = maxStamina;
    }

    private void FixedUpdate()
    {
        RegenerateStamina();
        UpdateStaminaBar();
    }

    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += 0.1f;
        }
    }

    private void UpdateStaminaBar()
    {
        float fillAmount = currentStamina / maxStamina;
        staminaBarFill.DOFillAmount(fillAmount, fillSpeed);
        staminaBarFill.DOColor(colorGradient.Evaluate(fillAmount), fillSpeed);
    }

    private void PlayerDashed()
    {
        currentStamina -= Constants.DASH_STAMINA;
    }
}
