using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OverworldPlayer : MonoBehaviour
{

    public List<HeroControl> characterList;
    public int goldCount;
    public int rationCount;

    public int bondLevel;
    public int bondCount;
    public int bondMax;

    public MapNode currNode;

    public event EventHandler NodeChanged;

    // Start is called before the first frame update
    void Start()
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




}