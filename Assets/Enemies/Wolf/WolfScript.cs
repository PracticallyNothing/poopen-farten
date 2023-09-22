using System;
using UnityEngine;

enum WolfState
{
    Idle,
    Moving,
    Dead,

    PreparingLunge,
    Lunging,
    Landing
}

// Вълкът има 4 състояния, когато се ядоса:
// 1. твърде далеч е от играча и трябва да се приближи
// 2. достатъчно близо е до играча, за да скочи към него, подготвя се да скочи
// 3. лети във въздуха след като е скочил
// 4. приземява се след скока си

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

    public void Lunge()
    {
        // TODO(Mario):
        //   В момента вълка перфектно те прескача, а идеята е да минава на
        //   височина лицето ти, освен ако не клекнеш.
        myBody.AddForce(new Vector3((moveDirection * 40).x, 30, 0), ForceMode2D.Impulse);
        currState = WolfState.Lunging;
    }

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

    /// How close the wolf needs to get to the player before jumping at him.
    [SerializeField] float maxDistanceBeforeLunge = 10;

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
            currState = WolfState.Moving;
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
                Vector2 pos = transform.position;
                Vector2 targetDir = (moveTarget - pos).normalized;

                float moveForce = agitated ? runSpeed : walkSpeed;
                /// Calculate the "final force" that will be applied, which is the moveForce, limited by the maximum velocity.
                float finalForce = Math.Min(
                    // NOTE(Mario):
                    //   (maxVelocity - currentVelocity) може да стане отрицателно и много бързо става много смешно - вълкът
                    //   хвръква на хиляди единици в грешната посока. Затова, ако е под 0, просто го заместваме с нула.
                    Math.Max(0, maxVelocity - myBody.velocity.magnitude),
                    moveForce
                );
                myBody.AddForce(moveDirection * finalForce, ForceMode2D.Impulse);

                if (!agitated) {
                    currState = WolfState.Idle;
                    animator.SetBool("Moving", false);
                }

                float distToPlayer = (player.transform.position - transform.position).magnitude;

                if (distToPlayer <= maxDistanceBeforeLunge)
                {
                    animator.SetBool("Lunging", true);
                    myBody.velocity = Vector3.zero;
                    currState = WolfState.PreparingLunge;
                    break;
                }

                UpdateMoveDirection();
                moveStart = transform.position;
                TurnTowardsPoint(player.transform.position);
                break;
            case WolfState.PreparingLunge:
                break;
            case WolfState.Lunging:
                if (Vector2.Dot(myBody.velocity.normalized, new Vector2(0, -1)) >= 0.3)
                {
                    animator.SetBool("Lunging", false);
                    currState = WolfState.Landing;
                }
                break;
            case WolfState.Landing:
                if (myBody.velocity.y < 0.3)
                {
                    moveStart = transform.position;
                    TurnTowardsPoint(player.transform.position);
                    UpdateMoveDirection();
                    currState = WolfState.Moving;
                }
                break;
        }
    }
}
