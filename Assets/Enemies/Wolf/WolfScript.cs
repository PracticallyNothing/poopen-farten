using System;
using UnityEngine;

enum WolfState
{
    Idle, Moving, SlowingDown, Dead
}

public class WolfScript : EnemyScript
{
    [SerializeField]
    WolfState currState = WolfState.Idle;

    // Whether the Wolf is angry. Determined by whether it's seen the player.
    bool agitated = false;

    // Location the farmer will try to move to in his moving state.
    // When the point is reached, the farmer stops.
    DateTime lastMoveTime = DateTime.Now;
    Vector2 moveStart = new(0, 0);
    Vector2 moveTarget = new(0, 0);

    System.Random random = new();

    // The direction towards which the Wolf is walking/running.
    Vector2 moveDirection;

    // Change where the Wolf is moving to.
    void UpdateMoveDirection() {
        if(agitated) {
            moveDirection = new Vector2((player.transform.position - transform.position).x, 0).normalized;
            TurnTowardsPoint(player.transform.position);
        } else {
            Vector2 pos = transform.position;
            moveDirection = new Vector2((moveTarget - moveStart).x, 0).normalized;
            TurnTowardsPoint(moveTarget);
        }
    }

    // Run the logic of the Wolf.
    void FixedUpdate()
    {
        // If the Wolf is dead, stop running logic for it.
        if (currState == WolfState.Dead)
            return;

        // NOTE(Mario):
        //   Според този код, веднъж щом те е видял, вълкът винаги знае къде си.
        //   Това ОК ли е?
        if (!agitated && SeesPlayer())
        {
            Debug.Log("BRRR IM ANGY 😠");
            agitated = true;
            animator.SetBool("Moving", true);
            animator.SetBool("Running", true);
            UpdateMoveDirection();
            myBody.drag = 0.8f;
        }
        else if (agitated && currState == WolfState.Moving)
            moveTarget = player.transform.position;


        // We can die at any time, so we handle it outside the switch-case.
        if (health <= 0 && currState != WolfState.Dead)
        {
            myBody.simulated = false;
            currState = WolfState.Dead;
            animator.SetBool("Alive", false);
            return;
        }

        Debug.DrawRay(
            transform.position,
            myBody.velocity,
            myBody.velocity.magnitude < 0.05 ? Color.red : Color.green);
        Debug.DrawLine(transform.position, moveTarget, Color.magenta);

        switch (currState)
        {
            case WolfState.Dead:
                return;
            case WolfState.Idle:
                if (lastMoveTime == null || DateTime.Now - lastMoveTime > new TimeSpan(0, 0, 5))
                {
                    lastMoveTime = DateTime.Now;
                    moveStart = transform.position;
                    moveTarget = transform.position + new Vector3(random.Next(1, 3) * 20 - 30, 0);
                    currState = WolfState.Moving;
                    UpdateMoveDirection();
                    animator.SetBool("Moving", true);
                }
                break;
            case WolfState.Moving:
                float moveForce = agitated ? runSpeed : walkSpeed;

                Vector2 pos = transform.position;
                Vector2 targetDir = (moveTarget - pos).normalized;

                myBody.AddForce(moveDirection * moveForce, ForceMode2D.Impulse);

                Debug.DrawRay(transform.position + Vector3.one * 0.2f, moveDirection * 2, Color.magenta);
                Debug.DrawRay(transform.position - Vector3.one * 0.2f, targetDir * 3, Color.yellow);

                // NOTE(Mario):
                //   Как работи Vector2.Dot():
                //     Vector2.Dot(←, ←) = 1
                //     Vector2.Dot(←, →) = -1
                //
                //     Vector2.Dot(←, ↑) = 0
                //       или
                //     Vector2.Dot(←, ↓) = 0 (т.е векторите са под 90° ъгъл)
                bool hasPassedTarget = Vector2.Dot(moveDirection, targetDir) < 0;

                if (!hasPassedTarget)
                    break;

                if (!agitated) {
                    currState = WolfState.Idle;
                    animator.SetBool("Moving", false);
                } else if(myBody.velocity.magnitude >= 8) {
                    currState = WolfState.SlowingDown;
                    animator.SetBool("Moving", true);
                    animator.SetBool("Slowing Down", true);
                    myBody.drag = 0.05f;
                }

                break;
            case WolfState.SlowingDown:
                Debug.Log(String.Format("🟥 Slowing down! {0}", myBody.velocity.magnitude));
                if (myBody.velocity.magnitude < 0.05)
                {
                    UpdateMoveDirection();
                    myBody.drag = 0.7f;
                    Debug.Log(String.Format("🟢 Moving again!!!!"));
                    moveStart = transform.position;
                    currState = WolfState.Moving;
                    animator.SetBool("Slowing Down", false);
                    TurnTowardsPoint(player.transform.position);
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Floor") && !agitated)
        {
            Debug.Log(moveTarget + " tuka iskam ma ne moje");
            moveTarget = moveTarget - 2*(moveTarget - moveStart);
            UpdateMoveDirection();
            Debug.Log(moveTarget);
        }
    }
}
