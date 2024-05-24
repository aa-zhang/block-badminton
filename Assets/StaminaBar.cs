using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private Gradient colorGradient;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
        StaminaManager.OnStaminaUpdate += UpdateStaminaBar;
    }

    void OnDisable()
    {
        StaminaManager.OnStaminaUpdate -= UpdateStaminaBar;
    }

    private void UpdateStaminaBar(float fillAmount)
    {
        staminaBarFill.DOFillAmount(fillAmount, fillSpeed);
        staminaBarFill.DOColor(colorGradient.Evaluate(fillAmount), fillSpeed);
    }
}
