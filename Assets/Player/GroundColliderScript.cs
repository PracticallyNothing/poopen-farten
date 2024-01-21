using UnityEngine;

public class GroundColliderScript : MonoBehaviour
{
    PlayerMovement movement;

    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Floor"))
            return;

        movement.canJump = true;
    }

    void OnTriggerExit2D(Collider2D other) {
        Debug.Log("Can't jump anymore!");
        movement.canJump = false;
    }
}
