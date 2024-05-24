using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private float maxStamina = 100f;
    public float currentStamina { get; private set; }

    public delegate void StaminaBarHandler(float fillAmount, int playerNum);
    public static StaminaBarHandler OnStaminaUpdate;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        currentStamina = maxStamina;
    }

    private void FixedUpdate()
    {
        RegenerateStamina();

        // Invoke event to update the stamina bar UI
        OnStaminaUpdate?.Invoke(currentStamina / maxStamina, playerManager.playerNum);
    }

    private void OnEnable()
    {
        OfflineGameStateManager.OnBeginServe += GameStateManager_OnBeginServe;
        GameMenu.OnGameRestart += GameMenu_OnGameRestart;
    }

    private void OnDisable()
    {
        OfflineGameStateManager.OnBeginServe -= GameStateManager_OnBeginServe;
        GameMenu.OnGameRestart -= GameMenu_OnGameRestart;
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

    private void GameStateManager_OnBeginServe(int playerNum, int gameEnvId)
    {
        ResetStamina();
    }

    private void GameMenu_OnGameRestart(int trainingEnvId)
    {
        ResetStamina();
    }


    private void ResetStamina()
    {
        currentStamina = maxStamina;
    }
}
