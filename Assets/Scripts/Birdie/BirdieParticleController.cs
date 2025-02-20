using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieParticleController : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject sparkEffectPrefab;
    [SerializeField] private GameObject blackFlashHitEffectPrefab;
    [SerializeField] private ParticleSystem blackFlashPs;
    private Rigidbody birdieRb;
    private TrailRenderer trailRenderer;

    private float timeSinceServe = 0;
    private bool timerActive = false;
    private bool blackFlashActive = false;
    private Vector3 blackFlashAngle;
    private GameEnvironmentManager gameEnv;

    void Awake()
    {
        // Trigger effects on start, since there is lag on first use of particle effect
        TriggerEffects();

    }

    // Start is called before the first frame update
    void Start()
    {
        birdieRb = GetComponentInParent<Rigidbody>();
        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();
        trailRenderer = GetComponent<TrailRenderer>();
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

    private IEnumerator EnableCameraShakeAfterDelay()
    {
        yield return new WaitForSeconds(1);
        CartoonFX.CFXR_Effect.GlobalDisableCameraShake = false;
    }

    void TriggerEffects()
    {
        // Trigger black flash effect
        CartoonFX.CFXR_Effect.GlobalDisableCameraShake = true;
        Instantiate(blackFlashHitEffectPrefab, transform.position, Quaternion.identity);
        StartCoroutine(EnableCameraShakeAfterDelay());
        // Trigger normal hit effect
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

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
        if (gameEnv.isTraining && gameEnv.id != trainingEnvId)
        {
            return;
        }

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // Get the first contact point of the collision
            ContactPoint contact = collision.contacts[0];

            // Get the exact position of the collision
            Vector3 collisionPosition = contact.point;
            Instantiate(sparkEffectPrefab, collisionPosition, Quaternion.Euler(0, 90, 0));
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

    public void EnableTrailRenderer(bool enable)
    {
        if (trailRenderer.enabled != enable)
        {
            trailRenderer.Clear();
            trailRenderer.enabled = enable;
        }
        
    }

}
