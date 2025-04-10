using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

// Reference: https://gist.github.com/st4rdog/80057b406bfd00f44c8ec8796a071a13
public class StatsMonitor : MonoBehaviour
{
	public enum DeltaTimeType
	{
		Smooth,
		Unscaled
	}

	[SerializeField] private TextMeshProUGUI fpsCounterText;
	[SerializeField] private TextMeshProUGUI pingText;

	[Tooltip("Unscaled is more accurate, but jumpy, or if your game modifies Time.timeScale. Use Smooth for smoothDeltaTime.")]
	[SerializeField] private DeltaTimeType DeltaType = DeltaTimeType.Smooth;

	private Dictionary<int, string> CachedNumberStrings = new();

	private int[] _frameRateSamples;
	private int _cacheNumbersAmount = 300;
	private int _averageFromAmount = 30;
	private int _averageCounter;
	private int _currentAveraged;

	void Awake()
	{
		// Cache strings and create array
		{
			for (int i = 0; i < _cacheNumbersAmount; i++)
			{
				CachedNumberStrings[i] = i.ToString();
			}

			_frameRateSamples = new int[_averageFromAmount];
		}
	}

	void Update()
	{
		{
			if (fpsCounterText != null && Time.timeScale != 0f)
			{
                fpsCounterText.text = $"{CalculateFPS()} FPS";
            }
			if (pingText != null)
			{
                pingText.text = $"{GetPing()} Ping";
            }
        }
	}

	private int CalculateFPS()
    {
		// Sample
		{
			var currentFrame = (int)Math.Round(1f / DeltaType switch
			{
				DeltaTimeType.Smooth => Time.smoothDeltaTime,
				DeltaTimeType.Unscaled => Time.unscaledDeltaTime,
				_ => Time.unscaledDeltaTime
			});
			_frameRateSamples[_averageCounter] = currentFrame;
		}

		// Average
		{
			var average = 0f;

			foreach (var frameRate in _frameRateSamples)
			{
				average += frameRate;
			}

			_currentAveraged = (int)Math.Round(average / _averageFromAmount);
			_averageCounter = (_averageCounter + 1) % _averageFromAmount;
		}

		return _currentAveraged;
	}

	private int GetPing()
    {
		return (int)((NetworkManager.Singleton.LocalTime.Time - NetworkManager.Singleton.ServerTime.Time) * 100);
	}
}