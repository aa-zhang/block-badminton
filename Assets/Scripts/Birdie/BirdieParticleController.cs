using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieParticleController : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject blackFlashHitEffectPrefab;
    [SerializeField] private ParticleSystem blackFlashPs;
    private Rigidbody birdieRb;

    private float timeSinceServe = 0;
    private bool timerActive = false;
    private bool blackFlashActive = false;
    private Vector3 blackFlashAngle;


    // Start is called before the first frame update
    void Start()
    {
        birdieRb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            IncrementServeTimer();
        }
        if (blackFlashActive)
        {
            SetBlackFlashParticleAngle();
        }
    }

    private void OnEnable()
    {
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
        OfflineServeController.OnHitServe += OfflineServeController_OnHitServe;

    }

    private void OnDisable()
    {
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
        OfflineServeController.OnHitServe -= OfflineServeController_OnHitServe;
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum, int trainingEnvId)
    {
        if (DetectBlackFlash(forceVector))
        {
            Instantiate(blackFlashHitEffectPrefab, transform.position, Quaternion.identity);
            blackFlashPs.Play();
            blackFlashActive = true;
            blackFlashAngle = new Vector3(XYToAngle(-birdieRb.velocity.x, birdieRb.velocity.y), 90, 0);
            // start corountine for black lightning
            StartCoroutine(StopBlackFlashParticles());

        }
        else
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

    }

    private void OfflineServeController_OnHitServe()
    {
        timeSinceServe = 0;
        timerActive = true;
    }


    private bool DetectBlackFlash(Vector3 forceVector)
    {
        return timerActive && timeSinceServe < 0.5 && forceVector.y < -8;
    }

    private void IncrementServeTimer()
    {
        timeSinceServe += Time.deltaTime;
    }

    private void SetBlackFlashParticleAngle()
    {
        // Use negative x value to emit particles opposite to the velocity
        blackFlashPs.transform.rotation = Quaternion.Euler(-blackFlashAngle);

    }

    private float XYToAngle(float x, float y)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    IEnumerator StopBlackFlashParticles()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        blackFlashPs.Stop();
        blackFlashActive = false;
        timerActive = false;
    }

}
