using System;
using Unity.VisualScripting;
using UnityEngine;

enum BoarState
{
    Idle, Moving, SlowingDown, Dead
}

public class BoarScript : EnemyScript
{
    [SerializeField]
    BoarState currState = BoarState.Idle;
    int damage = 2;

    // Whether the boar is angry. Determined by whether it's seen the player.
    bool agitated = false;
    bool blinded = false;

    // Location the farmer will try to move to in his moving state.
    // When the point is reached, the farmer stops.
    DateTime lastMoveTime = DateTime.Now;
    Vector2 moveStart = new(0, 0);
    Vector2 moveTarget = new(0, 0);

    System.Random random = new();

    // The direction towards which the boar is walking/running.
    Vector2 moveDirection;

    public override void ReactToStim(Element element, Stim stim) {
        if(element != Element.Salt || stim != Stim.Blindness)
            return;

        // TODO(Mario):
        //   Така ли е по-добре (да не реагира на солта, когато се забавя)
        //   или ще е по-добре да се въздържа от бягане ако вече е бил заслепен?
        if(currState == BoarState.SlowingDown)
            return;

        blinded = true;
    }

    // Change where the boar is moving to.
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

    // Run the logic of the Boar.
    void FixedUpdate()
    {
        // If the boar is dead, stop running logic for it.
        if (currState == BoarState.Dead)
            return;

        // NOTE(Mario):
        //   Според този код, веднъж щом те е видял, глиганът винаги знае къде си.
        //   Това ОК ли е?
        if (!agitated && SeesPlayer())
        {
            agitated = true;
            animator.SetBool("Moving", true);
            animator.SetBool("Running", true);
            UpdateMoveDirection();
            myBody.drag = 0.8f;
        }
        else if (agitated && currState == BoarState.Moving)
            moveTarget = player.transform.position;


        // We can die at any time, so we handle it outside the switch-case.
        if (health <= 0 && currState != BoarState.Dead)
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
        Debug.DrawLine(transform.position, moveTarget, Color.magenta);

        switch (currState)
        {
            case BoarState.Dead:
                return;
            case BoarState.Idle:
                if (lastMoveTime == null || DateTime.Now - lastMoveTime > new TimeSpan(0, 0, 5))
                {
                    lastMoveTime = DateTime.Now;
                    moveStart = transform.position;
                    moveTarget = transform.position + new Vector3(random.Next(1, 3) * 20 - 30, 0);
                    currState = BoarState.Moving;
                    UpdateMoveDirection();
                    animator.SetBool("Moving", true);
                }
                break;
            case BoarState.Moving:
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
                    currState = BoarState.Idle;
                    animator.SetBool("Moving", false);
                } else if(!blinded && myBody.velocity.magnitude >= 8) {
                    currState = BoarState.SlowingDown;
                    animator.SetBool("Moving", true);
                    animator.SetBool("Slowing Down", true);
                    myBody.drag = 0.05f;
                } else if(blinded && myBody.velocity.magnitude <= runSpeed) {
                    health -= 1;
                    currState = BoarState.SlowingDown;
                    blinded = false;
                }

                break;
            case BoarState.SlowingDown:
                if (myBody.velocity.magnitude < 0.05)
                {
                    UpdateMoveDirection();
                    myBody.drag = 0.7f;
                    moveStart = transform.position;
                    currState = BoarState.Moving;
                    animator.SetBool("Slowing Down", false);
                    TurnTowardsPoint(player.transform.position);
                }
                break;
        }
    }

    //  proverqva dali e blusnal igracha, ako da mu nanasq shteti
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && currState == BoarState.Moving)
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
