using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class InventoryAndPotions : MonoBehaviour
{
    [SerializeField] GameObject[] allThrowables;
    [SerializeField] GameObject[] hotbar;

    [SerializeField] GameObject heldThrowable = null;
    bool isAiming = false;

    private Rigidbody2D playerRigidBody2D;
    private AudioSource audioSource;

    void Start()
    {
        playerRigidBody2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
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

        var soundOnPick = heldThrowable.GetComponent<Throwable>().soundOnPick;
        if(soundOnPick != null)
            audioSource.PlayOneShot(soundOnPick);
    }

    // NOTE(Mario):
    //   Това НЕ ТРЯБВА ДА Е FixedUpdate()!
    //   Вътре има логика за засичане на мишката и клавиатурата, която трябва да се изпълнява всеки кадър!
    void Update()
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
            isAiming = true;

        // If we're aiming and the user presses RMB, cancel the throw.
        else if (isAiming && Input.GetMouseButtonDown(1))
        {
            heldThrowable.transform.localPosition = new Vector3(-1, 0, 0); // Reset the potion's location.
            isAiming = false;
        }

        // If we're aiming and the user lets go of LMB, do the actual throw.
        else if (isAiming && Input.GetMouseButtonUp(0))
        {
            // Put the throwable in such a place so its outside the player's hitbox.
            heldThrowable.transform.position = transform.position + dir.normalized * 5;
            heldThrowable.transform.SetParent(null, true);

            // Add the player's velocity to the throwable's, add more force and some torque.
            var rigidBody = heldThrowable.GetComponent<Rigidbody2D>();
            rigidBody.simulated = true;
            rigidBody.velocity = playerRigidBody2D.velocity / 1.2f;
            rigidBody.AddForce(dir * 25, ForceMode2D.Impulse);
            rigidBody.AddTorque(65);

            // Play the sound when the throwable is thrown.
            var soundOnThrow = heldThrowable.GetComponent<Throwable>().soundOnThrow;
            if(soundOnThrow != null)
                audioSource.PlayOneShot(soundOnThrow);

            heldThrowable = null;
            isAiming = false;
        }
    }
}
