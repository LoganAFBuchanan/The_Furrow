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
        currNode.flowchart.SetStringVariable("biome", currNode.biome);
        currNode.flowchart.SetIntegerVariable("goldCount", goldCount);
        currNode.flowchart.SetIntegerVariable("rationCount", rationCount);
        currNode.flowchart.SetStringVariable("char1Name", "Aldric");
        currNode.flowchart.SetStringVariable("char2Name", "Ide");
    }




}