using UnityEngine;

public class PotionThrow : MonoBehaviour
{
    public GameObject EffectOnShatter = null;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Potion"))
            return;

        if(EffectOnShatter != null) {
            Instantiate(
                EffectOnShatter,
                transform.position,
                Quaternion.identity,
                null
            );
        }

        Destroy(gameObject);
    }
}