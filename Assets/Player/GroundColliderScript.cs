using UnityEngine;

public class GroundColliderScript : MonoBehaviour
{
    PlayerMovement movement;

    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Floor"))
            return;

        movement.canJump = true;
    }

    void OnCollisionExit2D(Collision2D other)
    {
        Debug.Log("Can't jump anymore!");
        movement.canJump = false;
    }
}
