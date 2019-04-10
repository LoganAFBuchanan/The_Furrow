using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIntroText : MonoBehaviour
{

    public Text title;
    public Text body;
    public Text button;

    public bool triggerFade = false;

    private float alphaVal = 0;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(triggerFade)
        {
            if(alphaVal < 1f)
            {
                alphaVal += Time.deltaTime * speed;
                Color fadedColor = new Color(255, 255, 255, alphaVal);

                title.color = fadedColor;
                body.color = fadedColor;
                button.color = fadedColor;
            }
           

        }
    }


    public void TriggerTheFade()
    {
        triggerFade = true;
    }
}
