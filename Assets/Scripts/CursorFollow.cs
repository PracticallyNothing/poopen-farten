using UnityEngine;

/// An object that has its position set between the Player GameObject and the cursor.
public class CursorFollow : MonoBehaviour
{
    GameObject player;
    [SerializeField] Vector2 offset;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    [SerializeField]
    [Range(0, 1)]
    float distanceFromPlayer = 0.5f;

    void Update() {
        Vector3 offsetVec3 = offset;
        offsetVec3.z = 0;

        var playerPos = player.transform.position;
        var cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offsetVec3;
        transform.position = playerPos + (cursorPos - playerPos) * distanceFromPlayer;
    }
}