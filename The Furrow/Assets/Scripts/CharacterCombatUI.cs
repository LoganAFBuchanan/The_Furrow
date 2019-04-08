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
    private Image armorBar;

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
        armorBar = transform.Find("HealthBarBackground").GetChild(1).GetComponent<Image>();

        attackButton = transform.Find("SkillContainer").Find("AttackButton").gameObject;
        defendButton = transform.Find("SkillContainer").Find("Defend").gameObject;

        for(int i = 1; i < 4; i++)
        {
            skillSlots.Add(transform.Find("SkillContainer").Find("Skill" + i).gameObject);
        }

        int index = 1;
        foreach(GameObject skillObject in skillSlots)
        {
            skillObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);

            Debug.Log("Found skill object: " + skillObject.name);
            if(index == 1 && hero.skill2 != null) 
            {
                skillObject.GetComponent<Image>().sprite = hero.skill2.skillImage;
                skillObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if(index == 2 && hero.skill3 != null) 
            {
                skillObject.GetComponent<Image>().sprite = hero.skill3.skillImage;
                skillObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            
            index++;
        }
    }

    public void Update()
    {
        if(APText != null) APText.text = hero.ActionPoints.ToString();
        UpdateHealthAndArmor();
    }

    public void ConnectGUIController(GUIController gui)
    {
        Debug.Log("CONNECTING THE GUI");
        combatGUI = gui;
        Initialize();

        GetComponent<Canvas>().worldCamera = GameObject.Find("CombatCamera").GetComponent<Camera>();
        Debug.Log(combatGUI.gameObject.name);
    }

    public void UpdateHealthAndArmor()
    {
        healthBar.fillAmount = Map(hero.HitPoints, 0, hero.TotalHitPoints, 0, 1);
        armorBar.fillAmount = Map(hero.DefenceFactor, 0, hero.TotalHitPoints, 0, 1);
        
    }

    public void ShowSkills(bool setting)
    {
        SkillList.SetActive(setting);
    }

    public void OnSkillClicked(GameObject hoveredObject)
    {
        GetComponent<PlaySFX>().PlayFrom(Camera.main.gameObject.transform);
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

    //Adapted from several answers here: https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
    public static float Map(float from, float fromMin, float fromMax, float toMin,  float toMax) 
    {
        var fromAbs  =  from - fromMin;
        var fromMaxAbs = fromMax - fromMin;      
       
        var normal = fromAbs / fromMaxAbs;
 
        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;
 
        var to = toAbs + toMin;
       
        return to;
    }

   
}
