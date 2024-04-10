using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRacketCollision : MonoBehaviour
{
    public bool isOverhandCollider;

    private HitBirdie hitBirdie;


    // Start is called before the first frame update
    void Start()
    {
        hitBirdie = gameObject.transform.parent.gameObject.GetComponent<HitBirdie>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider collider)
    {
        // Using OnTriggerStay instead of OnTriggerEnter because
        // swings did not register if the birdie was already in the collider when the swing is performed

        if (collider.gameObject.layer == LayerMask.NameToLayer("Birdie"))
        {
            hitBirdie.ApplyForceToBirdie(isOverhandCollider);
        }
    }    
}
