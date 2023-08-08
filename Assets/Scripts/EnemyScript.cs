using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int health = 5;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Potion")) {
            other.gameObject.GetComponent<Potion>().OnHit(gameObject);
        }
    }

    void Start() {
    }

    void FixedUpdate() {
        if(health <= 0) {
            Destroy(gameObject);
        }
    }
}