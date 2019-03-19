using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using UnityEngine.SceneManagement;

public class MapControl : MonoBehaviour
{

    public bool nodesEnabled = true;

    public GameObject node;
    public List<MapNode> nodeList;
    private UnityEngine.Object[] encounterObjectList;
    private List<GameObject> encounterPreFabList;

    [System.NonSerialized]
    public OverworldPlayer playerScript;

    public GameObject playerObject;

    public int tierNumber;
    public int maxPositions;

    public bool isFirstMove;

    public event System.EventHandler MapGenerated;
    public event System.EventHandler ValuesChanged;

    [System.NonSerialized]
    public Vector3 savedPosition;

    public float moveDistance;
    public float positionAdjust; // determines the extent to which nodes are scattered

    private int initialSceneIndex;

    // Start is called before the first frame update
    void Awake()
    {
        if(GameObject.FindGameObjectsWithTag("MapControl").Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        GatherEncounterObjects();
        
        //GenerateMap();

        //Need to set access nodes for premade map
        SetupPremadeMap();

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

        playerScript.Initialize();
        playerScript.StatsChanged += OnStatsChanged;

        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        initialSceneIndex = SceneManager.GetActiveScene().buildIndex;

        //MapGenerated.Invoke(this, new EventArgs());
    }

    //Collect all encounter objects from the resources folder
    private void GatherEncounterObjects()
    {
        encounterPreFabList = new List<GameObject>();

        encounterObjectList = Resources.LoadAll("Encounters", typeof(GameObject));

        foreach(UnityEngine.Object obj in encounterObjectList)
        {
            GameObject encounterObject = (obj as UnityEngine.GameObject);
            Debug.Log(obj);

            encounterPreFabList.Add(encounterObject);
        }
    }


    //Generates a pyramid structure map of all the same node
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
                newNodeScript.moveDistance = moveDistance;
                newNodeScript.positionAdjust = positionAdjust;
                newNodeScript.SetPosition();

                

                newNodeScript.HoverExit += OnNodeHoverExit;
                newNodeScript.HoverEnter += OnNodeHoverEnter;
                newNodeScript.NodeClicked += OnNodeClicked;
                newNodeScript.PlayerEntered += OnPlayerEnterNode;

                newNodeScript.Initialize();

                nodeList.Add(newNodeScript);
            }
        }

        foreach(MapNode node in nodeList)
        {
            node.SetAccessNodes(nodeList);
        }

    }


    //Configures the access nodes for a premade map
    public void SetupPremadeMap()
    {
        
        for(int i = 0; i < this.transform.childCount; i++)
        {
            nodeList.Add(this.transform.GetChild(i).GetComponent<MapNode>());
        }

        foreach(MapNode node in nodeList)
        {
            node.moveDistance = moveDistance;
            node.positionAdjust = positionAdjust;

            node.HoverExit += OnNodeHoverExit;
            node.HoverEnter += OnNodeHoverEnter;
            node.NodeClicked += OnNodeClicked;
            node.PlayerEntered += OnPlayerEnterNode;

            node.positionID = UnityEngine.Random.Range(0, 1000000);

            node.Initialize();
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
        if(nodesEnabled)
        {
            MapNode clickedNode = (sender as MapNode);
            Debug.Log("Node CLicked!");
            if(clickedNode.isTaken)
            {
                Debug.Log("Cannot Move Node is Taken");
                return;
            }

            foreach (MapNode node in playerScript.currNode.accessNodes)
            {
                if(node.positionID == clickedNode.positionID)
                {
                    playerScript.MovePlayer(clickedNode);
                    if(isFirstMove) isFirstMove = false;
                    
                    playerScript.ExecuteFlowchart("Start");
                    //playerScript.currNode.flowchart.ExecuteBlock("Start");
                }
            }
        }
        else
        {
            
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

    public void OnStatsChanged(object sender, EventArgs e)
    {
        ValuesChanged.Invoke(this, new EventArgs());
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

    private void CleanUpDelegates()
    {
        foreach(Delegate d in MapGenerated.GetInvocationList())
        {
            MapGenerated -= (System.EventHandler)d;
        }
        
        foreach(Delegate d in ValuesChanged.GetInvocationList())
        {
            ValuesChanged -= (System.EventHandler)d;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MapGenerated.Invoke(this, new EventArgs());

        //if(scene.buildIndex != initialSceneIndex) CleanUpDelegates();

        Debug.Log("YUP that scene done changed to" + scene.name);
    }
    
}
