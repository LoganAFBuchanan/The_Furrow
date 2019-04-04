using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCamera : MonoBehaviour
{

    public float scrollPercentage;
    public float scrollSpeed;

    public float mapTop;
    public float mapBottom;

    public bool inEncounter;

    // Start is called before the first frame update
    void Awake()
    {
        inEncounter = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.mousePosition.y > (Screen.height - (Screen.height * scrollPercentage)) && transform.position.z < mapTop && !inEncounter)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + scrollSpeed * Time.deltaTime);
        }

        if(Input.mousePosition.y < (Screen.height * scrollPercentage) && transform.position.z > mapBottom && !inEncounter)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - scrollSpeed * Time.deltaTime);
        }
    }
}
