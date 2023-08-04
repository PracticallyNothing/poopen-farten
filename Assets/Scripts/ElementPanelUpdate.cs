using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ElementPanelUpdate : MonoBehaviour
{
    [SerializeField] Sprite[] elementIcons;

    // Update is called once per frame
    void Update()
    {
        int iconIndex = 0;

        iconIndex = FindAnyObjectByType<ElementCombine>().chosenElem1;
        transform.GetChild(0).GetComponent<Image>().sprite = elementIcons[iconIndex];

        iconIndex = FindAnyObjectByType<ElementCombine>().chosenElem2;
        transform.GetChild(1).GetComponent<Image>().sprite = elementIcons[iconIndex];

    }
}
