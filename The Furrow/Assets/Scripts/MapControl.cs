using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;

public class MapControl : MonoBehaviour
{

    public GameObject node;
    public List<MapNode> nodeList;
    //Need a list of all nodes
    //Think like cellgrid

    private OverworldPlayer playerScript;

    public GameObject playerObject;

    public int tierNumber;
    public int maxPositions;

    public bool isFirstMove;

    public float moveDistance;

    // Start is called before the first frame update
    void Start()
    {

        
        GenerateMap();

        if(GameObject.Find("Player") != null)
        {
            playerScript = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        }
        else
        {
            GameObject.Instantiate(playerObject);
            playerScript = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        }

        if(playerScript.currNode == null)
        {
            isFirstMove = true;
            SetFirstMovement();
        }
        else
        {
            checkHighlights();
        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMap()
    {
        for(int i = 0; i < tierNumber; i++)
        {
            for(int j = 0; j < maxPositions - i; j++)
            {
                GameObject newNode = Instantiate(node);
                MapNode newNodeScript = newNode.GetComponent<MapNode>();

                newNode.transform.SetParent(this.transform);
                newNodeScript.worldTier = i;
                newNodeScript.tierPosition = j;
                newNodeScript.SetPosition();

                newNodeScript.HoverExit += OnNodeHoverExit;
                newNodeScript.HoverEnter += OnNodeHoverEnter;
                newNodeScript.moveDistance = moveDistance;
                newNodeScript.NodeClicked += OnNodeClicked;
                newNodeScript.PlayerEntered += OnPlayerEnterNode;

                nodeList.Add(newNodeScript);
            }
        }

        foreach(MapNode node in nodeList)
        {
            node.SetAccessNodes(nodeList);
        }

    }

    public void SetFirstMovement()
    {
        playerScript.currNode = new MapNode();
        playerScript.currNode.accessNodes = new List<MapNode>(); 

        //Set all first level nodes to moveable
        foreach(MapNode node in nodeList)
        {
            if(node.worldTier == 0)
            {
                playerScript.currNode.accessNodes.Add(node);
            }
        }

        //Highlight all movable nodes
        checkHighlights();
        // foreach(MapNode node in playerScript.currNode.accessNodes)
        // {
        //     Debug.Log("Highlighting Node: " + node);
        //     node.OnHighlightNode();
        // }

        isFirstMove = false;
    }

    public void OnNodeHoverEnter(object sender, EventArgs e)
    {
        // MapNode hoveredNode = (sender as MapNode);

        // foreach(MapNode node in hoveredNode.accessNodes)
        // {
        //     Debug.Log("Highlighting Node: " + node);
        //     node.OnHighlightNode();
        // }
    }

    public void OnNodeHoverExit(object sender, EventArgs e)
    {
        // foreach(MapNode node in nodeList)
        // {
        //     node.OnDeHighlightNode();
        //     if(node.isTaken)
        //     {
        //         node.PlayerInNode();
        //     }
        // }
    }


    public void OnNodeChange(object sender, EventArgs e)
    {

    }

    public void OnNodeClicked(object sender, EventArgs e)
    {
        MapNode clickedNode = (sender as MapNode);

        if(clickedNode.isTaken)
        {
            return;
        }

        foreach (MapNode node in playerScript.currNode.accessNodes)
        {
            if(node.tierPosition == clickedNode.tierPosition && node.worldTier == clickedNode.worldTier)
            {
                playerScript.MovePlayer(clickedNode);
                playerScript.ExecuteFlowchart("Start");
                //playerScript.currNode.flowchart.ExecuteBlock("Start");
            }
        }
    }

    public void OnPlayerEnterNode(object sender, EventArgs e)
    {
        MapNode enteredNode = (sender as MapNode);
        checkHighlights();
        // foreach(MapNode node in enteredNode.accessNodes)
        // {
        //     Debug.Log("Highlighting Node: " + node);
        //     node.OnHighlightNode();
        //     if(node.isTaken) node.OnPlayerHighlight();
        // }
    }

    public void checkHighlights()
    {
        foreach(MapNode node in nodeList)
        {
            node.OnDeHighlightNode();
            if(node.isTaken) node.OnPlayerHighlight();
        }
        foreach(MapNode accessNode in playerScript.currNode.accessNodes)
        {
            accessNode.OnHighlightNode();
        }
    }
    
}
