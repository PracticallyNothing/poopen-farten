using UnityEngine;

public class StoryInteractible : MonoBehaviour
{
    [SerializeField] Collider2D triggerCollider;
    [SerializeField] GameObject interactionKeySprite;

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
