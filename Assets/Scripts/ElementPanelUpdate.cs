using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ElementIcon {
    public Element element;
    public Sprite icon;
}

public class ElementPanelUpdate : MonoBehaviour
{
    [SerializeField] 
    ElementIcon[] elementIcons = {};

    ElementCombine combiner = null;
    Sprite emptyIcon = null;

    void Start()
    {
        combiner = FindAnyObjectByType<ElementCombine>();

        emptyIcon = elementIcons
          .First((x) => x.element == Element.None)
          .icon;
    }

    // Update is called once per frame
    void Update()
    {
        var firstComponent = transform.GetChild(0).GetComponent<Image>();
        var secondComponent = transform.GetChild(1).GetComponent<Image>();

        firstComponent.sprite = emptyIcon;
        secondComponent.sprite = emptyIcon;

        if(combiner.selectedElements.Count > 0) {
            var elem = combiner.selectedElements[0];
            firstComponent.sprite = elementIcons
              .First((x) => x.element == elem)
              .icon;
        }

        if(combiner.selectedElements.Count > 1) {
            var elem = combiner.selectedElements[1];
            secondComponent.sprite = elementIcons
              .First((x) => x.element == elem)
              .icon;
        }
    }
}
