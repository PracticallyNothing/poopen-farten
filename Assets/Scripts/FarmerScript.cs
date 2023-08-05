using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

enum FarmerState
{
    Idle,
    Moving
}

public class FarmerScript : MonoBehaviour
{
    Rigidbody2D myBody = null;
    Animator animator = null;

    [SerializeField]
    FarmerState currState = FarmerState.Idle;
    DateTime lastMoveTime = DateTime.Now;

    // Location the farmer will try to move to in his moving state.
    // When the point is reached, the farmer stops.
    
    Vector2 moveStart = new (0, 0);

    [SerializeField]
    Vector2 moveTarget = new(0, 0);

    System.Random random = new();

    [SerializeField]
    float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 pos = transform.position;
        float dotProduct = Vector2.Dot((pos - moveTarget).normalized, (moveStart - moveTarget).normalized);

        if (currState == FarmerState.Idle && (DateTime.Now - lastMoveTime).Seconds > 5)
        {
            Debug.Log("Picking a place to move to and moving there.");
            currState = FarmerState.Moving;
            // Pick a point to move to.
            moveStart = transform.position;
            moveTarget = transform.position + new Vector3(random.Next(0, 5) * 2 - 5, 0);
            animator.SetBool("Moving", true);
            transform.localScale = new Vector3(0.7f * Math.Sign((moveStart - moveTarget).x), 0.7f, 1);
        }
        else if (currState == FarmerState.Moving && dotProduct < 0)
        {
            Debug.Log("Location reached, stopping movement.");
            currState = FarmerState.Idle;
            lastMoveTime = DateTime.Now;
            animator.SetBool("Moving", false);
        }
        else if(currState == FarmerState.Moving)
        {
            myBody.AddForce(
                (moveTarget - pos).normalized * speed,
                ForceMode2D.Impulse);
        }
    }
}
