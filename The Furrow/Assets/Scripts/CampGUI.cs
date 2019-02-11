using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Fungus;

public class CampGUI : OverworldGUI
{

    public GameObject choicePanel;

    private Button restButton;
    private Button huntButton;
    private Button bondButton;

    private OverworldPlayer playerScript;
 
    public override void Initialize()
    {
        base.Initialize();

        restButton = choicePanel.transform.GetChild(0).gameObject.GetComponent<Button>();
        huntButton = choicePanel.transform.GetChild(1).gameObject.GetComponent<Button>();
        bondButton = choicePanel.transform.GetChild(2).gameObject.GetComponent<Button>();

        restButton.onClick.AddListener(OnRestClicked);
        huntButton.onClick.AddListener(OnHuntClicked);
        bondButton.onClick.AddListener(OnBondClicked);

        playerScript = mapControlScript.playerScript;


    }

    public void OnRestClicked()
    {
        Debug.Log("Rest Clicked!");
        List<HeroControl> heroList = playerScript.characterList;

        foreach(HeroControl hero in heroList)
        {
            if(hero.HitPoints < hero.TotalHitPoints)
            {
                hero.HitPoints += Mathf.FloorToInt(hero.TotalHitPoints * Constants.CAMP_REST_PCT);

                if(hero.HitPoints > hero.TotalHitPoints) hero.HitPoints = hero.TotalHitPoints;
            }
        }
        UpdateUIValues();
        BackToOverworld();

    }

    public void OnHuntClicked()
    {
        Debug.Log("Hunt Clicked!");
        playerScript.rationCount += Constants.CAMP_HUNT_RATIONS;
        UpdateUIValues();
        BackToOverworld();
    }

    public void OnBondClicked()
    {
        Debug.Log("Bond Clicked!");

        if(playerScript.bondCount == playerScript.bondMax)
        {
            //Bond Level up! Initiate appropriate flowchart
            choicePanel.SetActive(false);
            GameObject.Find("BondMoment1").GetComponent<Flowchart>().ExecuteBlock("Start");
        }
        else
        {
            playerScript.bondCount += Constants.CAMP_BOND_INC;
            if(playerScript.bondCount > playerScript.bondMax) playerScript.bondCount = playerScript.bondMax;
            UpdateUIValues();
            BackToOverworld();
        }
    }


    public void OnBackToOverworldClick()
    {
        BackToOverworld();
    }


    public void BackToOverworld()
    {
        //Move Map Back
        mapControlScript.transform.position = mapControlScript.savedPosition;
        SceneManager.LoadScene(0);
    }

    public override void UpdateUIValues()
    {
        base.UpdateUIValues();
    }


    public void BondLevelUp()
    {
        Debug.Log("CHARACTERS HAVE BONDED!!!");
    }

}