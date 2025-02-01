using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Relay UI
    [SerializeField] private Button createRelayButton;
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private Button joinRelayButton;
    [SerializeField] private TMP_InputField roomCodeInput;
    [SerializeField] private Button backButton;


    // Game UI
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI matchText;


    //private void OnEnable()
    //{
    //    CreateAndJoinRelay.OnRelayJoined += CreateAndJoinRelay_OnRelayJoined;
    //    GameStateManager.OnStartMatch += GameStateManager_OnStartMatch;
    //}

    //private void OnDisable()
    //{
    //    CreateAndJoinRelay.OnRelayJoined -= CreateAndJoinRelay_OnRelayJoined;
    //    GameStateManager.OnStartMatch -= GameStateManager_OnStartMatch;
    //}

    //private void CreateAndJoinRelay_OnRelayJoined(bool isHost, string joinCode)
    //{
    //    if (isHost)
    //    {
    //        roomCodeText.text = joinCode;
    //    }
    //    createRelayButton.gameObject.SetActive(false);
    //    joinRelayButton.gameObject.SetActive(false);
    //    roomCodeInput.gameObject.SetActive(false);
    //    backButton.gameObject.SetActive(false);
    //}

    //private void GameStateManager_OnStartMatch()
    //{
    //    roomCodeText.gameObject.SetActive(false);
    //    scoreText.gameObject.SetActive(true);
    //    matchText.gameObject.SetActive(true);
    //    winnerText.gameObject.SetActive(true);
    //}



}
