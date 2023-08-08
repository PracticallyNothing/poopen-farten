using System;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public void OnHit(GameObject target) {
        var enemy = target.GetComponent<EnemyScript>();
        if(enemy != null) {
            enemy.health -= 1;
        }
    }

    DateTime aliveSince;
    
    [SerializeField]
    float durationSeconds = 3;

    void Start() {
        aliveSince = DateTime.Now;
    }

    void FixedUpdate() {
        if((DateTime.Now - aliveSince).Seconds > durationSeconds) {
            Destroy(gameObject);
        }
    }
}