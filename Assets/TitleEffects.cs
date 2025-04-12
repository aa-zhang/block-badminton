using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleEffects : MonoBehaviour
{
    [SerializeField] private float cubeRotationSpeed = 20f; // degrees per second
    [SerializeField] private RectTransform cubeRectTransform;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateCubeAnimation();
    }

    private void RotateCubeAnimation()
    {
        cubeRectTransform.Rotate(0f, 0f, -cubeRotationSpeed * Time.deltaTime);
    }
}
