using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Element {
    None,

    Acid,
    Sulfur,
    Salt,
}

[Serializable]
public class PotionRecipe {
    public Element[] ingredients;
    public GameObject result;
}

public class ElementCombine : MonoBehaviour
{
    [SerializeField]
    public PotionRecipe[] recipes;

    [SerializeField]
    public List<Element> selectedElements = new();

    GameObject heldPotion = null;
    Rigidbody2D myBody = null;

    bool isAiming = false;

    void Start() {
        myBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            MixElement(Element.Salt);
        if (Input.GetKeyDown(KeyCode.W))
            MixElement(Element.Sulfur);
        if (Input.GetKeyDown(KeyCode.E))
            MixElement(Element.Acid);

        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.Normalize();

        if(heldPotion != null) {
            if(isAiming) {
                heldPotion.transform.localPosition = new Vector3(-1, 0, -1) - dir;
            }

            // Upon pressing LMB, activate aiming mode.
            if(Input.GetMouseButtonDown(0)) {
                isAiming = true;
            }

            // If we're aiming and the user presses RMB, cancel the throw.
            if(isAiming && Input.GetMouseButtonDown(1)) {
                heldPotion.transform.localPosition = new Vector3(-1, 0, 0); // Reset the potion's location.
                isAiming = false;
            }

            // If we're aiming and the user lets go of LMB, do the actual throw.
            if(isAiming && Input.GetMouseButtonUp(0)) {
                var rigidBody = heldPotion.GetComponent<Rigidbody2D>();
                rigidBody.simulated = true;

                heldPotion.transform.position = transform.position + dir.normalized * 5;
                heldPotion.transform.SetParent(null, true);

                rigidBody.velocity = myBody.velocity / 1.2f;
                rigidBody.AddForce(dir * 25, ForceMode2D.Impulse);
                rigidBody.AddTorque(25);

                heldPotion = null;
                isAiming = false;

                // Only clear the elements after throwing so the player knows what potion
                // they've created.
                selectedElements.Clear();
            }
        }
    }

    // Attempt to add an element to the current mix.
    // Return whether the mixing succeeded.
    bool MixElement(Element e)
    {
        // Don't allow mixing elements if a potion has already been created.
        if(heldPotion != null) {
            Debug.LogError("You're already holding a potion!");
            return false;
        }

        if(selectedElements.Contains(e)) {
            Debug.LogError(string.Format(
                "Nope, {0} is present in the mix!",
                e
            ));
            return false;
        }

        selectedElements.Add(e);

        // Try to see if the current combo matches a known recipe.
        // If it does, finish the potion and spawn it.
        var elemsSet = selectedElements.ToHashSet();

        var foundRecipe = recipes
          .Where((p) => p.ingredients.Length == p.ingredients.Intersect(elemsSet).Count())
          .FirstOrDefault();

        // If a potion that matches the player's combination exists, spawn it.
        if(foundRecipe != null) {
            Combine(foundRecipe);
        }

        return true;
    }

    void Combine(PotionRecipe recipe)
    {
        if(recipe == null)
            return;

        heldPotion = Instantiate(
            recipe.result,
            transform.position + new Vector3(-1, 0, 0),
            Quaternion.identity,
            gameObject.transform
        );

        // Disable physics and collision on the potion.
        heldPotion.GetComponent<Rigidbody2D>().simulated = false;
    }
}
