using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieParticleController : MonoBehaviour
{
    public ParticleSystem normalPs;

    private Transform birdieTransform;
    private Rigidbody birdieRb;

    private float minSpeedForParticles = 15f;


    // Start is called before the first frame update
    void Start()
    {
        birdieTransform = gameObject.transform;
        birdieRb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ControlParticles();
    }

    private void ControlParticles()
    {
        if (birdieRb.velocity.magnitude < minSpeedForParticles)
        {
            normalPs.Stop();
        }
        else
        {
            normalPs.Play();
        }
    }

}
