using UnityEngine;

public enum Element {
    Salt,
    Acid,
    Kindling
}

public interface StimResponder {
    void ReactToStim(Element element, Stim stim);
}

public enum Stim {
    Blindness, /// The afflicted can't see (or e.g. protect their eyes if they should be immune)
    Burning,   /// The afflicted starts burning
    Obscured   /// The afflicted can't be seen by others - for better or for worse.
}

/// A generic class for all throwables.
/// A throwable is some element, shown both in the UI and the world, which applies a STIM (a stimulant)
/// either on hit or on entering a zone with the effect.
[ExecuteInEditMode]
[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Throwable : MonoBehaviour {
    /// What element this throwable actually represents.
    [SerializeField] Element element;

    /// What effects the throwable causes on hit.
    [SerializeField] Stim stim;

    // NOTE(Mario):
    //   Явно вече има променлива "name" в клас Object.
    //   За да не пищи компилатора се слага това "new".
    /// What this throwable is called.
    [SerializeField] new string name;

    /// A description of what this throwable does.
    [Multiline(5)]
    [SerializeField] string description;

    /// Does the throwable create a field of the stim after hitting?
    [SerializeField] bool spawnFieldOnHit = false;

    /// The icon shown in the UI for this element.
    [Header("Visuals")]
    [SerializeField] Sprite icon;

    /// The sprite used to visualize this throwable in the world.
    [SerializeField] Sprite thrownSprite;

    [Header("Sounds")]
    [SerializeField] public AudioClip soundOnPick;
    [SerializeField] public AudioClip soundOnThrow;
    [SerializeField] public AudioClip soundOnHit;

    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;

    void OnEnable() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void OnValidate() {
        if(rigidbody2D == null)
            rigidbody2D = GetComponent<Rigidbody2D>();
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        spriteRenderer.sprite = thrownSprite;

        rigidbody2D.gravityScale = 4.9f;
        rigidbody2D.angularDrag = 0.5f;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (spawnFieldOnHit && false)
        {
            // TODO(Mario):
            //   Трябва всъщност да направя полето да работи.
            Instantiate(null, transform.position, Quaternion.identity, null);
        }
        else
        {
            StimResponder stimResponder = other.gameObject.GetComponent<StimResponder>();
            if(stimResponder != null)
                stimResponder.ReactToStim(element, stim);
        }


        // Play the on hit sound when we impact something.
        // TODO(Mario):
        //   Искаме ли да добавяме вариация към звука - да му променяме тона, например?
        //   Ще е полезно ако могат да бъдат метнати няколко Throwable-и едновременно.
        if(soundOnHit != null)
            AudioSource.PlayClipAtPoint(soundOnHit, transform.position);

        Destroy(gameObject);
    }
}

// Salt Bag:
//   A bag of finely crushed salt crystals, used in high-profile cooking.
//   Causes temporary blindness.
//   Combines with acid to create a noxious smoke cloud, obscuring everyone inside.

// Acid Bottle:
//   A bottle of some unknown corrosive fluid.
//   Combines with salt to generate a noxious smoke cloud, obscuring everyone inside.

// Lit Kindling:
//   A bunch of kindling quickly ignited and thrown.
//   Essentially our fire potion.
