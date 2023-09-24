using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class EnemyScript : MonoBehaviour, StimResponder
{
    [SerializeField]
    public int health = 5;

    protected Rigidbody2D myBody = null;
    protected Animator animator = null;
    protected static GameObject player = null;

    [SerializeField] protected float maxVelocity = 15;

    [SerializeField] protected float viewRange = 40;
    [SerializeField] protected float walkSpeed = 0.35f;
    [SerializeField] protected float runSpeed = 1f;

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Potion"))
        {
            // other.gameObject.GetComponent<Potion>().OnHit(gameObject);
        }
    }

    // Check whether the enemy can see the player.
    protected bool SeesPlayer()
    {
        if(player == null) {
            player = GameObject.Find("Player");
        }

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
    protected void TurnTowardsPoint(Vector2 point)
    {
        Vector3 scale = transform.localScale;
        Vector2 pos = transform.position;

        float dir = (pos - point).normalized.x;
        scale.x = Math.Abs(scale.x) * Math.Sign(dir == 0 ? 1 : dir);
        transform.localScale = scale;
    }

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
    }

    public virtual void ReactToStim(Element element, Stim stim) {
        throw new NotImplementedException();
    }
}
