using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CharacterCombatUI : MonoBehaviour
{

    public HeroControl hero;
    private OverworldGUI overworldGUI;
    

    private Text APText;
    private Image healthBar;

    private GameObject attackButton;
    private GameObject defendButton;

    public GameObject SkillList;

    private List<GameObject> skillSlots;

    [System.NonSerialized]
    public GUIController combatGUI;

    // Start is called before the first frame update
    public void Initialize()
    {

        skillSlots = new List<GameObject>();
        overworldGUI = GameObject.Find("OverworldUI").GetComponent<OverworldGUI>();

        APText = transform.Find("ActionPointBackground").GetChild(0).GetComponent<Text>();
        healthBar = transform.Find("HealthBarBackground").GetChild(0).GetComponent<Image>();

        attackButton = transform.Find("SkillContainer").Find("AttackButton").gameObject;
        defendButton = transform.Find("SkillContainer").Find("Defend").gameObject;

        for(int i = 1; i < 4; i++)
        {
            skillSlots.Add(transform.Find("SkillContainer").Find("Skill" + i).gameObject);
        }

        foreach(GameObject skillObject in skillSlots)
        {
            Debug.Log("Found skill object: " + skillObject.name);
        }
    }

    public void Update()
    {
        APText.text = hero.ActionPoints.ToString();
    }

    public void ConnectGUIController(GUIController gui)
    {
        Debug.Log("CONNECTING THE GUI");
        combatGUI = gui;

        GetComponent<Canvas>().worldCamera = GameObject.Find("CombatCamera").GetComponent<Camera>();
        Debug.Log(combatGUI.gameObject.name);
    }

    public void ShowSkills(bool setting)
    {
        SkillList.SetActive(setting);
    }

    public void OnSkillClicked(GameObject hoveredObject)
    {
        switch(hoveredObject.name)
        {
            case "AttackButton":
                combatGUI.OnSkill1Clicked();
                break;

            case "Defend":
                combatGUI.DefendButtonClicked();
                break;

            case "Skill1":
                combatGUI.OnSkill2Clicked();
                break;
            
            case "Skill2":
                combatGUI.OnSkill3Clicked();
                break;
            
            case "Skill3":
                break;
        }
    }

    public void OnHoverEnter(GameObject hoveredObject)
    {
        Debug.Log(hoveredObject.name + " hovered for " + hero.UnitName);
        Debug.Log(combatGUI);
        switch(hoveredObject.name)
        {
            case "AttackButton":
                combatGUI.OnSkill1HoverEnter();
                break;

            case "Defend":
                break;

            case "Skill1":
                combatGUI.OnSkill2HoverEnter();
                break;
            
            case "Skill2":
                combatGUI.OnSkill3HoverEnter();
                break;
            
            case "Skill3":
                break;
        }
        
    }

    public void OnHoverExit(GameObject hoveredObject)
    {
        Debug.Log(hoveredObject.name + " hover exited for " + hero.UnitName);
        switch(hoveredObject.name)
        {
            case "AttackButton":
                combatGUI.OnSkill1HoverExit();
                break;

            case "Defend":
                break;

            case "Skill1":
                combatGUI.OnSkill2HoverExit();
                break;
            
            case "Skill2":
                combatGUI.OnSkill3HoverExit();
                break;
            
            case "Skill3":
                break;
        }
    }

   
}
