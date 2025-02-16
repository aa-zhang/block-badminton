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

    private GameEnvironmentManager gameEnv;


    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        currentStamina = maxStamina;
        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();

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
        PlayerLoader.OnPlayersLoaded += PlayerLoader_OnPlayersLoaded;
    }

    private void OnDisable()
    {
        OfflineGameStateManager.OnBeginServe -= GameStateManager_OnBeginServe;
        PlayerLoader.OnPlayersLoaded -= PlayerLoader_OnPlayersLoaded;
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
        if (gameEnv.isTraining && gameEnv.id != gameEnvId)
        {
            return;
        }
        ResetStamina();
    }

    private void PlayerLoader_OnPlayersLoaded(PlayMode playMode)
    {
        ResetStamina();
    }


    private void ResetStamina()
    {
        currentStamina = maxStamina;
    }
}
