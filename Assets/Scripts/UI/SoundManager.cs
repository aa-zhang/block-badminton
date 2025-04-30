using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip introClip;       // Played once
    [SerializeField] private AudioClip loopingClip;     // Looped after intro
    [SerializeField] private List<AudioClip> hitClips;
    [SerializeField] private AudioClip blackFlashClip;
    [SerializeField] private AudioClip swingClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip pointScoredClip;
    [SerializeField] private AudioClip pointScoredClip2;


    private AudioSource musicIntroSource;
    private AudioSource musicLoopSource;
    private AudioSource playerSFXAudioSource;
    private AudioSource environmentAudioSource;

    private float musicVolume = 1f;
    private float playerSFXVolume = 0.25f;
    private float environmentVolume = 1f;

    private GameState state;
    private bool playServeSound = true;
    void Start()
    {
        musicIntroSource = gameObject.AddComponent<AudioSource>();
        musicLoopSource = gameObject.AddComponent<AudioSource>();
        playerSFXAudioSource = gameObject.AddComponent<AudioSource>();
        environmentAudioSource = gameObject.AddComponent<AudioSource>();

        musicIntroSource.volume = musicVolume;
        musicLoopSource.volume = musicVolume;
        playerSFXAudioSource.volume = playerSFXVolume;
        environmentAudioSource.volume = environmentVolume;

        StartCoroutine(StartMusicWhenReady());
    }

    private void OnEnable()
    {
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
        PlayerLoader.OnPlayersLoaded += PlayerLoader_OnPlayersLoaded;
        OfflineGameStateManager.OnGameStateChange += OfflineGameStateManager_OnGameStateChange;
        BirdieParticleController.OnBlackFlash += BirdieParticleController_OnBlackFlash;
        PlayerMovement.OnJump += PlayerMovement_OnJump;
        PlayerMovement.OnSwing += PlayerMovement_OnSwing;
        OfflineBirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
    }

    private void OnDisable()
    {
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
        PlayerLoader.OnPlayersLoaded -= PlayerLoader_OnPlayersLoaded;
        OfflineGameStateManager.OnGameStateChange -= OfflineGameStateManager_OnGameStateChange;
        BirdieParticleController.OnBlackFlash -= BirdieParticleController_OnBlackFlash;
        PlayerMovement.OnJump -= PlayerMovement_OnJump;
        PlayerMovement.OnSwing -= PlayerMovement_OnSwing;
        OfflineBirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
    }

    IEnumerator StartMusicWhenReady()
    {
        // Wait until both clips are ready
        while (!introClip.loadState.Equals(AudioDataLoadState.Loaded) ||
               !loopingClip.loadState.Equals(AudioDataLoadState.Loaded))
        {
            yield return null;
        }

        double startTime = AudioSettings.dspTime;

        // Accurate length in seconds using samples
        double introDuration = (double)introClip.samples / introClip.frequency;
        double loopStartTime = startTime + introDuration;

        // Schedule both
        musicIntroSource.clip = introClip;
        musicIntroSource.PlayScheduled(startTime);

        musicLoopSource.clip = loopingClip;
        musicLoopSource.loop = true;
        musicLoopSource.PlayScheduled(loopStartTime);
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum, int gameEnvId)
    {
        if (playServeSound)
        {
            playerSFXAudioSource.PlayOneShot(hitClips[2]);
            playServeSound = false;
        }
        else if (forceVector.y < -5)
        {
            playerSFXAudioSource.PlayOneShot(hitClips[1]);
        }
        else
        {
            playerSFXAudioSource.PlayOneShot(hitClips[0]);
        }
    }

    private void BirdieParticleController_OnBlackFlash()
    {
        playerSFXAudioSource.PlayOneShot(blackFlashClip);
    }

    private void PlayerLoader_OnPlayersLoaded(PlayMode playMode)
    {
        musicLoopSource.DOFade(0.2f, 1f);  // Fade to target volume over time
    }

    private void OfflineGameStateManager_OnGameStateChange(GameState gameState)
    {
        if (gameState == GameState.NotPlaying)
        {
            musicLoopSource.DOFade(1f, 1f);  // Fade to target volume over time
        }

        if (gameState == GameState.Serving)
        {
            playServeSound = true;
        }

        state = gameState;

    }

    private void PlayerMovement_OnJump()
    {
        playerSFXAudioSource.PlayOneShot(jumpClip);
    }

    private void PlayerMovement_OnSwing()
    {
        playerSFXAudioSource.PlayOneShot(swingClip);
    }

    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        if (scoringPlayerNum == 1)
        {
            playerSFXAudioSource.PlayOneShot(pointScoredClip);
        }
        else
        {
            playerSFXAudioSource.PlayOneShot(pointScoredClip2);
        }
    }

}
