using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Whether hitting the Spacebar does anything.
    public bool canJump = true;

    Rigidbody2D myRigidbody = null;

    DateTime? lastJump = null;

    /// How high the character jumps.
    [SerializeField] float jumpForce = 40;

    /// The fastest speed the player character may move at.
    /// Increase for faster overall running.
    [SerializeField] float maxHorizontalVelocity = 12;

    /// The maximum amount of force that can be applied while accelerating.
    /// Increase if you want faster acceleration.
    [SerializeField] float maxMoveForce = 2;

    /// The amount of time to wait before allowing the player to jump again.
    /// Used for avoiding spamming.
    [SerializeField] int millisecondsBetweenJumps = 350;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    bool prevCrouching = false;

    void FixedUpdate()
    {
        bool crouching = Input.GetKey(KeyCode.S);
        bool itsTimeToJump = lastJump == null || (DateTime.Now - lastJump).Value.TotalMilliseconds > millisecondsBetweenJumps;

        if (Input.GetKey(KeyCode.Space) && itsTimeToJump && canJump && !crouching) {
            lastJump = DateTime.Now;
            myRigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode2D.Impulse);
            myRigidbody.mass = 0.3f;
        }

        Vector2 scale = transform.localScale;
        scale.y = crouching ? 0.6f : 2f;
        transform.localScale = scale;

        bool justCrouched =  crouching && !prevCrouching;
        bool justStoodUp  = !crouching &&  prevCrouching;

        Vector2 pos = transform.localPosition;

        if(justCrouched)     pos.y -= GetComponent<BoxCollider2D>().size.y * 0.6f * 0.6f;
        else if(justStoodUp) pos.y += GetComponent<BoxCollider2D>().size.y * 0.6f * 0.6f;

        if(justCrouched || justStoodUp) transform.localPosition = pos;


        Vector2 vel = myRigidbody.velocity;
        float moveForce = Math.Clamp(
            maxHorizontalVelocity / (crouching ? 2 : 1) - Math.Abs(vel.x), 0, maxMoveForce);

        if (Input.GetKey(KeyCode.A))
            myRigidbody.AddForce(new Vector3(-moveForce, 0, 0), ForceMode2D.Impulse);
        else if (Input.GetKey(KeyCode.D))
            myRigidbody.AddForce(new Vector3(moveForce, 0, 0), ForceMode2D.Impulse);

        prevCrouching = crouching;
    }
}
