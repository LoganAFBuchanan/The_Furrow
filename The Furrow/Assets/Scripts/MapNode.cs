using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapNode : MonoBehaviour
{

    public string biome;
    public int worldTier;
    public int tierPosition;

    public bool isTaken;

    [System.NonSerialized]
    public float moveDistance;

    List<MapNode> accessNodes;
    public event EventHandler HoverExit;

    // Start is called before the first frame update
    void Awake()
    {
        isTaken = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAccessNodes(List<MapNode> nodeList)
    {
        accessNodes = new List<MapNode>();
        float distance;
        foreach(MapNode node in nodeList)
        {
            distance = Vector3.Distance(transform.position, node.transform.position);
            if(distance <= moveDistance && distance != 0)
            {
                accessNodes.Add(node);
            }
        }
    }

    private void OnMouseEnter() 
    {
        Debug.Log("MouseEntered!");
        foreach(MapNode node in accessNodes)
        {
            Debug.Log("Highlighting Node: " + node);
            node.OnHighlightNode();
        }
    }

    private void OnMouseExit() 
    {
        HoverExit.Invoke(this, new EventArgs());
    }

    public void OnHighlightNode()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void OnDeHighlightNode()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void SetPosition()
    {
        transform.position = new Vector3((tierPosition * 10) + (worldTier * 5), 0, (worldTier * 10));
    }
}
