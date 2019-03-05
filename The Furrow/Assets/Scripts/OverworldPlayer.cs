using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;

public class OverworldPlayer : MonoBehaviour
{

    public List<HeroControl> characterList;
    public int goldCount;
    public int rationCount;

    public int bondLevel;
    public int bondCount;
    public int bondMax;

    public MapNode currNode;

    public event System.EventHandler NodeChanged;
    public event System.EventHandler StatsChanged;

    public void Awake()
    {
        if(GameObject.FindGameObjectsWithTag("Player").Length > 1)
        {
            Destroy(this.gameObject);
        }
    }

    // Sets up the player
    public void Initialize()
    {
        
        
            characterList = new List<HeroControl>();
            foreach(Transform child in transform)
            {
                characterList.Add(child.gameObject.GetComponent<HeroControl>());

            }

            foreach(HeroControl hero in characterList)
            {
                Debug.Log(hero.UnitName);
                if(hero.gameObject.activeSelf)
                {
                    hero.Initialize();
                    hero.gameObject.SetActive(false);
                }
            }

            bondLevel = Constants.STARTING_BOND_LEVEL;
            goldCount = Constants.STARTING_GOLD;
            rationCount = Constants.STARTING_RATIONS;
            bondMax = Constants.BOND_MAX_LEVEL_1;

            DontDestroyOnLoad(this.gameObject);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovePlayer(MapNode dest)
    {
        if(currNode == null) 
        {
            
        }
        else
        {
            currNode.isTaken = false;
            currNode.OnDeHighlightNode();
        }
        
        //Ration reduction when moving
        if(rationCount > 0)
        {
            rationCount--;
        }
        else
        {
            foreach(HeroControl hero in characterList)
            {
                //Reduce character health if ration count is 0
                hero.HitPoints -= Constants.STARVING_COST;
            }
        }
        
        currNode = dest;
        currNode.isTaken = true;
        StatsChanged.Invoke(this, new EventArgs());
        currNode.PlayerInNode();
    }

    public void ExecuteFlowchart(string startBlock)
    {
        
        if(!currNode.isVisited)
        {
            SetEncounterVariables();
            currNode.flowchart.ExecuteBlock(startBlock);
            currNode.isVisited = true;
        }
        else
        {
            //Move Darkness Forwards
        }
    }

    public void SetEncounterVariables()
    {
        //currNode.flowchart.SetStringVariable("biome", currNode.biome);
        currNode.flowchart.SetIntegerVariable("goldCount", goldCount);
        currNode.flowchart.SetIntegerVariable("rationCount", rationCount);

        currNode.flowchart.SetIntegerVariable("bondCount", bondCount);
        currNode.flowchart.SetIntegerVariable("bondMax", bondMax);
        currNode.flowchart.SetIntegerVariable("bondLevel", bondLevel);

        currNode.flowchart.SetIntegerVariable("char1Health", characterList[0].HitPoints);
        currNode.flowchart.SetIntegerVariable("char1MaxHealth", characterList[0].TotalHitPoints);

        currNode.flowchart.SetIntegerVariable("char2Health", characterList[1].HitPoints);
        currNode.flowchart.SetIntegerVariable("char2MaxHealth", characterList[1].TotalHitPoints);

        Debug.Log("CHARACTER 1 NAME IS BEFORE: " + currNode.flowchart.GetStringVariable("char1Name"));
        currNode.flowchart.SetStringVariable("char1Name", characterList[0].UnitName);
        currNode.flowchart.SetStringVariable("char2Name", characterList[1].UnitName);
        Debug.Log("CHARACTER 1 NAME IS NOW: " + currNode.flowchart.GetStringVariable("char1Name"));


    }

    
    public void SetGoldCount(int change)
    {
        goldCount += change;
        if(goldCount < 0) goldCount = 0;
        StatsChanged.Invoke(this, new EventArgs());
    }

    public void SetRationCount(int change)
    {
        rationCount += change;
        if(rationCount < 0) rationCount = 0;
        StatsChanged.Invoke(this, new EventArgs());
    }

    public void SetBondCount(int change)
    {
        bondCount += change;
        if(bondCount < 0) bondCount = 0;
        if(bondCount > bondMax) bondCount = bondMax;
        StatsChanged.Invoke(this, new EventArgs());
    }

    public void SetChar1Health(int change)
    {
        characterList[0].HitPoints += change;
        if(characterList[0].HitPoints > characterList[0].TotalHitPoints) characterList[0].HitPoints = characterList[0].TotalHitPoints;
        StatsChanged.Invoke(this, new EventArgs());
    }

    public void SetChar2Health(int change)
    {
        characterList[1].HitPoints += change;
        if(characterList[1].HitPoints > characterList[1].TotalHitPoints) characterList[1].HitPoints = characterList[1].TotalHitPoints;
        StatsChanged.Invoke(this, new EventArgs());
    }

    public void SetChar1MaxHealth(int change)
    {
        characterList[0].TotalHitPoints += change;
        characterList[0].HitPoints += change;
        StatsChanged.Invoke(this, new EventArgs());
    }

    public void SetChar2MaxHealth(int change)
    {
        characterList[1].TotalHitPoints += change;
        characterList[1].HitPoints += change;
        StatsChanged.Invoke(this, new EventArgs());
    }

    public void GetEncounterVariables()
    {
        //currNode.biome = currNode.flowchart.GetStringVariable("biome");
        goldCount = currNode.flowchart.GetIntegerVariable("goldCount");
        rationCount = currNode.flowchart.GetIntegerVariable("rationCount");
        //currNode.flowchart.GetStringVariable("char1Name");
        //currNode.flowchart.GetStringVariable("char2Name");
        StatsChanged.Invoke(this, new EventArgs());
    }




}