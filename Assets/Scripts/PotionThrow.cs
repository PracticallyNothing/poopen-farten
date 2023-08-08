using UnityEngine;

public class PotionThrow : MonoBehaviour
{
    public GameObject EffectOnShatter = null;

    void OnCollisionEnter2D(Collision2D other)
    {
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