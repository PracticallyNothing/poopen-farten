using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canJump = true;
    Rigidbody2D myRigidbody = null;
    DateTime? lastJump = null;
    const float maxHorizontalVel = 12;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var vel = myRigidbody.velocity;
        // Debug.Log(lastJump == null || DateTime.Now - lastJump > new TimeSpan(0, 0, 2));

        if (Input.GetKey(KeyCode.Space) && canJump &&
            (lastJump == null || DateTime.Now - lastJump > new TimeSpan(0, 0, 0, 0, 250)))
        {
            lastJump = DateTime.Now;
            myRigidbody.AddForce(
                new Vector3(0, 40, 0),
                ForceMode2D.Impulse);
            canJump = false;
        }
        
        if (Input.GetKey(KeyCode.A)) {
            myRigidbody.AddForce(
                new Vector3(-Math.Clamp(maxHorizontalVel - Math.Abs(vel.x), 0, 2), 0, 0),
                ForceMode2D.Impulse);
        }

        if (Input.GetKey(KeyCode.D)) {
            myRigidbody.AddForce(
                new Vector3(Math.Clamp(maxHorizontalVel - vel.x, 0, 2), 0, 0),
                ForceMode2D.Impulse);
        }
        
    }
}
