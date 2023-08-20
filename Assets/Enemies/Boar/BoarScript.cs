using System;
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

    [SerializeField] float viewRange = 40;
    [SerializeField] float walkSpeed = 0.35f;
    [SerializeField] float runSpeed = 1f;

    // The direction towards which the boar is walking/running.
    Vector2 moveDirection;

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
        if (dirToPlayer.magnitude > viewRange)
        {
            return false;
        }

        // NOTE(Mario):
        //   По някаква причина, макар да прочетох че Ray, изстрелян вътре в Collider,
        //   не се удря в този Collider, винаги лъчът се удря в глигана първо. Затова правя RaycastAll()
        //   и проверявам втория елемент във hitResult - първият винаги е самият глиган.
        var hitResult = Physics2D.RaycastAll(transform.position, dirToPlayer.normalized, 0.9f * viewRange);
        Debug.DrawRay(transform.position, dirToPlayer);

        if (hitResult.Length == 1)
        {
            return false;
        }

        bool rayHitPlayer = hitResult[1].collider.gameObject.CompareTag("Player");
        return rayHitPlayer;
    }

    // Mirror the boar so it's looking at the given point.
    void TurnTowardsPoint(Vector2 point)
    {
        Vector3 scale = transform.localScale;
        Vector2 pos = transform.position;

        float dir = (pos - point).normalized.x;
        scale.x = Math.Abs(scale.x) * Math.Sign(dir == 0 ? 1 : dir);
        transform.localScale = scale;
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
            Debug.Log("BRRR IM ANGY 😠");
            agitated = true;
            animator.SetBool("Moving", true);
            animator.SetBool("Running", true);
            UpdateMoveDirection();
            myBody.drag = 0.8f;
        }
        else if (agitated && currState == BoarState.Moving)
            moveTarget = player.transform.position;
        else if (agitated && SeesPlayer() == false)
        {
            Debug.Log("Uspokoih sa");
            animator.SetBool("Running", false);
            agitated = false;
        }
            

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
                else
                    UpdateMoveDirection();

                if (!agitated) {
                    currState = BoarState.Idle;
                    animator.SetBool("Moving", false);
                } else if(myBody.velocity.magnitude >= 8) {
                    currState = BoarState.SlowingDown;
                    animator.SetBool("Moving", true);
                    animator.SetBool("Slowing Down", true);
                    myBody.drag = 0.05f;
                }

                break;
            case BoarState.SlowingDown:
                Debug.Log(String.Format("🟥 Slowing down! {0}", myBody.velocity.magnitude));
                if (myBody.velocity.magnitude < 0.05)
                {
                    UpdateMoveDirection();
                    myBody.drag = 0.7f;
                    Debug.Log(String.Format("🟢 Moving again!!!!"));
                    moveStart = transform.position;
                    currState = BoarState.Moving;
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
