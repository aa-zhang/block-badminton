using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrailController : MonoBehaviour
{
    private TrailRenderer tr;
    [SerializeField] private float dashTrailDuration = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<TrailRenderer>();
    }

    public void EmitDashTrail()
    {
        StartCoroutine(ControlEmission());
    }

    IEnumerator ControlEmission()
    {
        tr.emitting = true;
        yield return new WaitForSeconds(dashTrailDuration);
        tr.emitting = false;
    }

}
