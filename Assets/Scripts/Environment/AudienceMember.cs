using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudienceMember : MonoBehaviour
{
    [SerializeField] private Transform focusPoint;
    [SerializeField] private bool recalculateLookDirection;
    [SerializeField] private int teamNum;

    private float jumpHeight = 0.5f;  // How high the audience jumps
    private float minJumpDuration = 0.3f;  // Min time per jump
    private float maxJumpDuration = 0.6f;  // Max time per jump
    private float totalDuration = 2f;  // Total celebration time


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (recalculateLookDirection)
        {
            transform.DOLookAt(focusPoint.position, 0.5f, AxisConstraint.Y)
                     .SetEase(Ease.OutQuad);
        }

    }

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
        if (scoringPlayerNum == teamNum)
        {
            StartCheerAnimation();
        }
    }

    private void StartCheerAnimation()
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;

        Sequence sequence = DOTween.Sequence();

        while (elapsedTime < totalDuration)
        {
            float jumpDuration = Random.Range(minJumpDuration, maxJumpDuration);

            // Always jump from the original Y position
            sequence.Append(transform.DOMoveY(originalPosition.y + jumpHeight, jumpDuration / 2).SetEase(Ease.OutQuad))
                    .Append(transform.DOMoveY(originalPosition.y, jumpDuration / 2).SetEase(Ease.InQuad));

            elapsedTime += jumpDuration;
        }

        sequence.Append(transform.DOMoveY(originalPosition.y, 0.2f).SetEase(Ease.OutQuad));
    }

}
