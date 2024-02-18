using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class StoryInteractible : MonoBehaviour
{
    Collider2D triggerCollider;
    [SerializeField] GameObject interactionKeySprite;

    void OnEnable() {
        triggerCollider = GetComponent<BoxCollider2D>();
    }

    void OnValidate()
    {
        if(triggerCollider == null)
            triggerCollider = GetComponent<BoxCollider2D>();

        triggerCollider.isTrigger = true;
        tag = "Interactible";
    }

    // Start is called before the first frame update
    void Start()
    {
        interactionKeySprite.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            interactionKeySprite.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            interactionKeySprite.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
