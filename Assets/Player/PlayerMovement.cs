using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Whether hitting the Spacebar does anything.
    public bool canJump = true;

    Rigidbody2D myRigidbody = null;

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

    void FixedUpdate()
    {
        float targetVelocity =
            Input.GetAxisRaw("Horizontal") * horizontalVelocity;

        myRigidbody.velocity = Vector2.SmoothDamp(
            myRigidbody.velocity,
            new Vector2(targetVelocity, myRigidbody.velocity.y),
            ref currentVelocity,
            0.05f);

        // Jump if the player wants to jump and we're grounded.
        if (Input.GetButton("Jump") && canJump)
        {
            myRigidbody.AddForce(
                Vector2.up * jumpForce,
                ForceMode2D.Impulse);
        }
    }
}
