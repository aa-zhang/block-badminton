using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class HitBirdie : MonoBehaviour
{
    private Transform racketTransform;
    private SwingRacket swingRacket;
    private GameObject player;
    private PlayerManager playerManager;
    private IServing serveController;

    [SerializeField] private float racketForce = 300;
    [SerializeField] private float birdieAngleAdjustment = 0.1f;

    public delegate void BirdieHitHandler(Vector3 force, int playerNum);
    public static BirdieHitHandler OnBirdieHit;


    // Start is called before the first frame update
    void Start()
    {
        racketTransform = gameObject.transform;
        swingRacket = gameObject.GetComponent<SwingRacket>();
        player = racketTransform.parent.gameObject;
        playerManager = player.GetComponent<PlayerManager>();
        if (GetComponent<ServeController>() != null)
        {
            serveController = player.GetComponent<ServeController>();
        }
        else
        {
            serveController = player.GetComponent<OfflineServeController>();
        }
    }


    public void CalculateBirdieForce(bool isOverhandCollider)
    {
        if ((swingRacket.overhand && !isOverhandCollider) || (!swingRacket.overhand && isOverhandCollider) || !swingRacket.inForwardSwingAnimation || swingRacket.alreadyMadeContact)
        {
            // Ignore collision if collision was registered for the incorrect collider
            // OR racket is currently in the backward swinging animation
            // OR collision was already registered during this swing
            return;
        }

        Vector3 hitAngle;
        hitAngle = CalculateRegularHitAngle();
        


        OnBirdieHit(hitAngle * racketForce, playerManager.playerNum);
        swingRacket.SetAlreadyMadeContact(true);
        Debug.Log($"hit the bertholdt. ARE WE DOING IT REINIER?!?!? angle: {hitAngle}");

    }

    private Vector3 CalculateRegularHitAngle()
    {
        float racketAngle;
        if (serveController.isServing)
        {
            racketAngle = serveController.GetServeAngle().z - 180;
        }
        else
        {
            racketAngle = racketTransform.localEulerAngles.z;
        }

        if (playerManager.playerNum == 2)
        {
            racketAngle = 180 - racketAngle;
        }

        // Convert degrees to radians
        racketAngle *= Mathf.Deg2Rad;

        // Calculate (x, y, z) vector given the angle in radians
        Vector3 hitAngle = new Vector3(Mathf.Cos(racketAngle), Mathf.Sin(racketAngle), 0);

        // Need to use oppposite direction if hitting with underhand swing
        hitAngle = swingRacket.overhand ? hitAngle : -hitAngle;

        // Add a slight angle adjustment and then normalize the vector
        hitAngle = new Vector3(hitAngle.x, hitAngle.y + birdieAngleAdjustment, hitAngle.z).normalized;

        return hitAngle;
    }


}
