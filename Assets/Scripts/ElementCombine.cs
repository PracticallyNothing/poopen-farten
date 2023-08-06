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
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            MixElement(Element.Salt);
        if (Input.GetKeyDown(KeyCode.W))
            MixElement(Element.Sulfur);
        if (Input.GetKeyDown(KeyCode.E))
            MixElement(Element.Acid);
    }

    // Attempt to add an element to the current mix.
    // Return whether the mixing succeeded.
    bool MixElement(Element e)
    {
        if(selectedElements.Contains(e)) {
            Debug.Log(string.Format(
                "Nope, element {0} is present in the mix!",
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

        // If a potion that matches the player's combination exists,
        // spawn it and then clear the list of elements.
        if(foundRecipe != null) {
            Debug.Log("Found recipe!");
            selectedElements.Clear();
            Combine(foundRecipe);
        }

        return true;
    }

    void Combine(PotionRecipe recipe)
    {
        if(recipe == null)
            return;
        
        Instantiate(
            recipe.result,
            transform.GetChild(0).position,
            Quaternion.identity,
            null
        );
    }
}
