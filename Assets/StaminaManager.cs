using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    private float maxStamina = 100f;
    public float currentStamina { get; private set; }

    public delegate void StaminaBarHandler(float fillAmount);
    public static StaminaBarHandler OnStaminaUpdate;

    void Start()
    {
        currentStamina = maxStamina;
    }

    private void FixedUpdate()
    {
        RegenerateStamina();

        // Invoke event to update the stamina bar UI
        OnStaminaUpdate?.Invoke(currentStamina / maxStamina);
    }

    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += 0.1f;
        }
    }

    public void ApplyDashStaminaCost()
    {
        currentStamina -= Constants.DASH_STAMINA_COST;
    }

    private void OnBeginServe()
    {
        currentStamina = maxStamina;
    }
}
