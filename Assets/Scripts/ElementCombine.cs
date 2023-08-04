using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCombine : MonoBehaviour
{
    [SerializeField] GameObject[] potions;
    public int chosenElem1;
    public int chosenElem2;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(chosenElem1 != 0 && chosenElem2 != 0 && chosenElem1 != chosenElem2)
                Combine();      
            else
                Debug.Log("Invalid recipe");
        }
        if(Input.GetKeyDown(KeyCode.Backspace))
            chosenElem1 = chosenElem2 = 0;

        if (Input.GetKeyDown(KeyCode.Q))
            SelectElement(1);
        if (Input.GetKeyDown(KeyCode.W))
            SelectElement(2);
        if (Input.GetKeyDown(KeyCode.E))
            SelectElement(3);
    }

    void SelectElement(int selectedElement)
    {
        Debug.Log("selecting element");

        if(chosenElem1 == 0)
            chosenElem1 = selectedElement;
        else if(chosenElem2 == 0)
            chosenElem2 = selectedElement;

        Debug.Log("selected elements " + chosenElem1 + " : " + chosenElem2);
    }

    void Combine()
    {
        int createdPotion = chosenElem1 + chosenElem2 - 3;
        chosenElem1 = chosenElem2 = 0;
        Debug.Log("Made potion: " + potions[createdPotion].name);
        Instantiate(potions[createdPotion],transform.GetChild(0).position, Quaternion.identity, null);
        
    }
}
