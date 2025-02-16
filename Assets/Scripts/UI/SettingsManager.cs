using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SettingsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI qualityButtonText;
    private int currentQuality;

    // Start is called before the first frame update
    void Start()
    {
        currentQuality = QualitySettings.GetQualityLevel();
        qualityButtonText.text = "Quality: " + QualitySettings.names[currentQuality];

    }


    public void SwitchToNextQuality()
    {
        currentQuality = QualitySettings.GetQualityLevel();
        int nextQuality = (currentQuality + 1) % QualitySettings.names.Length; // Loop back if at the highest setting
        QualitySettings.SetQualityLevel(nextQuality, applyExpensiveChanges: true);

        qualityButtonText.text = "Quality: " + QualitySettings.names[nextQuality];
    }
}
