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

public class FarmerScript : EnemyScript
{
    [SerializeField]
    FarmerState currState = FarmerState.Idle;
    DateTime lastMoveTime = DateTime.Now;

    // Location the farmer will try to move to in his moving state.
    // When the point is reached, the farmer stops.

    Vector2 moveStart = new (0, 0);
    Vector2 moveTarget = new(0, 0);

    System.Random random = new();

    [SerializeField]
    float speed = 1;

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

            Vector3 scale = transform.localScale;
            scale.x = Math.Abs(scale.x) * Math.Sign((moveStart - moveTarget).x);
            transform.localScale = scale;
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
