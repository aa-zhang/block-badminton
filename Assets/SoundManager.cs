using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip introClip;       // Played once
    [SerializeField] private AudioClip loopingClip;     // Looped after intro
    [SerializeField] private float volume = 1f;

    private AudioSource introSource;
    private AudioSource loopSource;

    void Start()
    {
        introSource = gameObject.AddComponent<AudioSource>();
        loopSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(StartMusicWhenReady());
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
        introSource.clip = introClip;
        introSource.PlayScheduled(startTime);

        loopSource.clip = loopingClip;
        loopSource.loop = true;
        loopSource.PlayScheduled(loopStartTime);
    }
}
