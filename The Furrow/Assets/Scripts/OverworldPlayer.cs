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
    void Awake()
    {
        
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


        Debug.Log("CHARACTER 1 NAME IS BEFORE: " + currNode.flowchart.GetStringVariable("char1Name"));
        currNode.flowchart.SetStringVariable("char1Name", "Aldric");
        currNode.flowchart.SetStringVariable("char2Name", "Ide");
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