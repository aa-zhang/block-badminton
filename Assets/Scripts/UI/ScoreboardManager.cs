using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreboardManager : MonoBehaviour
{
    // Game UI
    private CanvasGroup canvasGroup;

    [SerializeField] private List<Image> matchWinnerSquares = new List<Image>();
    [SerializeField] private Sprite emptySquare;
    [SerializeField] private Sprite filledSquare;


    [SerializeField] private TextMeshProUGUI matchInfo;
    [SerializeField] private TextMeshProUGUI playerOneScore;
    [SerializeField] private TextMeshProUGUI playerTwoScore;
    [SerializeField] private TextMeshProUGUI playAgainText;


    private Color winnerSquareEmptyColor = new Color(0f, 0f, 0f, 130f / 255f);
    private Color winnerSquareRedColor = new Color(1f, 0f, 0f, 200f / 255f);
    private Color winnerSquareBlueColor = new Color(0f, 0f, 1f, 200f / 255f);

    private Sequence scoreboardFadeSequence;



    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        StartPlayAgainPulse();
    }


    private void OnEnable()
    {
        OfflineGameStateManager.OnGameStateChange += OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange += OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnMatchNumChange += OfflineGameStateManager_OnMatchNumChange;
        OfflineGameStateManager.OnPointChange += OfflineGameStateManager_OnPointChange;
    }

    private void OnDisable()
    {
        OfflineGameStateManager.OnGameStateChange -= OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange -= OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnMatchNumChange += OfflineGameStateManager_OnMatchNumChange;
        OfflineGameStateManager.OnPointChange -= OfflineGameStateManager_OnPointChange;
    }

    private void OfflineGameStateManager_OnGameStateChange(GameState newGameState)
    {
        if (newGameState == GameState.MatchOver)
        {
            SetElementText(playAgainText, "Press Space to Play Again");
        }
        else
        {
            SetElementText(playAgainText, "");

            if (newGameState == GameState.Rallying || newGameState == GameState.NotPlaying)
            {
                scoreboardFadeSequence?.Kill();
                scoreboardFadeSequence = DOTween.Sequence();
                scoreboardFadeSequence.Append(canvasGroup.DOFade(0f, 0.25f));
            }
        }
    }

    private void OfflineGameStateManager_OnMatchTextChange(string text)
    {
        SetElementText(matchInfo, text);
    }

    private void OfflineGameStateManager_OnMatchNumChange(int matchNum, int winner)
    {
        // color/reset the match winner squares
        if (matchNum == 0)
        {
            ResetMatchWinnerSquares();
        }
        else
        {
            Color winnerColor = winner == 1 ? winnerSquareRedColor : winnerSquareBlueColor;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(1f) // Wait 1 second
                    .AppendCallback(() =>  // Change sprite and color
                    {
                        matchWinnerSquares[matchNum - 1].color = winnerColor;
                        matchWinnerSquares[matchNum - 1].sprite = filledSquare;
                    })
                    .Append(matchWinnerSquares[matchNum - 1].rectTransform.DOScale(2f, 0.2f).SetEase(Ease.InOutQuad)) // Increase size
                    .Append(matchWinnerSquares[matchNum - 1].rectTransform.DOScale(1f, 0.2f).SetEase(Ease.InOutQuad)); // Decrease size
        }
    }

    private void OfflineGameStateManager_OnPointChange(int playerOneScore, int playerTwoScore)
    {
        // Get current scores from text
        int currentPlayerOneScore = int.Parse(this.playerOneScore.text);
        int currentPlayerTwoScore = int.Parse(this.playerTwoScore.text);

        SetElementText(this.playerOneScore, playerOneScore.ToString());
        SetElementText(this.playerTwoScore, playerTwoScore.ToString());

        scoreboardFadeSequence?.Kill();
        scoreboardFadeSequence = DOTween.Sequence();

        // Delay before showing scoreboard on new match
        if (playerOneScore == 0 && playerTwoScore == 0)
        {
            scoreboardFadeSequence.AppendInterval(1f);
        }
        scoreboardFadeSequence.Append(canvasGroup.DOFade(1f, 0.25f));

        // Check who scored
        if (playerOneScore > currentPlayerOneScore)
        {
            AnimateScoreText(this.playerOneScore);
        }
        else if (playerTwoScore > currentPlayerTwoScore)
        {
            AnimateScoreText(this.playerTwoScore);
        }
    }



    private void SetElementText(TextMeshProUGUI textElement, string text)
    {
        textElement.text = text;
    }


    private void AnimateScoreText(TextMeshProUGUI textElement)
    {
        textElement.transform.DOKill();
        Sequence pulse = DOTween.Sequence();
        pulse.AppendInterval(0.2f);
        pulse.Append(textElement.transform.DOScale(2f, 0.25f)); // Scale up
        pulse.Append(textElement.transform.DOScale(1f, 0.25f));   // Scale back down
    }

    private void StartPlayAgainPulse()
    {
        playAgainText.transform.DOScale(1.1f, 1f) // Grow to 120% over 0.5s
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Loop forever, back and forth
    }

    private void ResetMatchWinnerSquares()
    {
        foreach (Image square in matchWinnerSquares)
        {
            square.sprite = emptySquare;
            square.color = winnerSquareEmptyColor;

}
    }
}
