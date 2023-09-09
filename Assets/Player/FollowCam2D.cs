using System;
using UnityEngine;

/// A class that makes a camera follow a Rigidbody2D.
///
/// It flies to the target's position, then follows its velocity 
/// with a configurable swing and offset.
public class FollowCam2D : MonoBehaviour
{
    [SerializeField]
    GameObject FollowTarget = null;

    Vector3 currVelocity = new(0, 1.80f, 0);

    Rigidbody2D targetRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        if (FollowTarget == null)
            throw new Exception("FollowCam2D must have an object attached!");
        else if(FollowTarget.GetComponent<Rigidbody2D>() == null)
            throw new Exception("FollowCam2D must be set to follow an object with a Rigidbody2D!");

        targetRigidbody = FollowTarget.GetComponent<Rigidbody2D>();
    }

    // NOTE(Mario):
    //   Мерси, ChatGPT 4!
    public float minSmoothTime = 0.35f; // Minimum time it takes for the camera to catch up to the target
    public float maxSmoothTime = 0.80f; // Maximum time it takes for the camera to catch up to the target   
    public Vector3 offset; // Fixed offset
    public float velocityFactor = 1.30f; // Factor for how much the camera should lead in front of the player

    void LateUpdate()
    {
        float targetSpeed = targetRigidbody.velocity.magnitude;
        float smoothTime = Mathf.Lerp(maxSmoothTime, minSmoothTime, targetSpeed / 10f);

        Vector3 targetPos = 
          FollowTarget.transform.position
          + offset
          + velocityFactor * new Vector3(targetRigidbody.velocity.x, 0, 0);


        // FIXME(Mario):
        //   Когато почнем да имаме повече вертикалност в нивата,
        //   това ще ни захапе по гъзовете.
        // Ignore the player's Y.
        targetPos.y -= FollowTarget.transform.position.y;

        Vector3 res = 
          Vector3.SmoothDamp(
            transform.position,
            targetPos, 
            ref currVelocity,
            smoothTime);

        res.z = -10;
        
        transform.position = res;
    }
}
