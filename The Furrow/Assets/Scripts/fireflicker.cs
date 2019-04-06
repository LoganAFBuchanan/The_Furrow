using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireflicker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0f,1f) < 0.1)
        {
            GetComponent<Light>().intensity = Random.Range(1.3f, 1.5f);
        }
        
    }
}
