using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class CampGUI : OverworldGUI
{

    public GameObject choicePanel;

    private Button restButton;
    private Button huntButton;
    private Button bondButton;
 
    public override void Initialize()
    {
        base.Initialize();

        restButton = choicePanel.transform.GetChild(0).gameObject.GetComponent<Button>();
        huntButton = choicePanel.transform.GetChild(1).gameObject.GetComponent<Button>();
        bondButton = choicePanel.transform.GetChild(2).gameObject.GetComponent<Button>();

        restButton.onClick.AddListener(OnRestClicked);
        huntButton.onClick.AddListener(OnHuntClicked);
        bondButton.onClick.AddListener(OnBondClicked);


    }

    public void OnRestClicked()
    {
        Debug.Log("Rest Clicked!");
    }

    public void OnHuntClicked()
    {
        Debug.Log("Hunt Clicked!");
    }

    public void OnBondClicked()
    {
        Debug.Log("Bond Clicked!");
    }


    public void OnBackToOverworldClick()
    {
        SceneManager.LoadScene(0);
    }

}