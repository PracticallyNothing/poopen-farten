using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCamera : MonoBehaviour
{
    [SerializeField] GameObject Player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Player.transform.position.y > transform.position.y)
            transform.Translate(0,15f*Time.deltaTime*(Player.transform.position.y-transform.position.y), 0);

    }
}
