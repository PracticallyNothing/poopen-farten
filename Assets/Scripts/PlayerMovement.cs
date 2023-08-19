using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Whether hitting the Spacebar does anything.
    public bool canJump = true;

    Rigidbody2D myRigidbody = null;

    // Time of last jump. Used in conjunction with delayBetweenJumpsMs.
    DateTime? lastJump = null;

    /// Time to wait before allowing the player to jump again.
    /// Used to counteract spam-jumping.
    [SerializeField] int delayBetweenJumpsMs = 250;

    /// How high the character jumps.
    [SerializeField] float jumpForce = 40;

    /// The fastest speed the player character may move at.
    /// Increase for faster overall running.
    [SerializeField] float maxHorizontalVelocity = 12;

    /// The maximum amount of force that can be applied while accelerating.
    /// Increase if you want faster acceleration.
    [SerializeField] float maxMoveForce = 2;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space) && canJump &&
            (lastJump == null
             || DateTime.Now - lastJump > new TimeSpan(0, 0, 0, 0, delayBetweenJumpsMs)))
        {
            lastJump = DateTime.Now;
            myRigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode2D.Impulse);
            canJump = false;
        }

        Vector2 vel = myRigidbody.velocity;
        float moveForce = Math.Clamp(maxHorizontalVelocity - Math.Abs(vel.x), 0, maxMoveForce);

        if (Input.GetKey(KeyCode.A))
        {
            myRigidbody.AddForce(new Vector3(-moveForce, 0, 0), ForceMode2D.Impulse);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            myRigidbody.AddForce(new Vector3(moveForce, 0, 0), ForceMode2D.Impulse);
        }
    }
}
