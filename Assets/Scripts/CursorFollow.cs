using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    GameObject player;
    [SerializeField] Vector2 offset;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    void Update() {
        Vector3 offsetVec3 = offset;
        offsetVec3.z = 0;

        var playerPos = player.transform.position;
        var cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offsetVec3;
        transform.position = playerPos + (cursorPos - playerPos) / 3;
    }
}