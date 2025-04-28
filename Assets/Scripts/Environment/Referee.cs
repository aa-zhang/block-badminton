using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Referee : MonoBehaviour
{
    [SerializeField] private Transform playerOneFlag;
    [SerializeField] private Transform playerTwoFlag;

    private bool playerOneFlagActive;
    private bool playerTwoFlagActive;

    private void OnEnable()
    {
        OfflineBirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
        OfflineServeController.OnHitServe += OfflineServeController_OnHitServe;
    }

    private void OnDisable()
    {
        OfflineBirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        OfflineServeController.OnHitServe -= OfflineServeController_OnHitServe;
    }

    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        RaiseFlag(scoringPlayerNum);
        playerOneFlagActive = scoringPlayerNum == 1;
        playerTwoFlagActive = scoringPlayerNum == 2;
    }

    private void OfflineServeController_OnHitServe()
    {
        if (playerOneFlagActive)
        {
            LowerFlag(1);
            playerOneFlagActive = false;
        }
        else if (playerTwoFlagActive)
        {
            LowerFlag(2);
            playerTwoFlagActive = false;
        }
    }

    private void RaiseFlag(int playerNum)
    {
        Transform targetFlag = playerNum == 1 ? playerOneFlag : playerTwoFlag;
        Vector3 targetPos = Constants.FLAG_UP_POSITION;
        Vector3 targetRot = Constants.FLAG_UP_ROTATION;

        if (playerNum == 1)
        {
            targetPos.x = -targetPos.x; // Invert X position for player 1 flag
        }
        else
        {
            targetRot.y = -targetRot.y; // Invert Y position for player 2 flag
        }
        Sequence seq = DOTween.Sequence();
        seq.Join(targetFlag.DOLocalMove(targetPos, 0.5f));
        seq.Join(targetFlag.DORotate(targetRot, 0.5f, RotateMode.FastBeyond360));
    }

    private void LowerFlag(int playerNum)
    {
        Transform targetFlag = playerNum == 1 ? playerOneFlag : playerTwoFlag;
        Vector3 targetPos = Constants.FLAG_DOWN_POSITION;
        Vector3 targetRot = Constants.FLAG_DOWN_ROTATION;

        if (playerNum == 1)
        {
            targetPos.x = -targetPos.x; // Invert X position for player 1 flag
        }

        Sequence seq = DOTween.Sequence();
        seq.Join(targetFlag.DOLocalMove(targetPos, 0.5f));
        seq.Join(targetFlag.DORotate(targetRot, 0.5f));
    }
}
