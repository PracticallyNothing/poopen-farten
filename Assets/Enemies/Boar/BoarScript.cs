using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

enum BoarState
{
    Idle, Moving, SlowingDown, Dead
}

public class BoarScript : MonoBehaviour
{
    Rigidbody2D myBody = null;
    Animator animator = null;

    GameObject player = null;

    EnemyScript enemyPart = null;

    [SerializeField]
    BoarState currState = BoarState.Idle;

    // Whether the boar is angry. Determined by whether it's seen the player.
    bool agitated = false;

    // Location the farmer will try to move to in his moving state.
    // When the point is reached, the farmer stops.
    DateTime lastMoveTime = DateTime.Now;
    Vector2 moveStart = new(0, 0);
    Vector2 moveTarget = new(0, 0);

    System.Random random = new();

    [SerializeField] float walkSpeed = 0.35f;
    [SerializeField] float runSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyPart = GetComponent<EnemyScript>();

        player = GameObject.Find("Player");
    }

    // Check whether the boar can see the player.
    bool SeesPlayer()
    {
        Vector2 dirToPlayer = player.transform.position - transform.position;

        // If the player is way too far away, we definitely don't see them.
        // Don't waste time firing a ray.
        if (dirToPlayer.magnitude > 20 * 4)
        {
            Debug.Log(String.Format("⛰ BROOO, you're way too far! {0}", dirToPlayer.magnitude));
            return false;
        }

        var hitResult = Physics2D.Raycast(transform.position, dirToPlayer.normalized, 15 * 4);
        bool rayHitPlayer = hitResult.collider.gameObject.CompareTag("Player");

        Debug.DrawRay(transform.position, dirToPlayer.normalized, rayHitPlayer ? Color.green : Color.red);

        return rayHitPlayer;
    }

    // Mirror the boar so it's looking at the given point.
    void TurnTowardsPoint(Vector2 point)
    {
        Vector3 scale = transform.localScale;
        Vector2 pos = transform.position;
        scale.x = Math.Abs(scale.x) * Math.Sign((point - pos).x);
        transform.localScale = scale;
    }

    // Run the logic of the Boar.
    void FixedUpdate()
    {
        // If the boar is dead, stop running logic for it.
        if(currState == BoarState.Dead) {
            return;
        }

        // NOTE(Mario):
        //   Според този код, веднъж щом те е видял, глиганът винаги знае къде си.
        //   Това ОК ли е?
        if (!agitated && SeesPlayer()) {
            Debug.Log("BRRR IM ANGY 😠");
            agitated = true;
        }
        else if (agitated && currState == BoarState.Moving)
            moveTarget = player.transform.position;

        // We can die at any time, so we handle it outside the switch-case.
        if (enemyPart.health <= 0 && currState != BoarState.Dead)
        {
            myBody.simulated = false;
            currState = BoarState.Dead;
            animator.SetBool("Alive", false);
            return;
        }

        Debug.DrawRay(
            transform.position,
            myBody.velocity,
            myBody.velocity.magnitude < 0.05 ? Color.red : Color.green);

        switch (currState)
        {
            case BoarState.Dead:
                return;
            case BoarState.Idle:
                if (agitated)
                {
                    TurnTowardsPoint(player.transform.position);
                    animator.SetBool("Moving", true);
                    animator.SetBool("Running", true);
                }
                else if (lastMoveTime == null || DateTime.Now - lastMoveTime > new TimeSpan(0, 0, 5))
                {
                    lastMoveTime = DateTime.Now;
                    moveStart = transform.position;
                    moveTarget = transform.position + new Vector3(random.Next(1, 4) * 5 - 12.5f, 0);
                    TurnTowardsPoint(moveTarget);
                    currState = BoarState.Moving;
                    animator.SetBool("Moving", true);
                }
                break;
            case BoarState.Moving:
                float moveForce = agitated ? runSpeed : walkSpeed;

                Vector2 pos = transform.position;

                Vector2 moveDir = moveTarget - moveStart;
                Vector2 targetDir = moveTarget - pos;

                myBody.AddForce(
                    new Vector2(targetDir.x, 0).normalized * moveForce,
                    ForceMode2D.Impulse);

                // NOTE(Mario):
                //   Как работи Vector2.Dot():
                //     Vector2.Dot(←, ←) = 1
                //     Vector2.Dot(←, →) = -1
                //
                //     Vector2.Dot(←, ↑) = 0
                //       или
                //     Vector2.Dot(←, ↓) = 0 (т.е векторите са под 90° ъгъл)
                bool hasPassedTarget = Vector2.Dot(moveDir.normalized, targetDir.normalized) < 0;

                if (hasPassedTarget)
                {
                    currState = agitated ? BoarState.SlowingDown : BoarState.Idle;
                    // Stop moving if the boar is agitated, keep running otherwise.
                    animator.SetBool("Moving", agitated);
                    animator.SetBool("Slowing Down", true);
                }
                break;
            case BoarState.SlowingDown:
                if (myBody.velocity.magnitude < 0.05)
                {
                    currState = BoarState.Moving;
                    animator.SetBool("SlowingDown", false);
                    TurnTowardsPoint(player.transform.position);
                }
                break;
        }
    }
}
