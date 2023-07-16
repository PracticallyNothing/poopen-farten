using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{   
    [SerializeField] float heightBetweenPlatforms = 6;
    [SerializeField] GameObject platform;

    StaticCamera cam;
    float lastPlacedPlatformHeight = 0;

    public int shit = 1;

    // fuck this bitch
    void Start() {
        cam = FindObjectOfType<StaticCamera>();
        lastPlacedPlatformHeight = heightBetweenPlatforms;
    }

    // Update is called once per frame
    void Update()
    {
        float newHeight = lastPlacedPlatformHeight + heightBetweenPlatforms;
        if(cam.transform.position.y + 6 > newHeight) {
            GameObject.Instantiate(
                platform,
                new Vector3(Random.Range(-6, 6), newHeight, 0),
                Quaternion.identity);
            lastPlacedPlatformHeight = newHeight;
            
        }
    }

    public void GeneratePlatform() {
    }
}
