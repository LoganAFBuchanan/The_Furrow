using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using UnityEngine.SceneManagement;

public class MapNode : MonoBehaviour
{

    public string biome;
    public int encounterID;
    public int worldTier;
    public int tierPosition;
    public bool isUnique;
    
    [System.NonSerialized]
    public bool isVisited;

    public bool isTaken;

    [System.NonSerialized]
    public float moveDistance;

    [System.NonSerialized]
    public Vector3 savedPosition;

    [System.NonSerialized]
    public float positionAdjust;

    public List<MapNode> accessNodes;
    public event System.EventHandler HoverEnter;
    public event System.EventHandler HoverExit;
    public event System.EventHandler NodeClicked;
    public event System.EventHandler PlayerEntered;

    public Flowchart flowchart;

    public List<GameObject> enemyList;

    // Start is called before the first frame update
    public void Initialize()
    {
        isVisited = false;
        isTaken = false;
        accessNodes = new List<MapNode>();
        flowchart = GetComponent<Flowchart>();
        enemyList = new List<GameObject>();

        for(int i = 0; i < transform.childCount; i++)
        {
            enemyList.Add(transform.GetChild(i).gameObject);
        }
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
        //Debug.Log("MouseEntered!");
        HoverEnter.Invoke(this, new EventArgs());
    }

    private void OnMouseExit() 
    {
        HoverExit.Invoke(this, new EventArgs());
    }

    private void OnMouseUpAsButton() 
    {
        Debug.Log("The Node Recognizes this as a button click!");
        NodeClicked.Invoke(this, new EventArgs());
    }

    public void OnHighlightNode()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void OnDeHighlightNode()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void OnPlayerHighlight()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void PlayerInNode()
    {
        PlayerEntered.Invoke(this, new EventArgs());
    }

    //Evenly spaces out nodes. Will be randomized later for more interesting layouts
    public void SetPosition()
    {
        float adjustVal = moveDistance/positionAdjust;

        Vector3 randomAdjustment = new Vector3(UnityEngine.Random.Range(-adjustVal, adjustVal), 0, UnityEngine.Random.Range(-adjustVal, adjustVal));

        transform.position = new Vector3((tierPosition * 10) + (worldTier * 5), 0, (worldTier * 10));
        transform.position += randomAdjustment;
    }

    public void UpdateVariables()
    {
        Debug.Log("EVENT IS OVER!!!");
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.GetEncounterVariables();
    }

    public void AddRandomArtifact()
    {
        Debug.Log("Player Gains an Artifact!");
    }

    public void FightEnemyGroup(int groupNum)
    {
        Debug.Log("Player fights enemy group " + groupNum + "!");
        DontDestroyOnLoad(this.gameObject);
        savedPosition = transform.position;
        transform.SetParent(null);
        transform.position = new Vector3(0, 0, 0);
        SceneManager.LoadScene(1);
    }
}

