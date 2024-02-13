using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Whether the player is touching the ground.
    public bool isGrounded = true;

    Rigidbody2D myRigidbody = null;

    [SerializeField] BoxCollider2D groundCollider;

    DateTime? lastJumpTime = null;

    /// How high the character jumps.
    [SerializeField] float jumpForce = 40;

    /// The fastest speed the player character may move at.
    /// Increase for faster overall running.
    [SerializeField] float horizontalVelocity = 20;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector2 currentVelocity = Vector2.zero;

    int direction = 1;

    void Update()
    {
        if (myRigidbody.velocity.x > 0.01f)
        {
            transform.rotation = Quaternion.identity;
        }
        else if (myRigidbody.velocity.x < -0.01f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float targetVelocity = horizontalAxis * horizontalVelocity;

        if(Math.Abs(horizontalAxis) > 0)
            myRigidbody.velocity = Vector2.SmoothDamp(
                myRigidbody.velocity,
                new Vector2(targetVelocity, myRigidbody.velocity.y),
                ref currentVelocity,
                horizontalAxis == 0 ? 0.5f : 0.05f
            );

        var diffMs = (DateTime.Now - (lastJumpTime ?? DateTime.Now)).TotalMilliseconds;
        bool itsTimeToJump = lastJumpTime == null || diffMs > 20;

        bool notFalling = Math.Abs(myRigidbody.velocity.y) < 0.01;

        // Jump if the player wants to jump and we're grounded.
        if (Input.GetButton("Jump") && isGrounded && itsTimeToJump)
        {
            Debug.Log(
                String.Format(
                    "YAY - lastJumpTime={0}, diff={1}",
                    lastJumpTime,
                    diffMs));

            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0);

            myRigidbody.AddForce(
                Vector2.up * jumpForce,
                ForceMode2D.Impulse);

            lastJumpTime = DateTime.Now;
        }
    }
}
