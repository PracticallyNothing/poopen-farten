using UnityEngine;

public class GroundColliderScript : MonoBehaviour
{
        PlayerMovement movement;

        void Start()
        {
            movement = GetComponentInParent<PlayerMovement>();
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Floor"))
                return;

            movement.isGrounded = true;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            movement.isGrounded = false;
        }
}
