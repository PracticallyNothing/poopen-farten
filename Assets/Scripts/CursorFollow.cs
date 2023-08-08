using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    GameObject player;
    
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    void Update() {
        var playerPos = player.transform.position;
        var cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = playerPos + (cursorPos - playerPos) / 3;
    }
}