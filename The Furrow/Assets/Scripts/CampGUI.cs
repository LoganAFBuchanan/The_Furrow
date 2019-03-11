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

    private List<GameObject> skillList;
    private List<GameObject> playerSkillList;

    public GameObject skillPanel;
    private Text skillPanelTitle;


    void Awake()
    {

        
            Initialize();

        
        
    }
 
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

        playerSkillList = GetPlayerSkills();

        GatherSkillObjects();

        


    }

    // Grab all skills from resources
    // NOTE: WILL NEED TO CHECK PLAYER OBJECTS TO SEE IF THEY CURRENTLY POSSESS ANY OF THESE SKILLS AND OMIT THEM
    public void GatherSkillObjects()
    {
        skillList = new List<GameObject>();

        UnityEngine.Object[] skillObjectList = Resources.LoadAll("Skills", typeof(GameObject));

        foreach(UnityEngine.Object obj in skillObjectList)
        {
            GameObject skillObject = (obj as UnityEngine.GameObject);
            //Debug.Log(obj);

            skillList.Add(skillObject);
        }
    }
    
    private List<GameObject> GetPlayerSkills()
    {

        List<GameObject> _PlayerSkills = new List<GameObject>();

        foreach(Transform heroChild in playerScript.transform)
        {
            foreach(Transform skillChild in heroChild)
            {
                if(skillChild.gameObject.GetComponent<Skill>())
                {
                    _PlayerSkills.Add(skillChild.gameObject);
                }
            }

        }

        return _PlayerSkills;
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

            switch(playerScript.bondLevel)
            {
                case 1:   
                    GameObject.Find("BondMoment1").GetComponent<Flowchart>().ExecuteBlock("Start");
                    break;
                
                case 2:
                    GameObject.Find("BondMoment1").GetComponent<Flowchart>().ExecuteBlock("Start");
                    break;
                
                case 3:
                    GameObject.Find("BondMoment1").GetComponent<Flowchart>().ExecuteBlock("Start");
                    break;
                
                case 4:
                    GameObject.Find("BondMoment1").GetComponent<Flowchart>().ExecuteBlock("Start");
                    break;
            }
            
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
        playerScript.bondCount = 0;
        
        foreach(HeroControl hero in playerScript.characterList)
        {
            hero.HitPoints += Constants.LEVEL_UP_HP_INC;
            hero.TotalHitPoints += Constants.LEVEL_UP_HP_INC;
        }

        switch(playerScript.bondLevel)
        {
            case 1:

                playerScript.bondMax = Constants.BOND_MAX_LEVEL_2;

                break;
            
            case 2:

                playerScript.bondMax = Constants.BOND_MAX_LEVEL_3;

                break;
            
            case 3:

                playerScript.bondMax = Constants.BOND_MAX_LEVEL_4;

                break;
            
            case 4:

                playerScript.bondMax = Constants.BOND_MAX_LEVEL_5;

                break;
        }

        skillPanel.SetActive(true);
        List<GameObject> chosenSkills = PickRandomSkills(skillList);
        AttachSkillsToUI(chosenSkills);
    }

    private List<GameObject> PickRandomSkills(List<GameObject> resourceList)
    {
        //Grab 4 Random Skills from pool 
        //NOTE: Need to make it so that 2 Aldric and 2 Ide skills are chosen each time

        List<GameObject> selectedList = new List<GameObject>();

        int numberOfSkills = 4;

        for(int i = 0; i < resourceList.Count; i++)
        {
            if(selectedList.Count >= numberOfSkills) break;

            float randomVal = UnityEngine.Random.Range(0.0f, 1.0f);
            float pickChance = ((float)numberOfSkills - (float)selectedList.Count) / ((float)resourceList.Count - (float)i);
            Debug.Log("RandomVal = " + randomVal + "|| PickChance = " + pickChance);

            if(randomVal <= pickChance)
            {
                selectedList.Add(resourceList[i]);
                Debug.Log("Added Skill: " + resourceList[i].GetComponent<Skill>().skillname);
            }

        }

        return selectedList;

    }

    private void AttachSkillsToUI(List<GameObject> _chosenSkills)
    {
        //Slot each skill into UI Button
        //Remember to set listenrs for this using UNity Events to avoid delegate errors
        
        List<GameObject> skillCards = new List<GameObject>();
        foreach(Transform skillChoiceTransform in skillPanel.transform)
        {
            if(skillChoiceTransform.gameObject.GetComponent<Button>()) skillCards.Add(skillChoiceTransform.gameObject);
        }

        int i = 0;

        foreach(GameObject skillCard in skillCards)
        {
            Text skillName = skillCard.transform.GetChild(0).GetComponent<Text>();
            Text skillDesc = skillCard.transform.GetChild(1).GetComponent<Text>();
            Image skillImage = skillCard.transform.GetChild(2).GetComponent<Image>();

            skillName.text = _chosenSkills[i].GetComponent<Skill>().skillname;
            skillDesc.text = _chosenSkills[i].GetComponent<Skill>().character;
            //Add skill image here

            //Add listener
            GameObject thisSkill = _chosenSkills[i];

            skillCard.GetComponent<Button>().onClick.AddListener(delegate 
            { 
                Debug.Log("Made it into the Button!");
                GiveSkill(thisSkill, thisSkill.GetComponent<Skill>().character); 
            });

            i++;
        }


    }

    public void GiveSkill(GameObject skill, string characterName)
    {
        foreach(Transform heroChild in playerScript.transform)
        {
            if(heroChild.gameObject.GetComponent<HeroControl>().UnitName == characterName) 
            {
                HeroControl heroScript = heroChild.GetComponent<HeroControl>();

                if(heroScript.skillObject2 == null)
                {
                    GameObject realSkill = GameObject.Instantiate(skill);

                    realSkill.transform.SetParent(heroChild);
                    heroScript.skillObject2 = realSkill;
                }
                else if(heroScript.skillObject3 == null)
                {
                    GameObject realSkill = GameObject.Instantiate(skill);

                    realSkill.transform.SetParent(heroChild);
                    heroScript.skillObject3 = realSkill;
                }
                else
                {
                    //Enable replacement stuff
                }
                heroScript.AttachSkills();
            }
        }
        BackToOverworld();
    }

}