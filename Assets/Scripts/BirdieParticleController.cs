using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieParticleController : MonoBehaviour
{
    public ParticleSystem birdiePs;
    private ParticleSystem.MainModule psMain;
    public Gradient psGradient;
    private bool isPsPlaying = false;

    public ParticleSystem blackFlashPs;
    private bool isBlackFlashPlaying = false;


    private Transform birdieTransform;
    private Rigidbody birdieRb;

    private float minSpeedForParticles = 15f;
    private float timeSinceServe = 0;



    // Start is called before the first frame update
    void Start()
    {
        birdieTransform = gameObject.transform;
        birdieRb = gameObject.GetComponent<Rigidbody>();

        psMain = birdiePs.main;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateServeTimer();
        ControlParticles();
    }

    private void FixedUpdate()
    {

    }

    private void ControlParticles()
    {
        PlayBasedOnBirdieSpeed();
        ColourBasedOnBirdieAngle();
        DetectBlackFlash();
    }

    private void PlayBasedOnBirdieSpeed()
    {
        // Checking the custom variable isPsPlaying works better than the built-in .isPlaying property
        // Need this check otherwise Ps doesn't play sometimes
        if (!isPsPlaying && birdieRb.velocity.magnitude > minSpeedForParticles)
        {
            birdiePs.Play();
            isPsPlaying = true;
        }
        else if (isPsPlaying && birdieRb.velocity.magnitude < minSpeedForParticles)
        {
            birdiePs.Stop();
            isPsPlaying = false;
        }
    }

    private void ColourBasedOnBirdieAngle()
    {
        // Use absolute value of x to account for player 2 opposite direction
        float angle = XYToAngle(Mathf.Abs(birdieRb.velocity.x), birdieRb.velocity.y);

        // -60 degree angle and below = red
        // 60 degree angle and above = white
        // Add 60 and divide by 120 to have output in [0, 1]
        float gradientValue = (Mathf.Clamp(angle, -60, 60) + 60) / 120;

        psMain.startColor = psGradient.Evaluate(gradientValue);
    }


    private void DetectBlackFlash()
    {
        // Same as above - checking the custom variable isBlackFlashPlaying works better than the built-in .isPlaying property
        // Need this check otherwise Ps doesn't play sometimes
        if (!isBlackFlashPlaying && timeSinceServe < 0.75 && birdieRb.velocity.y < -10)
        {
            blackFlashPs.Play();
            isBlackFlashPlaying = true;
        }
        if (isBlackFlashPlaying && timeSinceServe > 1)
        {
            blackFlashPs.Stop();
            isBlackFlashPlaying = false;
        }
        // Use negative x value to emit particles opposite to the velocity
        blackFlashPs.transform.eulerAngles = new Vector3(XYToAngle(-birdieRb.velocity.x, birdieRb.velocity.y), 90, 0);

    }

    private float XYToAngle(float x, float y)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    private void UpdateServeTimer()
    {
        timeSinceServe += Time.deltaTime;
    }

    public void ResetServeTimer()
    {
        timeSinceServe = 0;

    }
}
