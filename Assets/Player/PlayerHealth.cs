using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int playerMaxHealth = 5;
    [SerializeField] float invincibleTime = 1.5f;
    [SerializeField] GameObject respawnPoint;

    int playerHealth;
    bool playerAlive = true;
    float timeTracker;

    void Start()
    {
        playerHealth = playerMaxHealth;
    }

    private void FixedUpdate()
    {
        //  vrushta geroq obratno na normalniq layer sled kato izteche vremeto
        if(gameObject.layer == 6)
        {
            timeTracker += Time.deltaTime;
            if(timeTracker >= invincibleTime)
            {
                gameObject.layer = 0;
                timeTracker = 0;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        //  igracha poema shtetite
        playerHealth -= damage;

        //  vremenno premestva geroq na layer koito ne reagira s layera na koito sa oponentite
        gameObject.layer = 6;

        //  respawnva geroq ako umira ot shtetite
        if(playerHealth <= 0)
        {
            playerAlive = false;
            PlayerRespawn();
        }
    }

    void PlayerRespawn()
    {
        transform.position = respawnPoint.transform.position;
        transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        playerHealth = playerMaxHealth;
    }

}
