using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip introClip;       // Played once
    [SerializeField] private AudioClip loopingClip;     // Looped after intro
    [SerializeField] private List<AudioClip> hitClips;

    private AudioSource musicIntroSource;
    private AudioSource musicLoopSource;
    private AudioSource playerSFXAudioSource;
    private AudioSource environmentAudioSource;

    private float musicVolume = 1f;
    private float playerSFXVolume = 0.3f;
    private float environmentVolume = 1f;


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
        GameMenu.OnReturnToTitleScreen += GameMenu_OnReturnToTitleScreen;
    }

    private void OnDisable()
    {
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
        PlayerLoader.OnPlayersLoaded -= PlayerLoader_OnPlayersLoaded;
        GameMenu.OnReturnToTitleScreen -= GameMenu_OnReturnToTitleScreen;
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
        playerSFXAudioSource.PlayOneShot(hitClips[0]);
    }

    private void PlayerLoader_OnPlayersLoaded(PlayMode playMode)
    {
        musicLoopSource.DOFade(0.1f, 1f);  // Fade to target volume over time
    }

    private void GameMenu_OnReturnToTitleScreen()
    {
        musicLoopSource.DOFade(1f, 1f);  // Fade to target volume over time
    }
}
