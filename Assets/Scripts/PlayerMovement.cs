using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    bool CanJump = true;
    Rigidbody2D myRigidbody = null;
    DateTime? lastJump = null;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponentInChildren<Rigidbody2D>();
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor")) CanJump = true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KillZone"))
        {
            collision.gameObject.GetComponent<DeathHandle>().Death();
        }
    }

    const float maxHorizontalVel = 12;

    void FixedUpdate()
    {
        var vel = myRigidbody.velocity;
        // Debug.Log(lastJump == null || DateTime.Now - lastJump > new TimeSpan(0, 0, 2));

        if (Input.GetKey(KeyCode.W) && 
            (lastJump == null || DateTime.Now - lastJump > new TimeSpan(0, 0, 0, 0, 250)))
        {
            lastJump = DateTime.Now;
            myRigidbody.AddForce(
                new Vector3(0, 40, 0),
                ForceMode2D.Impulse);
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
