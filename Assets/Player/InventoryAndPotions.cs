using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class InventoryAndPotions : MonoBehaviour
{
    [SerializeField] GameObject[] allThrowables;
    [SerializeField] GameObject[] hotbar;

    [SerializeField] GameObject heldThrowable = null;
    bool isAiming = false;

    private Rigidbody2D playerRigidBody2D;

    void Start()
    {
        playerRigidBody2D = GetComponent<Rigidbody2D>();
    }

    /// Select a throwable from the hotbar based on its index and Instantiate()
    void PickThrowable(int hotbarIndex)
    {
        // If we're trying to pick an index that doesn't exist in the hotbar,
        // give up.
        if (hotbarIndex >= hotbar.Length)
            return;

        // If we're already holding something, don't pick a throwable.
        if (heldThrowable != null)
            return;

        heldThrowable = Instantiate(
            hotbar[hotbarIndex],
            transform.position + new Vector3(-1, 0, 0),
            Quaternion.identity,
            gameObject.transform
        );

        // Disable physics and collision on the throwable.
        heldThrowable.GetComponent<Rigidbody2D>().simulated = false;
    }

    void FixedUpdate()
    {
        // Pick the 1-st, 2-nd and 3-rd throwable on the hotbar using Q, W and E.
        KeyCode[] bindings = {
            KeyCode.Q,
            KeyCode.W,
            KeyCode.E
        };

        for (int i = 0; i < bindings.Length; i++)
        {
            if (Input.GetKey(bindings[i]))
            {
                PickThrowable(i);
                break;
            }
        }

        // Skip aiming logic if player isn't holding anything.
        if (heldThrowable == null)
            return;

        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.Normalize();

        if (isAiming)
            heldThrowable.transform.localPosition = new Vector3(-1, 0, -1) - dir;

        // Upon pressing LMB, activate aiming mode.
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Aiming...");
            isAiming = true;
        }

        // If we're aiming and the user presses RMB, cancel the throw.
        else if (isAiming && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Not aiming.");
            heldThrowable.transform.localPosition = new Vector3(-1, 0, 0); // Reset the potion's location.
            isAiming = false;
        }

        // If we're aiming and the user lets go of LMB, do the actual throw.
        else if (isAiming && Input.GetMouseButtonUp(0))
        {
            Debug.Log("Throwing!!!");
            var rigidBody = heldThrowable.GetComponent<Rigidbody2D>();
            rigidBody.simulated = true;

            heldThrowable.transform.position = transform.position + dir.normalized * 5;
            heldThrowable.transform.SetParent(null, true);

            rigidBody.velocity = playerRigidBody2D.velocity / 1.2f;
            rigidBody.AddForce(dir * 25, ForceMode2D.Impulse);
            rigidBody.AddTorque(25);

            heldThrowable = null;
            isAiming = false;
        }
    }
}
