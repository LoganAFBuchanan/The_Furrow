using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{

    public Text title;
    public Text button1;
    public Text button2;

    public bool triggerFade = false;

    private float alphaVal = 1;

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
            if(alphaVal > 0)
            {
                alphaVal -= Time.deltaTime * speed;
                Color fadedColor = new Color(255, 255, 255, alphaVal);

                title.color = fadedColor;
                button1.color = fadedColor;
                button2.color = fadedColor;
            }else{
                GameObject.Find("Menu").SetActive(false);
            }
           

        }
    }

    public void ShowPreamble()
    {
        GetComponent<PlaySFX>().PlayFromCamera();
        GameObject.Find("Preamble").GetComponent<FadeIntroText>().TriggerTheFade();
        triggerFade = true;
    }

    public void StartTheFuckingGameYouBastardYouDontKnowMeYoureNotGOD()
    {
        GetComponent<PlaySFX>().PlayFromCamera();
        StartCoroutine(GameObject.Find("Fade").GetComponent<SceneFader>().FadeAndLoadScene( SceneFader.FadeDirection.In, 1));
    }
}
