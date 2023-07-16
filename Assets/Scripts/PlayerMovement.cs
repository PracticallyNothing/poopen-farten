using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    bool CanJump;
    // Start is called before the first frame update
    void Start()
    {
        
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
    void FixedUpdate()
    {
        
        if (Input.GetKey(KeyCode.W) && CanJump)
        {
            transform.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, 10, 0), ForceMode2D.Impulse);
            CanJump= false; 
        }
        if (Input.GetKey(KeyCode.A)) transform.Translate(-0.1f, 0, 0);
        if (Input.GetKey(KeyCode.D)) transform.Translate(0.1f, 0, 0);
        
    }
}
