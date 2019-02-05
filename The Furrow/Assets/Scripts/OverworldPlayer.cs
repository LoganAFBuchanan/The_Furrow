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

    // Start is called before the first frame update
    void Start()
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
        
        currNode = dest;
        currNode.isTaken = true;
        currNode.PlayerInNode();
    }

    public void ExecuteFlowchart(string startBlock)
    {
        SetEncounterVariables();
        currNode.flowchart.ExecuteBlock(startBlock);
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

    public void GetEncounterVariables()
    {
        //currNode.biome = currNode.flowchart.GetStringVariable("biome");
        goldCount = currNode.flowchart.GetIntegerVariable("goldCount");
        rationCount = currNode.flowchart.GetIntegerVariable("rationCount");
        //currNode.flowchart.GetStringVariable("char1Name");
        //currNode.flowchart.GetStringVariable("char2Name");
    }




}