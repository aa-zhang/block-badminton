using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Referee : MonoBehaviour
{
    private void OnEnable()
    {
        OfflineBirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
    }

    private void OnDisable()
    {
        OfflineBirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
    }



    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        if (scoringPlayerNum == 1)
        {
            
        }

        Constants.FLAG_DOWN_POSITION
        Constants.FLAG_DOWN_ROTATION
        Constants.FLAG_UP_POSITION
        Constants.FLAG_UP_POSITION
    }
}
