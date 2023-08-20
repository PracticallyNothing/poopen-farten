using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]
    public int health = 5;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Potion")) {
            other.gameObject.GetComponent<Potion>().OnHit(gameObject);
        }
    }
}