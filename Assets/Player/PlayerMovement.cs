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

    [SerializeField] float airControl = 3;

    public float yVelocity { get => myRigidbody.velocity.y; }

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector2 currentVelocity = Vector2.zero;

    void FixedUpdate()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float targetVelocity = horizontalAxis * horizontalVelocity;

        myRigidbody.velocity = Vector2.SmoothDamp(
            myRigidbody.velocity,
            new Vector2(targetVelocity, myRigidbody.velocity.y),
            ref currentVelocity,
            horizontalAxis == 0 ? 0.5f : 0.05f
        );

        Vector2 pos = groundCollider.transform.position;
        var colliders = Physics2D.OverlapBoxAll(
            pos + groundCollider.offset,
            groundCollider.size,
            0
        );

        bool canJump = false;
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Floor"))
            {
                canJump = true;
                break;
            }
        }


        var diffMs = (DateTime.Now - (lastJumpTime ?? DateTime.Now)).TotalMilliseconds;
        bool itsTimeToJump = lastJumpTime == null || diffMs > 20;

        isGrounded = Math.Abs(myRigidbody.velocity.y - float.Epsilon) > 0;

        // Jump if the player wants to jump and we're grounded.
        if (Input.GetButton("Jump") && isGrounded && canJump && itsTimeToJump)
        {
            Debug.Log(
                String.Format(
                    "YAY - lastJumpTime={0}, diff={1}",
                    lastJumpTime,
                    diffMs));

            isGrounded = false;
            myRigidbody.AddForce(
                Vector2.up * jumpForce,
                ForceMode2D.Impulse);

            lastJumpTime = DateTime.Now;
        }
    }
}
