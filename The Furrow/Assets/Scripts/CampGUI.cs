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
    }


    public void OnBackToOverworldClick()
    {
        SceneManager.LoadScene(0);
    }

}