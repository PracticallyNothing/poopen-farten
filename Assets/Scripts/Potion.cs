using UnityEngine;

public class Potion : MonoBehaviour
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