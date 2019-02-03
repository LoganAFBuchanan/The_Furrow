using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapControl : MonoBehaviour
{

    public GameObject node;
    public List<MapNode> nodeList;
    //Need a list of all nodes
    //Think like cellgrid

    public int tierNumber;
    public int maxPositions;

    public float moveDistance;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
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
                newNodeScript.moveDistance = moveDistance;

                nodeList.Add(newNodeScript);
            }
        }

        foreach(MapNode node in nodeList)
        {
            node.SetAccessNodes(nodeList);
        }

    }

    public void OnNodeHoverExit(object sender, EventArgs e)
    {
        foreach(MapNode node in nodeList)
        {
            node.OnDeHighlightNode();
        }
    }
}
